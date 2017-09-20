using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting;
using System.Collections.Generic;
using Data.Tools.UnitTesting.Equality;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Tests
{
    [TestClass]
    public class ResultSetEqualityComparerTests
    {
        [TestMethod]
        public void CanEquateResultSets()
        {
            var rs1 = new ResultSet();
            var rs2 = new ResultSet();

            Assert.IsTrue(rs1.EqualResultSet(rs2));

            // schema check?
            rs1.Schema.Columns.Add(new Column { Name = "cola", ClrType = typeof(int), DbType = "nvarchar" });
            Assert.IsFalse(rs1.EqualResultSet(rs2));
            rs2.Schema.Columns.Add(new Column { Name = "cola", ClrType = typeof(int), DbType = "nvarchar" });
            Assert.IsTrue(rs1.EqualResultSet(rs2));

            // rows check
            rs1.Rows.Add(new ResultSetRow());
            Assert.IsFalse(rs1.EqualResultSet(rs2));
            rs2.Rows.Add(new ResultSetRow());
            Assert.IsTrue(rs1.EqualResultSet(rs2));

            rs1.Rows[0]["cola"] = "hey";
            Assert.IsFalse(rs1.EqualResultSet(rs2));
            rs2.Rows[0]["cola"] = "abc";
            Assert.IsFalse(rs1.EqualResultSet(rs2));
            rs2.Rows[0]["cola"] = "hey";
            Assert.IsTrue(rs1.EqualResultSet(rs2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EqualResultSetThrowsIfParameter1IsNull()
        {
            new ResultSet().EqualResultSet(null);
        }

        [TestMethod]
        public void CanEquateRows()
        {
            var row1 = new ResultSetRow();
            var row2 = new ResultSetRow();

            Assert.IsTrue(row1.EqualRows(row2));

            row1["cola"] = 12;
            Assert.IsFalse(row1.EqualRows(row2));
            row2["cola"] = 11;
            Assert.IsFalse(row1.EqualRows(row2));
            row2["cola"] = 12;
            Assert.IsTrue(row1.EqualRows(row2));

            row1["colb"] = null;
            Assert.IsFalse(row1.EqualRows(row2));
            row2["colb"] = null;
            Assert.IsTrue(row1.EqualRows(row2));

            row1["cold"] = null;
            row2["cold"] = "abc";
            Assert.IsFalse(row1.EqualRows(row2));

            row1["cold"] = "def";
            row2["cold"] = null;
            Assert.IsFalse(row1.EqualRows(row2));
            row2["cold"] = "def";
            Assert.IsTrue(row1.EqualRows(row2));

            row1["colc"] = DBNull.Value;
            Assert.IsFalse(row1.EqualRows(row2));
            row2["colc"] = DBNull.Value;
            Assert.IsTrue(row1.EqualRows(row2));

            row2["colc"] = 12.6m;
            Assert.IsFalse(row1.EqualRows(row2));
            row1["colc"] = 14.6m;
            row2["colc"] = DBNull.Value;
            Assert.IsFalse(row1.EqualRows(row2));
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EquateRowsToNullThrows()
        {
            new ResultSetRow().EqualRows(null);
        }

    }
}
