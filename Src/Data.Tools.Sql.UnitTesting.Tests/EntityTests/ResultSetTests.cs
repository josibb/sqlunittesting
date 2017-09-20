using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using Data.Tools.UnitTesting.Tests.Utils;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Tests
{
    [TestClass]
    public class ResultSetTests
    {
        [TestMethod]
        public void IsSchemaInitialized()
        {
            var r = new ResultSet();
            Assert.IsNotNull(r.Schema);
        }

        [TestMethod]
        public void IsRowsInitialized()
        {
            var r = new ResultSet();
            Assert.IsNotNull(r.Rows);
        }

        [TestMethod]
        public void CanValidateResultSet()
        {
            var rs = new ResultSet();

            // valid resultset (non recursive)
            Assert.IsNull(rs.Validate(false));
            Assert.IsNull(rs.Validate());

            // no schema --> nonrecusrive --> invalid, recursive --> invalid
            rs = new ResultSet { Schema = null };
            Assert.IsNotNull(rs.Validate(false));
            Assert.IsNotNull(rs.Validate());
            Assert.IsNotNull(rs.Validate(true));

            // no rows --> nonrecusrive --> invalid, recursive --> invalid
            rs = new ResultSet { Rows = null };
            Assert.IsNotNull(rs.Validate(false));
            Assert.IsNotNull(rs.Validate());
            Assert.IsNotNull(rs.Validate(true));

            // invalid schema --> nonrecursive --> valid, recursive --> invalid
            rs = new ResultSet();
            rs.Schema.Columns = null; // makes schema invalid
            Assert.IsNull(rs.Validate(false));
            Assert.IsNull(rs.Validate());
            var ex = rs.Validate(true);
            Assert.IsNotNull(ex);
            Assert.AreSame(typeof(InvalidOperationException), ex.GetType());
            Assert.IsNotNull(ex.InnerException);

            // invalid rows --> nonrecursve --> valid, recursive --> invalid
            rs = new ResultSet();
            rs.Rows.Add(new ResultSetRow());
            rs.Rows[0]["cola"] = "aa"; // nonexisting row makes rows invalid
            Assert.IsNull(rs.Validate(false));
            Assert.IsNull(rs.Validate());
            ex = rs.Validate(true);
            Assert.IsNotNull(ex);
            Assert.AreSame(typeof(InvalidOperationException), ex.GetType());
        }

        private IDataReader CreateTestReader()
        {
            var r = new TestDataReader();
            r.ResultSets.Add(new ResultSet());
            r.ResultSets[0].Schema.Columns.Add(new Column { Name = "cola", ClrType = typeof(string), DbType = "varchar" });
            r.ResultSets[0].Schema.Columns.Add(new Column { Name = "colb", ClrType = typeof(int), DbType = "int" });

            var row1 = new ResultSetRow();
            row1["cola"] = "a";
            row1["colb"] = 33;

            var row2 = new ResultSetRow();
            row2["cola"] = "aa";
            row2["colb"] = 3333;

            r.ResultSets[0].Rows.Add(row1);
            r.ResultSets[0].Rows.Add(row2);

            return r;
        }
        [TestMethod]
        public void CanCreateFromReaderWith2ColumnsAnd2Rows()
        {
            var resultSet = ResultSet.CreateFromReader(CreateTestReader());
            Assert.IsNotNull(resultSet);

            Assert.IsNotNull(resultSet.Schema);
            Assert.IsNotNull(resultSet.Schema.Columns);
            Assert.AreEqual(2, resultSet.Schema.Columns.Count);
            Assert.AreEqual("cola", resultSet.Schema.Columns[0].Name);
            Assert.AreEqual("varchar", resultSet.Schema.Columns[0].DbType);
            Assert.AreSame(typeof(string), resultSet.Schema.Columns[0].ClrType);
            Assert.AreEqual("colb", resultSet.Schema.Columns[1].Name);
            Assert.AreEqual("int", resultSet.Schema.Columns[1].DbType);
            Assert.AreSame(typeof(int), resultSet.Schema.Columns[1].ClrType);

            Assert.IsNotNull(resultSet.Rows);
            Assert.AreEqual(2, resultSet.Rows.Count);
            Assert.AreEqual("a", resultSet.Rows[0]["cola"]);
            Assert.AreEqual(33, resultSet.Rows[0]["colb"]);
            Assert.AreEqual("aa", resultSet.Rows[1]["cola"]);
            Assert.AreEqual(3333, resultSet.Rows[1]["colb"]);
        }

        [TestMethod]
        public void CreateFromReaderReturnsNullWithoutResultSet()
        {
            var r = new TestDataReader();

            var resultSet = ResultSet.CreateFromReader(r);
            Assert.IsNull(resultSet);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateFromReaderTHrowsIfReaderIsNull()
        {
            ResultSet.CreateFromReader(null);
        }
    }
}
