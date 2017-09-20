using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting.Serialization;
using Data.Tools.UnitTesting;
using System.Xml;
using System.IO;
using Data.Tools.UnitTesting.Tests.Utils;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Tests.Serialization
{
    [TestClass]
    public class ColumnSerializerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteXmlTHrowsWithNullWriter()
        {
            new ColumnSerializer().Serialize(null, new Column());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteXmlTHrowsWithNullColumn()
        {
            using (var s = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(s))
                {
                    new ColumnSerializer().Serialize(writer, null);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadXmlThrowsWithNullReader()
        {
            new ColumnSerializer().Deserialize(null);
        }

        [TestMethod]
        public void CanWriteXmlForColumn()
        {
            var expectedXml = "<Column name=\"cola\" dbType=\"varchar\" clrType=\"System.String\" />";

            using (var w = new TestXmlWriter())
            {
                new ColumnSerializer().Serialize(w.Writer,
                    new Column
                    {
                        ClrType = typeof(string),
                        DbType = "varchar",
                        Name = "cola"
                    });

                Assert.AreEqual(expectedXml, w.Xml);
            }
        }

        [TestMethod]
        public void CannotWriteXmlForInvalidColumn()
        {
            var c = new Column();

            using (var w = new TestXmlWriter())
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    new ColumnSerializer().Serialize(w.Writer, new Column());
                });
            }
        }

        [TestMethod]
        public void CanReadXmlForColumn()
        {
            using (var r = TestXmlReader.Create("<Column name=\"colb\" dbType=\"int\" clrType=\"System.Int32\" />"))
            {
                var c = new ColumnSerializer().Deserialize(r.Reader);

                Assert.IsNotNull(c);
                Assert.AreEqual("colb", c.Name);
                Assert.AreEqual("int", c.DbType);
                Assert.AreSame(typeof(System.Int32), c.ClrType);
            }
        }

        [TestMethod]
        public void ReadXmlForColumnTHrowsIfElementNameIsDifferent()
        {
            using (var r = TestXmlReader.Create("<IncorrectColumn name=\"colb\" dbType=\"int\" clrType=\"System.Int32\" />"))
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    new ColumnSerializer().Deserialize(r.Reader);
                });
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
                    new ColumnSerializer().Serialize(w.Writer, new ValidationTestColumn { ValidationException = ex1 });
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

        private class ValidationTestColumn: Column
        {
            public Exception ValidationException { get; set; }
            public override Exception Validate()
            {
                return ValidationException;
            }
        }
    }
}