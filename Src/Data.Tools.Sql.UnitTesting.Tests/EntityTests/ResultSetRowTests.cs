using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting;
using Data.Tools.UnitTesting.Tests.Utils;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Tests.EntityTests
{
    [TestClass]
    public class ResultSetRowTests
    {
        [TestMethod]
        public void CreateFromReaderThrowsWithNullReader()
        {
            Assert.ThrowsException<ArgumentNullException>(() => { ResultSetRow.CreateFromReader(null); }, "reader");
        }

        [TestMethod]
        public void CanCreateFromReader()
        {
            var r = new TestDataReader();
            var rs = new ResultSet();
            r.ResultSets.Add(rs);

            rs.Schema.Columns.Add(new Column { Name = "cola", ClrType = typeof(string), DbType = "varchar" });
            rs.Schema.Columns.Add(new Column { Name = "colb", ClrType = typeof(int), DbType = "int" });

            var row = new ResultSetRow();
            row["cola"] = "will";
            row["colb"] = 33;
            rs.Rows.Add(row);

            Assert.IsTrue(r.Read()); // check that read works
            var readerRow = ResultSetRow.CreateFromReader(r);
            Assert.IsNotNull(readerRow);
            Assert.AreEqual(2, readerRow.Count);
            Assert.AreEqual("will", readerRow["cola"]);
            Assert.AreEqual(33, readerRow["colb"]);
        }


        [TestMethod]
        public void ValidationThrowsWithoutSchema()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ResultSetRow().Validate(null);
            }, "schema");
        }

        [TestMethod]
        public void ValidationFailsWithRowNameThatIsNotInSchema()
        {
            var schema = new ResultSetSchema();
            schema.Columns.Add(new Column { Name = "Age", ClrType = typeof(int), DbType = "varchar" });
            var row = new ResultSetRow();
            row["Name"] = "smith";
            row["Age"] = 12;

            var ex = row.Validate(schema);
            Assert.IsNotNull(ex);
            Assert.AreSame(typeof(InvalidOperationException), ex.GetType());
            Assert.AreEqual("Column 'Name' is not defined in schema", ex.Message);
        }

        [TestMethod]
        public void ValidationFailsWithInvalidSchema()
        {
            var schema = new ResultSetSchema();
            schema.Columns.Add(new Column { Name = null, ClrType = typeof(string), DbType = "varchar" });

            var row = new ResultSetRow();
            row["Name"] = "smith";

            var ex = row.Validate(schema);
            Assert.IsNotNull(ex);
            Assert.AreSame(typeof(InvalidOperationException), ex.GetType());
            Assert.AreEqual("Invalid schema", ex.Message);
            Assert.IsNotNull(ex.InnerException);
        }

        [TestMethod]
        public void ValiationFailsWithEmptyRowName()
        {
            var schema = new ResultSetSchema();

            var row = new ResultSetRow();
            row[" "] = "smith"; // defines use of string.isnulloremptywhitespace

            var ex = row.Validate(schema);
            Assert.IsNotNull(ex);
            Assert.AreSame(typeof(InvalidOperationException), ex.GetType());
            Assert.AreEqual("Row contains a value without column name (null/empty)", ex.Message);
        }

        [TestMethod]
        public void ValidationFailsIfRowValueDiffersFromClrTypeDefinedInSchema()
        {
            var schema = new ResultSetSchema();
            schema.Columns.Add(new Column { Name = "Name", ClrType = typeof(string), DbType = "varchar" });
            schema.Columns.Add(new Column { Name = "Age", ClrType = typeof(int), DbType = "int" });

            var row = new ResultSetRow();
            row["Name"] = "smith";
            row["Age"] = "test";

            var ex = row.Validate(schema);
            Assert.IsNotNull(ex);
            Assert.AreSame(typeof(InvalidOperationException), ex.GetType());
            Assert.AreEqual("Value for 'Age' is of type 'System.String' while schema defines type 'System.Int32'", ex.Message);
        }

        [TestMethod]
        public void CanValidate()
        {
            var schema = new ResultSetSchema();
            schema.Columns.Add(new Column { Name = "Name", ClrType = typeof(string), DbType = "varchar" });
            schema.Columns.Add(new Column { Name = "Age", ClrType = typeof(int), DbType = "int" });

            var row = new ResultSetRow();
            row["Name"] = "smith";
            row["Age"] = 81;

            Assert.IsNull(row.Validate(schema));
        }
    }
}
