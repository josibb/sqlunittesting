using Data.Tools.UnitTesting.Result;
using Data.Tools.UnitTesting.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Data.Tools.UnitTesting.Tests
{
    [TestClass]
    public class ActionResultTests
    {
        [TestMethod]
        public void ConstructorThrowsWithNullSerializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ActionResult(null);
            }, "serializer");
        }

        [TestMethod]
        public void CanGetSchema()
        {
            Assert.IsNull(new ActionResult().GetSchema());
        }

        #region Validation tests..

        [TestMethod]
        public void CanValidate()
        {
            var ar = new ActionResult();

            Assert.IsNull(ar.Validate());
            Assert.IsNull(ar.Validate(false));
            Assert.IsNull(ar.Validate(true));

            ar.ResultSets.Add(new ResultSet());
            Assert.IsNull(ar.Validate());
            Assert.IsNull(ar.Validate(false));
            Assert.IsNull(ar.Validate(true));

            ar.ResultSets.Add(new ResultSet());
            Assert.IsNull(ar.Validate());
            Assert.IsNull(ar.Validate(false));
            Assert.IsNull(ar.Validate(true));

            // schema validation should fail
            ar.ResultSets[1].Schema.Columns = null;
            Assert.IsNull(ar.Validate());
            Assert.IsNull(ar.Validate(false));
            var ex = ar.Validate(true);
            Assert.IsNotNull(ex);
            Assert.AreSame(typeof(InvalidOperationException), ex.GetType());
            Assert.AreEqual("ResultSet validation failed", ex.Message);
            Assert.IsNotNull(ex.InnerException);

            // row collection validation should fail
            ar.ResultSets[1] = new ResultSet();
            ar.ResultSets[1].Rows = null;
            Assert.IsNull(ar.Validate());
            Assert.IsNull(ar.Validate(false));
            ex = ar.Validate(true);
            Assert.IsNotNull(ex);
            Assert.AreSame(typeof(InvalidOperationException), ex.GetType());
            Assert.AreEqual("ResultSet validation failed", ex.Message);
            Assert.IsNotNull(ex.InnerException);
        }

        [TestMethod]
        public void ValidationFailsIfResultSetsIsNull()
        {
            var ar = new ActionResult();
            ar.ResultSets = null;

            var ex1 = ar.Validate(false);
            Assert.IsNotNull(ex1);
            Assert.AreSame(typeof(InvalidOperationException), ex1.GetType());
            Assert.AreEqual("ResultSets is null", ex1.Message);

            var ex2 = ar.Validate();
            Assert.IsNotNull(ex2);
            Assert.AreSame(typeof(InvalidOperationException), ex2.GetType());
            Assert.AreEqual("ResultSets is null", ex2.Message);
        }

        #endregion

        #region Serialization

        [TestMethod]
        public void CanSerializerToString()
        {
            string xml = new ActionResult().Serialize();
            Assert.IsNotNull(xml);
            Assert.IsTrue(xml.StartsWith("<?xml"));
            Assert.IsTrue(xml.Contains("ActionResult xmlns=\"http://tempuri.org/ActionResult.xsd\""));
        }

        [TestMethod]
        public void CanSerializeToStream()
        {
            using (var ms = new MemoryStream())
            {
                new ActionResult().Serialize(ms);

                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                Assert.IsTrue(ms.Length > 0);
            }
        }

        [TestMethod]
        public void SerializeToStreamThrowsIfStreamIsNull()
        {
            Stream s = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ActionResult().Serialize(s);
            }, "stream");
        }

        [TestMethod]
        public void CanSerializerToFile()
        {
            var fileName = $"CanSerializerToFile";
            var guid = Guid.NewGuid();
            while (File.Exists($"{fileName}-{guid.ToString()}.xml"))
            {
                guid = Guid.NewGuid();
            }
            fileName = $"{fileName}-{guid.ToString()}.xml";
            try
            {
                new ActionResult().Serialize(fileName);

                Assert.IsTrue(File.Exists(fileName));

                var xml = File.ReadAllText(fileName);
                Assert.IsNotNull(xml);
                Assert.IsTrue(xml.StartsWith("<?xml"));
                Assert.IsTrue(xml.Contains("ActionResult xmlns=\"http://tempuri.org/ActionResult.xsd\""));
            }
            finally
            {
                File.Delete(fileName);
            }

        }

        [TestMethod]
        public void SerializeToFileThrowsIfFileIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                string fileName = null;
                new ActionResult().Serialize(fileName);
            }, "fileName");
        }

        [TestMethod]
        public void CanDeserializeFromStream()
        {
            using (var s = ResourceHelper.GetResource("CanDeserializeFromStream_ActionResult.xml"))
            {
                var ar = ActionResult.Deserialize(s);

                Assert.IsNotNull(ar);
                Assert.IsNotNull(ar.ResultSets);
                Assert.AreEqual(1, ar.ResultSets.Count);
                Assert.IsNotNull(ar.ResultSets[0]);
                Assert.IsNotNull(ar.ResultSets[0].Schema);
                Assert.IsNotNull(ar.ResultSets[0].Rows);
                Assert.AreEqual(2, ar.ResultSets[0].Rows.Count);
            }
        }

        [TestMethod]
        public void CannotDeserializeFromStreamWithIncorrectSchema()
        {
            using (var s = ResourceHelper.GetResource("CanDeserializeFromStream_ActionResult_IncorrectSchema.xml"))
            {
                try

                {
                    ActionResult.Deserialize(s);
                }
                catch (Exception ex)
                {
                    Assert.AreSame(typeof(InvalidOperationException), ex.GetType());
                    Assert.IsNotNull(ex.InnerException);
                    Assert.AreSame(typeof(XmlSchemaValidationException), ex.InnerException.GetType());
                    return;
                }

                Assert.Fail("Expected exception was not thrown");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CannotDeserializeFromNullStream()
        {
            ActionResult.Deserialize(null);
        }


        #endregion

        #region Create from reader tests..

        [TestMethod]
        public void CreateFromReaderThrowsWithNullReader()
        {
            Assert.ThrowsException<ArgumentNullException>(() => { ActionResult.CreateFromReader(null); }, "reader");
        }

        [TestMethod]
        public void CanCreateFromReaderWithoutResultSets()
        {
            var dr = new TestDataReader();

            var ar = ActionResult.CreateFromReader(dr);
            Assert.IsNotNull(ar);
            Assert.IsNotNull(ar.ResultSets);
            Assert.AreEqual(0, ar.ResultSets.Count);
        }

        [TestMethod]
        public void CanCreateFromReaderFor1ResultSet()
        {
            var dr = new TestDataReader();
            var rs = new ResultSet();
            dr.ResultSets.Add(rs);

            rs.Schema.Columns.Add(new Column { Name = "cola", DbType = "int", ClrType = typeof(int) });
            rs.Schema.Columns.Add(new Column { Name = "colb", DbType = "varchar", ClrType = typeof(string) });

            var row1 = new ResultSetRow();
            row1["cola"] = 15;
            row1["colb"] = "test";
            rs.Rows.Add(row1);

            var row2 = new ResultSetRow();
            row2["cola"] = 19;
            row2["colb"] = "testing";
            rs.Rows.Add(row2);

            var ar = ActionResult.CreateFromReader(dr);
            Assert.IsNotNull(ar);
            Assert.IsNotNull(ar.ResultSets);
            Assert.AreEqual(1, ar.ResultSets.Count);
            Assert.IsNotNull(ar.ResultSets[0]);
            Assert.IsNotNull(ar.ResultSets[0].Schema);
            Assert.IsNotNull(ar.ResultSets[0].Schema.Columns);
            Assert.AreEqual(2, ar.ResultSets[0].Schema.Columns.Count);
            Assert.IsNotNull(ar.ResultSets[0].Rows);
            Assert.AreEqual(2, ar.ResultSets[0].Rows.Count);
        }

        [TestMethod]
        public void CanCreateFromReaderFor2ResultSets()
        {
            var dr = new TestDataReader();

            // resultset 1
            var rs1 = new ResultSet();
            dr.ResultSets.Add(rs1);

            rs1.Schema.Columns.Add(new Column { Name = "cola", DbType = "int", ClrType = typeof(int) });
            rs1.Schema.Columns.Add(new Column { Name = "colb", DbType = "varchar", ClrType = typeof(string) });

            var row1 = new ResultSetRow();
            row1["cola"] = 15;
            row1["colb"] = "test";
            rs1.Rows.Add(row1);

            var row2 = new ResultSetRow();
            row2["cola"] = 19;
            row2["colb"] = "testing";
            rs1.Rows.Add(row2);

            // resultset 2
            var rs2 = new ResultSet();
            dr.ResultSets.Add(rs2);

            rs2.Schema.Columns.Add(new Column { Name = "cola", DbType = "int", ClrType = typeof(int) });
            rs2.Schema.Columns.Add(new Column { Name = "colb", DbType = "varchar", ClrType = typeof(string) });

            row1 = new ResultSetRow();
            row1["cola"] = 15;
            row1["colb"] = "test";
            rs2.Rows.Add(row1);

            row2 = new ResultSetRow();
            row2["cola"] = 19;
            row2["colb"] = "testing";
            rs2.Rows.Add(row2);

            var ar = ActionResult.CreateFromReader(dr);
            Assert.IsNotNull(ar);
            Assert.IsNotNull(ar.ResultSets);
            Assert.AreEqual(2, ar.ResultSets.Count);

            Assert.IsNotNull(ar.ResultSets[0]);
            Assert.IsNotNull(ar.ResultSets[0].Schema);
            Assert.IsNotNull(ar.ResultSets[0].Schema.Columns);
            Assert.AreEqual(2, ar.ResultSets[0].Schema.Columns.Count);
            Assert.IsNotNull(ar.ResultSets[0].Rows);
            Assert.AreEqual(2, ar.ResultSets[0].Rows.Count);

            Assert.IsNotNull(ar.ResultSets[1]);
            Assert.IsNotNull(ar.ResultSets[1].Schema);
            Assert.IsNotNull(ar.ResultSets[1].Schema.Columns);
            Assert.AreEqual(2, ar.ResultSets[1].Schema.Columns.Count);
            Assert.IsNotNull(ar.ResultSets[1].Rows);
            Assert.AreEqual(2, ar.ResultSets[1].Rows.Count);
        }

        #endregion
    }
}
