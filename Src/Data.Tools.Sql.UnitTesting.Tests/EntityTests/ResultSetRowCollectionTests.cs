using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting;
using Data.Tools.UnitTesting.Tests.Utils;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Tests.EntityTests
{
    [TestClass]
    public class ResultSetRowCollectionTests
    {
        [TestMethod]
        public void ValidateThrowsWithoutSchema()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ResultSetRowCollection().Validate(null);
            }, "schema");
        }

        [TestMethod]
        public void CanValidateWithoutNoRecursiveCheck()
        {
            var schema = new ResultSetSchema();
            var rows = new ResultSetRowCollection();

            rows.Add(new ResultSetRow());
            rows[0]["Name"] = "will";

            Assert.IsNull(rows.Validate(schema));
        }

        [TestMethod]
        public void ValidationFailsWithRecursiveCheckAndInvalidRow()
        {
            var schema = new ResultSetSchema();
            var rows = new ResultSetRowCollection();

            rows.Add(new ResultSetRow());
            rows[0]["Name"] = "will";

            var ex = rows.Validate(schema, true);
            Assert.IsNotNull(ex);
            Assert.AreSame(typeof(InvalidOperationException), ex.GetType());
            Assert.AreEqual("Found invalid row(s)", ex.Message);
            Assert.IsNotNull(ex.InnerException);
        }

        [TestMethod]
        public void ValidationSucceedsWithRecursiveCheckAndValidRows()
        {
            var schema = new ResultSetSchema();
            schema.Columns.Add(new Column { Name = "Name", ClrType = typeof(string), DbType = "varchar" });
            var rows = new ResultSetRowCollection();

            rows.Add(new ResultSetRow());
            rows[0]["Name"] = "will";

            Assert.IsNull(rows.Validate(schema, true));
        }

        [TestMethod]
        public void CreateFromReaderThrowsWithNullReader()
        {
            Assert.ThrowsException<ArgumentNullException>(() => { ResultSetRowCollection.CreateFromReader(null); }, "reader");
        }

        [TestMethod]
        public void CanCreateFromReaderWith0Rows()
        {
            var r = new TestDataReader();
            var rs = new ResultSet();
            r.ResultSets.Add(rs);

            rs.Schema.Columns.Add(new Column { Name = "cola", ClrType = typeof(string), DbType = "varchar" });

            var readRs = ResultSetRowCollection.CreateFromReader(r);
            Assert.IsNotNull(readRs);
            Assert.AreEqual(0, readRs.Count);
        }

        [TestMethod]
        public void CancreateFromReaderWith2Rows()
        {
            var r = new TestDataReader();
            var rs = new ResultSet();
            r.ResultSets.Add(rs);

            rs.Schema.Columns.Add(new Column { Name = "cola", ClrType = typeof(string), DbType = "varchar" });
            rs.Rows.Add(new ResultSetRow());
            rs.Rows.Add(new ResultSetRow());

            rs.Rows[0]["cola"] = "row1";
            rs.Rows[1]["cola"] = "row2";

            var readRs = ResultSetRowCollection.CreateFromReader(r);
            Assert.IsNotNull(readRs);
            Assert.AreEqual(2, readRs.Count);
        }
    }
}
