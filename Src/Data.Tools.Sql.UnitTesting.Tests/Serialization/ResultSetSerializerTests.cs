using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting.Serialization;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using Data.Tools.UnitTesting.Tests.Utils;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Tests.Serialization
{
    [TestClass]
    public class ResultSetSerializerTests
    {
        #region Read tests

        [TestMethod]
        public void ReadXmlFailsIfNoSchemaElementIsDefined()
        {
            using (var r = new TestXmlReader("<ResultSet><Rows/></ResultSet>"))
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    new ResultSetSerializer().Deserialize(r.Reader);
                }
                , "Cannot find Schema in ResultSet");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadShouldTHrowWithNullReader()
        {
            new ResultSetSerializer().Deserialize(null);
        }

        [TestMethod]
        public void ReadShouldThrowIfInvalidElement()
        {
            using (var r = new TestXmlReader("<abc/>"))
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    new ResultSetSerializer().Deserialize(r.Reader);
                }, "Reader not at a ResultSet elemen");
            }
        }

        [TestMethod]
        public void CanReadXmlForEmptyResultSet()
        {
            var xml = new StringBuilder()
                .Append("<ResultSet>")
                    .Append("<Schema>")
                        .Append("<Columns/>")
                    .Append("</Schema>")
                    .Append("<Rows/>")
                .Append("</ResultSet>");

            using (var r = new TestXmlReader(xml.ToString()))
            {
                var rs = new ResultSetSerializer().Deserialize(r.Reader);

                Assert.IsNotNull(rs);
                Assert.IsNotNull(rs.Schema);
                Assert.IsNotNull(rs.Schema.Columns);
                Assert.AreEqual(0, rs.Schema.Columns.Count);
                Assert.IsNotNull(rs.Rows);
                Assert.AreEqual(0, rs.Rows.Count);
            }
        }

        [TestMethod]
        public void CanReadXmlForResultSet()
        {
            var xml = new StringBuilder()
                .Append("<ResultSet>")
                    .Append("<Schema>")
                        .Append("<Columns>")
                            .Append("<Column name=\"cola\" dbType=\"nvarchar\" clrType=\"System.String\" />")
                            .Append("<Column name=\"colb\" dbType=\"nvarchar\" clrType=\"System.String\" />")
                        .Append("</Columns>")
                    .Append("</Schema>")
                    .Append("<Rows>")
                        .Append("<Row><cola>test</cola></Row>")
                        .Append("<Row><colb>test</colb></Row>")
                    .Append("</Rows>")
                .Append("</ResultSet>");

            using (var r = new TestXmlReader(xml.ToString()))
            {
                var rs = new ResultSetSerializer().Deserialize(r.Reader);

                Assert.IsNotNull(rs);
                Assert.IsNotNull(rs.Schema);
                Assert.IsNotNull(rs.Schema.Columns);
                Assert.AreEqual(2, rs.Schema.Columns.Count);
                Assert.IsNotNull(rs.Rows);
                Assert.AreEqual(2, rs.Rows.Count);
            }
        }

        #endregion

        #region Write tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteShouldThrowWithNullWriter()
        {
            new ResultSetSerializer().Serialize(null, new ResultSet());
        }

        [TestMethod]
        public void WriteShouldThrowWithNullResultSet()
        {
            using (var w = new TestXmlWriter())
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new ResultSetSerializer().Serialize(w.Writer, null);
                });
            }
        }

        [TestMethod]
        public void CanWriteXmlForEmptyResultSet()
        {
            using (var w = new TestXmlWriter())
            {
                new ResultSetSerializer().Serialize(w.Writer, new ResultSet());

                var expectedXml = "<ResultSet><Schema><Columns /></Schema><Rows /></ResultSet>";
                Assert.AreEqual(expectedXml, w.Xml);
            }
        }

        [TestMethod]
        public void CanWriteXmlForResultSetWith2ColumnsAnd2Rows()
        {
            var rs = new ResultSet();
            rs.Schema.Columns.Add(new Column { Name = "Name", DbType = "nvarchar", ClrType = typeof(string) });
            rs.Schema.Columns.Add(new Column { Name = "Age", DbType = "int", ClrType = typeof(int) });
            rs.Rows.Add(new ResultSetRow());
            rs.Rows.Add(new ResultSetRow());

            rs.Rows[0]["Name"] = "tim";
            rs.Rows[0]["Age"] = 14;
            rs.Rows[1]["Name"] = "Smith";
            rs.Rows[1]["Age"] = 15;

            using (var w = new TestXmlWriter())
            {
                new ResultSetSerializer().Serialize(w.Writer, rs);

                var expectedXml = "<ResultSet><Schema><Columns><Column name=\"Name\" dbType=\"nvarchar\" clrType=\"System.String\" /><Column name=\"Age\" dbType=\"int\" clrType=\"System.Int32\" /></Columns></Schema><Rows><Row><Name>tim</Name><Age>14</Age></Row><Row><Name>Smith</Name><Age>15</Age></Row></Rows></ResultSet>";
                Assert.AreEqual(expectedXml, w.Xml);
            }
        }


        [TestMethod]
        public void WriteShouldThrowIfResultSetIsInvalid()
        {
            var ex1 = new Exception();

            using (var w = new TestXmlWriter())
            {
                try
                {
                    new ResultSetSerializer().Serialize(w.Writer, new TestResultSet { ValidationException = ex1 });
                }
                catch (Exception ex2)
                {
                    Assert.AreSame(typeof(InvalidOperationException), ex2.GetType());
                    Assert.AreSame(ex1, ex2.InnerException);
                    return;
                }

                Assert.Fail("Expected exception not thrown");
            }
        }

        #endregion

        [TestMethod]
        public void CanWriteAndReadResultSetXmlForEmptyResultSet()
        {
            using (var w = new TestXmlWriter())
            {
                new ResultSetSerializer().Serialize(w.Writer, new ResultSet());

                using (var r = new TestXmlReader(w.Xml))
                {
                    var rs = new ResultSetSerializer().Deserialize(r.Reader);

                    Assert.IsNotNull(rs);
                    Assert.IsNotNull(rs.Schema);
                    Assert.IsNotNull(rs.Schema.Columns);
                    Assert.AreEqual(0, rs.Schema.Columns.Count);
                    Assert.IsNotNull(rs.Rows);
                    Assert.AreEqual(0, rs.Rows.Count);
                }
            }
        }

        [TestMethod]
        public void CanWriteAndReadResultSetXmlForResultSetWith2ColumnsAnd2Rows()
        {
            var rs = new ResultSet();
            rs.Schema.Columns.Add(new Column { Name = "Name", DbType = "nvarchar", ClrType = typeof(string) });
            rs.Schema.Columns.Add(new Column { Name = "Age", DbType = "int", ClrType = typeof(int) });
            rs.Rows.Add(new ResultSetRow());
            rs.Rows.Add(new ResultSetRow());

            rs.Rows[0]["Name"] = "tim";
            rs.Rows[0]["Age"] = 14;
            rs.Rows[1]["Name"] = "Smith";
            rs.Rows[1]["Age"] = 15;

            using (var w = new TestXmlWriter())
            {
                new ResultSetSerializer().Serialize(w.Writer, rs);

                using (var r = new TestXmlReader(w.Xml))
                {
                    var rs2 = new ResultSetSerializer().Deserialize(r.Reader);

                    Assert.IsNotNull(rs2);
                    Assert.IsNotNull(rs2.Schema);
                    Assert.IsNotNull(rs2.Schema.Columns);
                    Assert.AreEqual(2, rs2.Schema.Columns.Count);
                    Assert.IsNotNull(rs2.Rows);
                    Assert.AreEqual(2, rs2.Rows.Count);
                }
            }
        }

        private class TestResultSet : ResultSet
        {
            public Exception ValidationException { get; set; }

            public override Exception Validate(bool checkProperties = false)
            {
                return ValidationException;
            }
        }
    }
}
