using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Xml;
using Data.Tools.UnitTesting.Tests.Utils;
using System.IO;
using Data.Tools.UnitTesting.Serialization;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Tests.Serialization
{
    [TestClass]
    public class ActionResultSerializerTests
    {
        #region ReadXml tests..

        [TestMethod]
        public void ReadXmlThrowsWithNullReader()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ActionResultSerializer().ReadXml(null, new ActionResult());
            }, "reader");
        }

        [TestMethod]
        public void ReadXmlThrowsWithNullActionResult()
        {
            using (var r = new TestXmlReader("<ActionResult/>"))
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new ActionResultSerializer().ReadXml(r.Reader, null);
                }, "actionResult");
            }
        }

        [TestMethod]
        public void ReadXmlThrowsWhenReaderNoAtActionResultElement()
        {
            using (var r = new TestXmlReader("<abc/>"))
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    new ActionResultSerializer().ReadXml(r.Reader, new ActionResult());
                }, "Reader not at ActionResult element");
            }
        }

        [TestMethod]
        public void ReadXmlThrowForActionResultXmlWithoutResultSetsNode()
        {
            using (var r = new TestXmlReader("<ActionResult/>"))
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    new ActionResultSerializer().ReadXml(r.Reader, new ActionResult());
                }, "No ResultSets element found in ActionResult");
            }
        }

        [TestMethod]
        public void CanReadActionResultWithoutResultSets()
        {
            using (var r = new TestXmlReader("<ActionResult><ResultSets/></ActionResult>"))
            {
                var ar = new ActionResult();
                new ActionResultSerializer().ReadXml(r.Reader, ar);

                Assert.IsNotNull(ar.ResultSets);
                Assert.AreEqual(0, ar.ResultSets.Count);
            }
        }

        [TestMethod]
        public void CanReadActionResultFor2ResultSets()
        {
            var xml = new StringBuilder()
            .Append("<ActionResult>")
                .Append("<ResultSets>")

                    .Append("<ResultSet>")
                        .Append("<Schema>")
                            .Append("<Columns>")
                                .Append("<Column name=\"Id\" dbType=\"int\" clrType=\"System.Int32\" />")
                            .Append("</Columns>")
                        .Append("</Schema>")
                        .Append("<Rows>")
                            .Append("<Row>")
                            .Append("<Id>12</Id>")
                            .Append("</Row>")
                        .Append("</Rows>")
                    .Append("</ResultSet>")

                    .Append("<ResultSet>")
                        .Append("<Schema>")
                            .Append("<Columns>")
                                .Append("<Column name=\"Cola\" dbType=\"int\" clrType=\"System.Int32\" />")
                                .Append("<Column name=\"Colb\" dbType=\"int\" clrType=\"System.Int32\" />")
                            .Append("</Columns>")
                        .Append("</Schema>")
                        .Append("<Rows>")
                            .Append("<Row>")
                                .Append("<Cola>22</Cola>")
                                .Append("<Colb>22</Colb>")
                            .Append("</Row>")
                            .Append("<Row>")
                                .Append("<Cola>22</Cola>")
                                .Append("<Colb>22</Colb>")
                            .Append("</Row>")
                        .Append("</Rows>")
                    .Append("</ResultSet>")
                .Append("</ResultSets>")
            .Append("</ActionResult>");

            using (var r = new TestXmlReader(xml.ToString()))
            {
                var ar = new ActionResult();
                new ActionResultSerializer().ReadXml(r.Reader, ar);

                Assert.IsNotNull(ar.ResultSets);
                Assert.AreEqual(2, ar.ResultSets.Count);

                Assert.IsNotNull(ar.ResultSets[0]);
                Assert.IsNotNull(ar.ResultSets[0].Schema);
                Assert.IsNotNull(ar.ResultSets[0].Schema.Columns);
                Assert.AreEqual(1, ar.ResultSets[0].Schema.Columns.Count);
                Assert.IsNotNull(ar.ResultSets[0].Rows);
                Assert.AreEqual(1, ar.ResultSets[0].Rows.Count);

                Assert.IsNotNull(ar.ResultSets[1]);
                Assert.IsNotNull(ar.ResultSets[1].Schema);
                Assert.IsNotNull(ar.ResultSets[1].Schema.Columns);
                Assert.AreEqual(2, ar.ResultSets[1].Schema.Columns.Count);
                Assert.IsNotNull(ar.ResultSets[1].Rows);
                Assert.AreEqual(2, ar.ResultSets[1].Rows.Count);
            }
        }

        #endregion

        #region WriteXml tests..

        [TestMethod]
        public void WriteXmlThrowsWithNullWriter()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ActionResultSerializer().WriteXml(null, new ActionResult());
            }, "writer");

        }

        [TestMethod]
        public void WriteXmlTHrowsWithNullActionResult()
        {
            using (var w = new TestXmlWriter())
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new ActionResultSerializer().WriteXml(w.Writer, null);
                }, "actionResult");
            }
        }

        [TestMethod]
        public void WriteXmlTHrowsWithActionResultWithNullResultSets()
        {
            using (var w = new TestXmlWriter())
            {
                var ar = new ActionResult();
                ar.ResultSets = null;

                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    new ActionResultSerializer().WriteXml(w.Writer, ar);
                }, "actionResult.ResultSets is null");
            }
        }

        [TestMethod]
        public void WriteXmlTHrowsWithInvalidActionResult()
        {
            var ar = new TestActionResult { Exception = new Exception() };

            using (var w = new TestXmlWriter())
            {
                try
                {
                    new ActionResultSerializer().WriteXml(w.Writer, ar);
                } catch (Exception ex)
                {
                    Assert.AreSame(typeof(InvalidOperationException), ex.GetType());
                    Assert.AreEqual("ActionResult validation failed", ex.Message);
                    Assert.AreSame(ar.Exception, ex.InnerException);
                    return;
                }

                Assert.Fail("Expected exception not thrown");
            }
        }

        private class TestActionResult: ActionResult
        {
            public Exception Exception { get; set; }

            public override Exception Validate(bool checkRecursively = false)
            {
                return Exception;
            }
        }

        [TestMethod]
        public void CanSerializeActionResultWithoutResultSets()
        {
            var actionResult = new ActionResult();

            // declaration of actionresult node is ommitted. this is handled by the .net serializer
            var expectedXml = "<ResultSets />";

            using (var w = new TestXmlWriter())
            {
                new ActionResultSerializer().WriteXml(w.Writer, actionResult);

                Assert.AreEqual(expectedXml, w.Xml);
            }
        }

        [TestMethod]
        public void CanSerializeActionResultWith2ResultSets()
        {
            var dt = new DateTime(2009, 12, 25, 11, 48, 33).AddMilliseconds(983);

            var actionResult = new ActionResult();

            var rs1 = new ResultSet();
            actionResult.ResultSets.Add(rs1);
            rs1.Schema.Columns.Add(new Column { Name = "Id", DbType = "int", ClrType = typeof(int) });
            rs1.Schema.Columns.Add(new Column { Name = "Name", DbType = "varchar", ClrType = typeof(string) });
            rs1.Schema.Columns.Add(new Column { Name = "Price", DbType = "decimal", ClrType = typeof(decimal) });
            rs1.Schema.Columns.Add(new Column { Name = "Expiry", DbType = "date", ClrType = typeof(DateTime) });

            rs1.Rows.Add(new ResultSetRow());
            rs1.Rows[0]["Id"] = 12;
            rs1.Rows[0]["Name"] = "NVC001";
            rs1.Rows[0]["Price"] = 12.6m;
            rs1.Rows[0]["Expiry"] = dt;

            rs1.Rows.Add(new ResultSetRow());
            rs1.Rows[1]["Id"] = 12;
            rs1.Rows[1]["Name"] = "NVC001";
            rs1.Rows[1]["Price"] = 12.6m;
            rs1.Rows[1]["Expiry"] = null;

            // 2nd resultset
            var rs2 = new ResultSet();
            actionResult.ResultSets.Add(rs2);
            rs2.Schema.Columns.Add(new Column { Name = "Cola", DbType = "int", ClrType = typeof(int) });
            rs2.Schema.Columns.Add(new Column { Name = "Colb", DbType = "varchar", ClrType = typeof(string) });

            rs2.Rows.Add(new ResultSetRow());
            rs2.Rows[0]["Cola"] = 19;
            rs2.Rows[0]["Colb"] = null;

            // declaration of actionresult node is ommitted. this is handled by the .net serializer
            var expectedXml = "<ResultSets><ResultSet><Schema><Columns><Column name=\"Id\" dbType=\"int\" clrType=\"System.Int32\" /><Column name=\"Name\" dbType=\"varchar\" clrType=\"System.String\" /><Column name=\"Price\" dbType=\"decimal\" clrType=\"System.Decimal\" /><Column name=\"Expiry\" dbType=\"date\" clrType=\"System.DateTime\" /></Columns></Schema><Rows><Row><Id>12</Id><Name>NVC001</Name><Price>12.6</Price><Expiry>2009-12-25T11:48:33.983</Expiry></Row><Row><Id>12</Id><Name>NVC001</Name><Price>12.6</Price></Row></Rows></ResultSet><ResultSet><Schema><Columns><Column name=\"Cola\" dbType=\"int\" clrType=\"System.Int32\" /><Column name=\"Colb\" dbType=\"varchar\" clrType=\"System.String\" /></Columns></Schema><Rows><Row><Cola>19</Cola></Row></Rows></ResultSet></ResultSets>";

            using (var w = new TestXmlWriter())
            {
                new ActionResultSerializer().WriteXml(w.Writer, actionResult);

                Assert.AreEqual(expectedXml, w.Xml);
            }
        }

        #endregion

        [TestMethod]
        public void CanWriteAndReadActionResult()
        {
            var dt = new DateTime(2009, 12, 25, 11, 48, 33).AddMilliseconds(983);

            var actionResult = new ActionResult();

            var rs1 = new ResultSet();
            actionResult.ResultSets.Add(rs1);
            rs1.Schema.Columns.Add(new Column { Name = "Id", DbType = "int", ClrType = typeof(int) });
            rs1.Schema.Columns.Add(new Column { Name = "Name", DbType = "varchar", ClrType = typeof(string) });
            rs1.Schema.Columns.Add(new Column { Name = "Price", DbType = "decimal", ClrType = typeof(decimal) });
            rs1.Schema.Columns.Add(new Column { Name = "Expiry", DbType = "date", ClrType = typeof(DateTime) });

            rs1.Rows.Add(new ResultSetRow());
            rs1.Rows[0]["Id"] = 12;
            rs1.Rows[0]["Name"] = "NVC001";
            rs1.Rows[0]["Price"] = 12.6m;
            rs1.Rows[0]["Expiry"] = dt;

            rs1.Rows.Add(new ResultSetRow());
            rs1.Rows[1]["Id"] = 12;
            rs1.Rows[1]["Name"] = "NVC001";
            rs1.Rows[1]["Price"] = 12.6m;
            rs1.Rows[1]["Expiry"] = null;

            // 2nd resultset
            var rs2 = new ResultSet();
            actionResult.ResultSets.Add(rs2);
            rs2.Schema.Columns.Add(new Column { Name = "Cola", DbType = "int", ClrType = typeof(int) });
            rs2.Schema.Columns.Add(new Column { Name = "Colb", DbType = "varchar", ClrType = typeof(string) });

            rs2.Rows.Add(new ResultSetRow());
            rs2.Rows[0]["Cola"] = 19;
            rs2.Rows[0]["Colb"] = null;

            using (var w = new TestXmlWriter())
            {
                new ActionResultSerializer().WriteXml(w.Writer, actionResult);

                using (var r = new TestXmlReader($"<ActionResult>{w.Xml}</ActionResult>")) // embed in actionresult (see serializer for comments)
                {
                    var ar = new ActionResult();
                    new ActionResultSerializer().ReadXml(r.Reader, ar);

                    Assert.IsNotNull(ar.ResultSets);
                    Assert.AreEqual(2, ar.ResultSets.Count);

                    Assert.IsNotNull(ar.ResultSets[0]);
                    Assert.IsNotNull(ar.ResultSets[0].Schema);
                    Assert.IsNotNull(ar.ResultSets[0].Schema.Columns);
                    Assert.AreEqual(4, ar.ResultSets[0].Schema.Columns.Count);
                    Assert.IsNotNull(ar.ResultSets[0].Rows);
                    Assert.AreEqual(2, ar.ResultSets[0].Rows.Count);
                }
            }
        }


        #region XmlSerializer tests..

        [TestMethod]
        public void CanSerializeActionResultUsingXmlSerializer()
        {
            var dt = new DateTime(2009, 12, 25, 11, 48, 33).AddMilliseconds(983);

            var actionResult = new ActionResult();

            var rs1 = new ResultSet();
            actionResult.ResultSets.Add(rs1);
            rs1.Schema.Columns.Add(new Column { Name = "Id", DbType = "int", ClrType = typeof(int) });
            rs1.Schema.Columns.Add(new Column { Name = "Name", DbType = "varchar", ClrType = typeof(string) });
            rs1.Schema.Columns.Add(new Column { Name = "Price", DbType = "decimal", ClrType = typeof(decimal) });
            rs1.Schema.Columns.Add(new Column { Name = "Expiry", DbType = "date", ClrType = typeof(DateTime) });

            rs1.Rows.Add(new ResultSetRow());
            rs1.Rows[0]["Id"] = 12;
            rs1.Rows[0]["Name"] = "NVC001";
            rs1.Rows[0]["Price"] = 12.6m;
            rs1.Rows[0]["Expiry"] = dt;

            rs1.Rows.Add(new ResultSetRow());
            rs1.Rows[1]["Id"] = 12;
            rs1.Rows[1]["Name"] = "NVC001";
            rs1.Rows[1]["Price"] = 12.6m;
            rs1.Rows[1]["Expiry"] = null;

            // 2nd resultset
            var rs2 = new ResultSet();
            actionResult.ResultSets.Add(rs2);
            rs2.Schema.Columns.Add(new Column { Name = "Cola", DbType = "int", ClrType = typeof(int) });
            rs2.Schema.Columns.Add(new Column { Name = "Colb", DbType = "varchar", ClrType = typeof(string) });

            rs2.Rows.Add(new ResultSetRow());
            rs2.Rows[0]["Cola"] = 19;
            rs2.Rows[0]["Colb"] = null;

            // declaration of actionresult node is ommitted. this is handled by the .net serializer
            var expectedXml = "<ActionResult xmlns=\"http://tempuri.org/ActionResult.xsd\"><ResultSets><ResultSet><Schema><Columns><Column name=\"Id\" dbType=\"int\" clrType=\"System.Int32\" /><Column name=\"Name\" dbType=\"varchar\" clrType=\"System.String\" /><Column name=\"Price\" dbType=\"decimal\" clrType=\"System.Decimal\" /><Column name=\"Expiry\" dbType=\"date\" clrType=\"System.DateTime\" /></Columns></Schema><Rows><Row><Id>12</Id><Name>NVC001</Name><Price>12.6</Price><Expiry>2009-12-25T11:48:33.983</Expiry></Row><Row><Id>12</Id><Name>NVC001</Name><Price>12.6</Price></Row></Rows></ResultSet><ResultSet><Schema><Columns><Column name=\"Cola\" dbType=\"int\" clrType=\"System.Int32\" /><Column name=\"Colb\" dbType=\"varchar\" clrType=\"System.String\" /></Columns></Schema><Rows><Row><Cola>19</Cola></Row></Rows></ResultSet></ResultSets></ActionResult>";

            var xml = SerializerHelper.SerializeObject(actionResult);

            Assert.AreEqual(expectedXml, xml);
        }

        [TestMethod]
        public void CanDeserializeActionResultUsingXmlSerializer()
        {
            var dt1 = new DateTime(2000, 1, 15, 16, 33, 12).AddMilliseconds(788);
            var dt2 = new DateTime(2001, 2, 18, 7, 59, 55).AddMilliseconds(250);

            var sb = new StringBuilder();
            sb.Append("<ActionResult xmlns=\"http://tempuri.org/ActionResult.xsd\">");
            sb.Append("<ResultSets>");
            sb.Append("<ResultSet>");
            sb.Append("<Schema>");
            sb.Append("<Columns>");
            sb.Append("<Column name=\"Id\" dbType=\"int\" clrType=\"System.Int32\" />");
            sb.Append("<Column name=\"Name\" dbType=\"varchar\" clrType=\"System.String\" />");
            sb.Append("<Column name=\"Price\" dbType=\"decimal\" clrType=\"System.Decimal\" />");
            sb.Append("<Column name=\"Expiry\" dbType=\"date\" clrType=\"System.DateTime\" />");
            sb.Append("</Columns>");
            sb.Append("</Schema>");
            sb.Append("<Rows>");
            sb.Append("<Row>");
            sb.Append("<Id>12</Id>");
            sb.Append("<Name>NVC001</Name>");
            sb.Append("<Price>12.6</Price>");
            sb.Append("<Expiry>" + XmlConvert.ToString(dt1, XmlDateTimeSerializationMode.Utc) + "</Expiry>");
            sb.Append("</Row>");
            sb.Append("<Row>");
            sb.Append("<Id>15</Id>");
            sb.Append("<Name>NVC002</Name>");
            sb.Append("<Price>12.8</Price>");
            sb.Append("<Expiry>" + XmlConvert.ToString(dt2, XmlDateTimeSerializationMode.Utc) + "</Expiry>");
            sb.Append("</Row>");
            sb.Append("</Rows>");
            sb.Append("</ResultSet>");

            sb.Append("<ResultSet>");
            sb.Append("<Schema>");
            sb.Append("<Columns>");
            sb.Append("<Column name=\"Cola\" dbType=\"int\" clrType=\"System.Int32\" />");
            sb.Append("<Column name=\"Colb\" dbType=\"varchar\" clrType=\"System.String\" />");
            sb.Append("</Columns>");
            sb.Append("</Schema>");
            sb.Append("<Rows>");
            sb.Append("<Row>");
            sb.Append("<Cola>22</Cola>");
            sb.Append("</Row>");
            sb.Append("</Rows>");
            sb.Append("</ResultSet>");

            sb.Append("</ResultSets>");
            sb.Append("</ActionResult>");


            var xml = sb.ToString();//

            var ar = SerializerHelper.Deserialize<ActionResult>(xml);

            Assert.IsNotNull(ar.ResultSets);
            Assert.AreEqual(2, ar.ResultSets.Count);

            // 1st resultset
            var rs1 = ar.ResultSets[0];
            Assert.IsNotNull(rs1.Rows);
            Assert.AreEqual(2, rs1.Rows.Count);
            Assert.IsNotNull(rs1);
            Assert.IsNotNull(rs1.Schema);
            Assert.IsNotNull(rs1.Schema.Columns);
            Assert.AreEqual(4, rs1.Schema.Columns.Count);

            Assert.AreEqual("Id", rs1.Schema.Columns[0].Name);
            Assert.AreEqual("int", rs1.Schema.Columns[0].DbType);
            Assert.AreSame(typeof(int), rs1.Schema.Columns[0].ClrType);

            Assert.AreEqual("Name", rs1.Schema.Columns[1].Name);
            Assert.AreEqual("varchar", rs1.Schema.Columns[1].DbType);
            Assert.AreSame(typeof(string), rs1.Schema.Columns[1].ClrType);

            Assert.AreEqual("Price", rs1.Schema.Columns[2].Name);
            Assert.AreEqual("decimal", rs1.Schema.Columns[2].DbType);
            Assert.AreSame(typeof(decimal), rs1.Schema.Columns[2].ClrType);

            Assert.AreEqual("Expiry", rs1.Schema.Columns[3].Name);
            Assert.AreEqual("date", rs1.Schema.Columns[3].DbType);
            Assert.AreSame(typeof(DateTime), rs1.Schema.Columns[3].ClrType);

            Assert.AreEqual(2, rs1.Rows.Count);
            Assert.IsNotNull(rs1.Rows[0]);
            Assert.AreEqual(4, rs1.Rows[0].Count);
            Assert.AreEqual(12, rs1.Rows[0]["Id"]);
            Assert.AreSame(typeof(int), rs1.Rows[0]["Id"].GetType());
            Assert.AreEqual("NVC001", rs1.Rows[0]["Name"]);
            Assert.AreSame(typeof(string), rs1.Rows[0]["Name"].GetType());
            Assert.AreEqual(12.6m, rs1.Rows[0]["Price"]);
            Assert.AreSame(typeof(decimal), rs1.Rows[0]["Price"].GetType());
            Assert.AreEqual(dt1, rs1.Rows[0]["Expiry"]);
            Assert.AreSame(typeof(DateTime), rs1.Rows[0]["Expiry"].GetType());

            Assert.IsNotNull(rs1.Rows[1]);
            Assert.AreEqual(4, rs1.Rows[1].Count);
            Assert.AreEqual(15, rs1.Rows[1]["Id"]);
            Assert.AreSame(typeof(int), rs1.Rows[1]["Id"].GetType());
            Assert.AreEqual("NVC002", rs1.Rows[1]["Name"]);
            Assert.AreSame(typeof(string), rs1.Rows[1]["Name"].GetType());
            Assert.AreEqual(12.8m, rs1.Rows[1]["Price"]);
            Assert.AreSame(typeof(decimal), rs1.Rows[1]["Price"].GetType());
            Assert.AreEqual(dt2, rs1.Rows[1]["Expiry"]);
            Assert.AreSame(typeof(DateTime), rs1.Rows[1]["Expiry"].GetType());


            // 2nd resultset
            var rs2 = ar.ResultSets[1];
            Assert.IsNotNull(rs2);
            Assert.IsNotNull(rs2.Schema);
            Assert.IsNotNull(rs2.Schema.Columns);
            Assert.AreEqual(2, rs2.Schema.Columns.Count);

            Assert.AreEqual("Cola", rs2.Schema.Columns[0].Name);
            Assert.AreEqual("int", rs2.Schema.Columns[0].DbType);
            Assert.AreSame(typeof(int), rs2.Schema.Columns[0].ClrType);

            Assert.AreEqual("Colb", rs2.Schema.Columns[1].Name);
            Assert.AreEqual("varchar", rs2.Schema.Columns[1].DbType);
            Assert.AreSame(typeof(string), rs2.Schema.Columns[1].ClrType);

            Assert.IsNotNull(rs2.Rows[0]);
            Assert.AreEqual(2, rs2.Rows[0].Count);
            Assert.AreEqual(22, rs2.Rows[0]["Cola"]);
            Assert.AreSame(typeof(int), rs2.Rows[0]["Cola"].GetType());
            Assert.AreEqual(DBNull.Value, rs2.Rows[0]["Colb"]);
            Assert.AreSame(DBNull.Value.GetType(), rs2.Rows[0]["Colb"].GetType());
        }

        [TestMethod]
        public void CanSerializeAndDeserialize()
        {
            var dt = new DateTime(2009, 12, 25, 11, 48, 33).AddMilliseconds(983);

            var actionResult = new ActionResult();

            var rs1_ = new ResultSet();
            actionResult.ResultSets.Add(rs1_);
            rs1_.Schema.Columns.Add(new Column { Name = "Id", DbType = "int", ClrType = typeof(int) });
            rs1_.Schema.Columns.Add(new Column { Name = "Name", DbType = "varchar", ClrType = typeof(string) });
            rs1_.Schema.Columns.Add(new Column { Name = "Price", DbType = "decimal", ClrType = typeof(decimal) });
            rs1_.Schema.Columns.Add(new Column { Name = "Expiry", DbType = "date", ClrType = typeof(DateTime) });

            rs1_.Rows.Add(new ResultSetRow());
            rs1_.Rows[0]["Id"] = 12;
            rs1_.Rows[0]["Name"] = "NVC001";
            rs1_.Rows[0]["Price"] = 12.6m;
            rs1_.Rows[0]["Expiry"] = dt;

            rs1_.Rows.Add(new ResultSetRow());
            rs1_.Rows[1]["Id"] = 12;
            rs1_.Rows[1]["Name"] = "NVC001";
            rs1_.Rows[1]["Price"] = 12.6m;
            rs1_.Rows[1]["Expiry"] = null;

            // 2nd resultset
            var rs2_ = new ResultSet();
            actionResult.ResultSets.Add(rs2_);
            rs2_.Schema.Columns.Add(new Column { Name = "Cola", DbType = "int", ClrType = typeof(int) });
            rs2_.Schema.Columns.Add(new Column { Name = "Colb", DbType = "varchar", ClrType = typeof(string) });

            rs2_.Rows.Add(new ResultSetRow());
            rs2_.Rows[0]["Cola"] = 19;
            rs2_.Rows[0]["Colb"] = null;

            var xml = SerializerHelper.SerializeObject(actionResult);

            var ar = SerializerHelper.Deserialize<ActionResult>(xml);
            Assert.IsNotNull(ar);
            Assert.IsNotNull(ar.ResultSets);
            Assert.AreEqual(2, ar.ResultSets.Count);

            Assert.IsNotNull(ar.ResultSets[0]);
            Assert.IsNotNull(ar.ResultSets[0].Schema);
            Assert.IsNotNull(ar.ResultSets[0].Schema.Columns);
            Assert.AreEqual(4, ar.ResultSets[0].Schema.Columns.Count);
            Assert.IsNotNull(ar.ResultSets[0].Rows);
            Assert.AreEqual(2, ar.ResultSets[0].Rows.Count);

            Assert.IsNotNull(ar.ResultSets[1]);
            Assert.IsNotNull(ar.ResultSets[1].Schema);
            Assert.IsNotNull(ar.ResultSets[1].Schema.Columns);
            Assert.AreEqual(2, ar.ResultSets[1].Schema.Columns.Count);
            Assert.IsNotNull(ar.ResultSets[1].Rows);
            Assert.AreEqual(1, ar.ResultSets[1].Rows.Count);
        }

        #endregion
    }
}
