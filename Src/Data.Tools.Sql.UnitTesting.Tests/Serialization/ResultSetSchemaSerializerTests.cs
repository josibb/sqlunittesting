using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting.Serialization;
using Data.Tools.UnitTesting;
using System.IO;
using System.Xml;
using Data.Tools.UnitTesting.Tests.Utils;
using System.Text;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Tests.Serialization
{
    [TestClass]
    public class ResultSetSchemaSerializerTests
    {
        [TestMethod]
        public void CanReadXmlForResultSetSchema()
        {
            var xml = new StringBuilder();
            xml.Append("<Schema>");
            xml.Append("<Columns>");
            xml.Append("<Column name=\"cola\" dbType=\"varchar\" clrType=\"System.String\" />");
            xml.Append("<Column name=\"colb\" dbType=\"varchar\" clrType=\"System.String\" />");
            xml.Append("</Columns>");
            xml.Append("</Schema>");

            using (var r = new TestXmlReader(xml.ToString()))
            {
                var rss = new ResultSetSchemaSerializer().Deserialize(r.Reader);

                Assert.IsNotNull(rss);
                Assert.IsNotNull(rss.Columns);
                Assert.AreEqual(2, rss.Columns.Count);
            }
        }

        [TestMethod]
        public void ReadXmlShouldThrowIfReaderIsNotAtSchemaElement()
        {
            using (var r = new TestXmlReader("<abc/>"))
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    new ResultSetSchemaSerializer().Deserialize(r.Reader);
                }, "Reader not at a Schema elemen");
            }
        }

        [TestMethod]
        public void ReadXmlShouldThrowIfNoColumnsNodeFound()
        {
            using (var r = new TestXmlReader("<Schema></Schema>"))
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    new ResultSetSchemaSerializer().Deserialize(r.Reader);
                }, "No element Columns found in Schema");
            }
        }

        [TestMethod]
        public void CanWriteXmlForResultSetSchema()
        {
            var expectedXml = new StringBuilder();
            expectedXml.Append("<Schema>");
            expectedXml.Append("<Columns>");
            expectedXml.Append("<Column name=\"colc\" dbType=\"int\" clrType=\"System.Int32\" />");
            expectedXml.Append("</Columns>");
            expectedXml.Append("</Schema>");

            var rss = new ResultSetSchema();
            rss.Columns.Add(new Column { ClrType = typeof(int), DbType = "int", Name = "colc" });

            using (var w = new TestXmlWriter())
            {
                new ResultSetSchemaSerializer().Serialize(w.Writer, rss);

                Assert.AreEqual(expectedXml.ToString(), w.Xml);
            }
        }

        [TestMethod]
        public void CanWriteAndReadXmlForResultSetSchema()
        {
            var rss = new ResultSetSchema();
            rss.Columns.Add(new Column { ClrType = typeof(int), DbType = "int", Name = "colc" });

            using (var w = new TestXmlWriter())
            {
                new ResultSetSchemaSerializer().Serialize(w.Writer, rss);

                using (var r = new TestXmlReader(w.Xml))
                {
                    var rss2 = new ResultSetSchemaSerializer().Deserialize(r.Reader);

                    Assert.IsNotNull(rss2);
                    Assert.IsNotNull(rss2.Columns);
                    Assert.AreEqual(1, rss2.Columns.Count);
                    Assert.AreEqual("colc", rss2.Columns[0].Name);
                    Assert.AreEqual("int", rss2.Columns[0].DbType);
                    Assert.AreSame(typeof(int), rss2.Columns[0].ClrType);
                }
            }
        }




        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadXmlThrowsIfReaderIsNull()
        {
            new ResultSetSchemaSerializer().Deserialize(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteXmlTHrowsIfWriterIsNull()
        {
            new ResultSetSchemaSerializer().Serialize(null, new ResultSetSchema());
        }

        [TestMethod]
        public void WriteXmlThrowsIfSchemaIsNull()
        {
            using (var s = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(s))
                {
                    Assert.ThrowsException<ArgumentNullException>(() =>
                    {
                        new ResultSetSchemaSerializer().Serialize(writer, null);
                    });
                }
            }
        }

        [TestMethod]
        public void WriteOfInvalidSchemaShouldThrow()
        {
            var ex1 = new Exception();

            using (var w = new TestXmlWriter())
            {
                try
                {
                    new ResultSetSchemaSerializer().Serialize(w.Writer, new ResultSetSchemaSerializerValidationTest { ValidationException = ex1 });
                }
                catch (Exception ex2)
                {
                    Assert.AreSame(typeof(InvalidOperationException), ex2.GetType());
                    Assert.AreSame(ex1, ex2.InnerException);
                    return;
                }

                Assert.Fail("WriteXml did not throw exception for invalid ResultSetSchema");
            }
        }

        private class ResultSetSchemaSerializerValidationTest : ResultSetSchema
        {
            public Exception ValidationException { get; set; }
            public override Exception Validate(bool checkProperties = false)
            {
                return ValidationException;
            }
        }
    }
}
