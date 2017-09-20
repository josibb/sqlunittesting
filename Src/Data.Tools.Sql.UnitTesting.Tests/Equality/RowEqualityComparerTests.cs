using Data.Tools.UnitTesting;
using Data.Tools.UnitTesting.Equality;
using Data.Tools.UnitTesting.Result;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Tests.Equality
{
    [TestClass]
    public class RowEqualityComparerTests
    {
        [TestMethod]
        public void CanCompareRows()
        {
            var row1 = new ResultSetRow();
            row1["cola"] = "hello";
            row1["colb"] = DBNull.Value;
            row1["colc"] = null;
            row1["cold"] = DBNull.Value;

            var row2 = new ResultSetRow();
            row2["cola"] = "hello";
            row2["colb"] = DBNull.Value;
            row2["colc"] = null;
            row2["cold"] = null;


            Assert.IsTrue(row1.EqualRows(row2));
            Assert.IsTrue(row2.EqualRows(row1));

            row2["cold"] = "check";
            Assert.IsFalse(row1.EqualRows(row2));
            Assert.IsFalse(row2.EqualRows(row1));
        }

        [TestMethod]
        public void CanEqualValues()
        {
            Assert.IsTrue(RowEqualityComparer.EqualValues(null, null));
            Assert.IsTrue(RowEqualityComparer.EqualValues(DBNull.Value, DBNull.Value));
            Assert.IsTrue(RowEqualityComparer.EqualValues(DBNull.Value, null));
            Assert.IsTrue(RowEqualityComparer.EqualValues("ha", "ha"));
            Assert.IsTrue(RowEqualityComparer.EqualValues(12.6m, 12.6m));
            Assert.IsTrue(RowEqualityComparer.EqualValues(
                new DateTime(2009, 12, 25, 11, 48, 33).AddMilliseconds(983),
                new DateTime(2009, 12, 25, 11, 48, 33).AddMilliseconds(983)));
            Assert.IsFalse(RowEqualityComparer.EqualValues(null, "ha"));
            Assert.IsFalse(RowEqualityComparer.EqualValues("haaa", null));
            Assert.IsFalse(RowEqualityComparer.EqualValues(12.6m, 12.6));
            Assert.IsFalse(RowEqualityComparer.EqualValues("12.7", 12.7));
            Assert.IsFalse(RowEqualityComparer.EqualValues("15.4", 15.4m));
            Assert.IsFalse(RowEqualityComparer.EqualValues(12f, 12m));
            Assert.IsFalse(RowEqualityComparer.EqualValues(
                new DateTime(2009, 12, 25, 11, 48, 33).AddMilliseconds(983),
                new DateTime(2009, 12, 25, 11, 48, 33).AddMilliseconds(777)));
        }

        [TestMethod]
        public void RowsAreNotEqualWithDifferentColumnNames()
        {
            var row1 = new ResultSetRow();
            row1["la"] = "hello";
            row1["lb"] = DBNull.Value;
            row1["lc"] = null;
            row1["ld"] = DBNull.Value;

            var row2 = new ResultSetRow();
            row2["cola"] = "hello";
            row2["colb"] = DBNull.Value;
            row2["colc"] = null;
            row2["cold"] = null;

            Assert.IsFalse(row1.EqualRows(row2));
            Assert.IsFalse(row2.EqualRows(row1));
        }

        [TestMethod]
        public void CompareRowToNullThrowsException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ResultSetRow().EqualRows(null);
            }, "r2");
        }

        [TestMethod]
        public void CompareFromNullRowThrowsException()
        {
            ResultSetRow row = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                row.EqualRows(new ResultSetRow());
            }, "r2");
        }


        [TestMethod]
        public void RowsDoNotEqualWithDifferentCountOfColumns()
        {
            var row1 = new ResultSetRow();
            row1["cola"] = "hello";
            row1["colb"] = DBNull.Value;

            var row2 = new ResultSetRow();
            row2["cola"] = "hello";
            row2["colb"] = DBNull.Value;
            row2["colc"] = null;

            Assert.IsFalse(row1.EqualRows(row2));
            Assert.IsFalse(row2.EqualRows(row1));
        }
    }
}
