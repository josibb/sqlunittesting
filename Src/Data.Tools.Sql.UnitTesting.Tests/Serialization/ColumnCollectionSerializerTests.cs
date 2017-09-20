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
    public class ColumnCollectionSerializerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadXmlTHrowsWithNullReader()
        {
            new ColumnCollectionSerializer().Deserialize(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteXmlTHrowsWithNullWriter()
        {
            new ColumnCollectionSerializer().Serialize(null, new ColumnCollection());
        }

        [TestMethod]
        public void WriteXmlThrowsWithNullCollection()
        {
            using (var s = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(s))
                {
                    Assert.ThrowsException<ArgumentNullException>(() => {
                        new ColumnCollectionSerializer().Serialize(writer, null);
                    });
                }
            }
        }

        [TestMethod]
        public void ReadXmlThrowsIfReaderNotAtColumnsElement()
        {
            using (var r = new TestXmlReader("<incorrectnode/>"))
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    new ColumnCollectionSerializer().Deserialize(r.Reader);
                });
            }
        }

        [TestMethod]
        public void CanReadXmlForColumnCollectionWith2Columns()
        {
            var xml = new StringBuilder();
            xml.Append("<Columns>");
            xml.Append("<Column name=\"cola\" dbType=\"varchar\" clrType=\"System.String\" />");
            xml.Append("<Column name=\"colb\" dbType=\"int\" clrType=\"System.Int32\" />");
            xml.Append("</Columns>");


            using (var r = new TestXmlReader(xml.ToString()))
            {
                var cc = new ColumnCollectionSerializer().Deserialize(r.Reader);

                Assert.IsNotNull(cc);
                Assert.AreEqual(2, cc.Count);
                Assert.IsNotNull(cc[0]);
                Assert.AreEqual("cola", cc[0].Name, "abc");
                Assert.AreEqual("varchar", cc[0].DbType);
                Assert.AreSame(typeof(string), cc[0].ClrType);
                Assert.IsNotNull(cc[1]);
                Assert.AreEqual("colb", cc[1].Name, "def");
                Assert.AreEqual("int", cc[1].DbType, "int");
                Assert.AreEqual(typeof(int), cc[1].ClrType);
            }
        }

        [TestMethod]
        public void CanReadXmlForColumnCollectionWith1Column()
        {
            var xml = new StringBuilder();
            xml.Append("<Columns>");
            xml.Append("<Column name=\"colc\" dbType=\"varchar\" clrType=\"System.String\" />");
            xml.Append("</Columns>");

            using (var r = new TestXmlReader(xml.ToString()))
            {
                var cc = new ColumnCollectionSerializer().Deserialize(r.Reader);

                Assert.IsNotNull(cc);
                Assert.AreEqual(1, cc.Count);
                Assert.IsNotNull(cc[0]);
                Assert.AreEqual("colc", cc[0].Name, "abc");
                Assert.AreEqual("varchar", cc[0].DbType);
                Assert.AreSame(typeof(string), cc[0].ClrType);
            }
        }

        [TestMethod]
        public void CanReadXmlForColumnCollectionWithoutColumns()
        {

            using (var r = new TestXmlReader("<Columns />"))
            {
                var cc = new ColumnCollectionSerializer().Deserialize(r.Reader);

                Assert.IsNotNull(cc);
                Assert.AreEqual(0, cc.Count);
            }

            using (var r = new TestXmlReader("<Columns></Columns>"))
            {
                var cc = new ColumnCollectionSerializer().Deserialize(r.Reader);

                Assert.IsNotNull(cc);
                Assert.AreEqual(0, cc.Count);
            }
        }

        [TestMethod]
        public void CanWriteXmlForColumnCollectionWithoutColumns()
        {
            var xml = new StringBuilder();
            xml.Append("<Columns />");

            using (var w = new TestXmlWriter())
            {
                new ColumnCollectionSerializer().Serialize(w.Writer, new ColumnCollection());

                Assert.AreEqual("<Columns />", w.Xml);
            }
        }

        [TestMethod]
        public void CanWriteXmlForColumnsWith1Column()
        {
            var xml = new StringBuilder();
            xml.Append("<Columns>");
            xml.Append("<Column name=\"colc\" dbType=\"int\" clrType=\"System.Int32\" />");
            xml.Append("</Columns>");

            var cc = new ColumnCollection();
            cc.Add(new Column { ClrType = typeof(int), DbType = "int", Name = "colc" });

            using (var w = new TestXmlWriter())
            {
                new ColumnCollectionSerializer().Serialize(w.Writer, cc);

                Assert.AreEqual(xml.ToString(), w.Xml);
            }
        }

        [TestMethod]
        public void CanWriteXmlForColimnsWith2Columns()
        {
            var xml = new StringBuilder();
            xml.Append("<Columns>");
            xml.Append("<Column name=\"cola\" dbType=\"varchar\" clrType=\"System.String\" />");
            xml.Append("<Column name=\"colb\" dbType=\"int\" clrType=\"System.Int32\" />");
            xml.Append("</Columns>");

            var cc = new ColumnCollection();
            cc.Add(new Column { ClrType = typeof(string), DbType = "varchar", Name = "cola" });
            cc.Add(new Column { ClrType = typeof(int), DbType = "int", Name = "colb" });

            using (var w = new TestXmlWriter())
            {
                new ColumnCollectionSerializer().Serialize(w.Writer, cc);

                Assert.AreEqual(xml.ToString(), w.Xml);
            }
        }

        [TestMethod]
        public void CanWriteAndReadXmlOfCollectionWith2Columns()
        {
            var cc = new ColumnCollection();

            cc.Add(new Column { ClrType = typeof(string), DbType = "varchar", Name = "abc" });
            cc.Add(new Column { ClrType = typeof(int), DbType = "int", Name = "def" });

            using (var w = new TestXmlWriter())
            {
                new ColumnCollectionSerializer().Serialize(w.Writer, cc);

                using (var r = new TestXmlReader(w.Xml))
                {
                    var cc2 = new ColumnCollectionSerializer().Deserialize(r.Reader);

                    Assert.IsNotNull(cc2);
                    Assert.AreEqual(2, cc2.Count);
                    Assert.AreEqual("abc", cc2[0].Name, "abc");
                    Assert.AreEqual("varchar", cc2[0].DbType, "varchar");
                    Assert.AreSame(typeof(string), cc2[0].ClrType);
                    Assert.AreEqual("def", cc2[1].Name, "def");
                    Assert.AreEqual("int", cc2[1].DbType, "int");
                    Assert.AreEqual(typeof(int), cc2[1].ClrType);
                }
            }
        }

        [TestMethod]
        public void CanWriteAndReadXmlOfCollectionWithoutColumns()
        {
            var cc = new ColumnCollection();

            using (var w = new TestXmlWriter())
            {
                new ColumnCollectionSerializer().Serialize(w.Writer, cc);

                using (var r = new TestXmlReader(w.Xml))
                {
                    var cc2 = new ColumnCollectionSerializer().Deserialize(r.Reader);

                    Assert.IsNotNull(cc2);
                    Assert.AreEqual(0, cc2.Count);
                }
            }
        }

        [TestMethod]
        public void CannotWriteXmlForColumnIfColumnIsInvalid()
        {
            using (var w = new TestXmlWriter())
            {
                var ex1 = new Exception();
                try
                {
                    new ColumnCollectionSerializer().Serialize(w.Writer, new ValidationTestColumnCollection { ValidationException = ex1 });
                }
                catch (Exception ex2)
                {
                    Assert.AreSame(typeof(InvalidOperationException), ex2.GetType());
                    Assert.AreSame(ex1, ex2.InnerException);

                    return;
                }

                Assert.Fail("Validation exception was not thrown");
            }
        }

        private class ValidationTestColumnCollection : ColumnCollection
        {
            public Exception ValidationException { get; set; }
            public override Exception Validate(bool checkProperties = false)
            {
                return ValidationException;
            }
        }
    }
}
