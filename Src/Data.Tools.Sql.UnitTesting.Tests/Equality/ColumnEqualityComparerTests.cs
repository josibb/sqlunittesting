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
    public class ColumnEqualityComparerTests
    {
        [TestMethod]
        public void CanEquateColumns()
        {
            Assert.IsTrue(
                new Column().EqualsColumn(
                    new Column()));

            // name
            Assert.IsTrue(
                new Column { Name = "cola" }.EqualsColumn(
                    new Column { Name = "cola" }));

            Assert.IsFalse(
                new Column { Name = "cola" }.EqualsColumn(
                    new Column { Name = "colb" }));

            Assert.IsFalse(
                new Column { Name = null }.EqualsColumn(
                    new Column { Name = "colb" }));

            Assert.IsFalse(
                new Column { Name = "cola" }.EqualsColumn(
                    new Column { Name = "null" }));

            // DbType
            Assert.IsTrue(
               new Column { DbType = "varchar" }.EqualsColumn(
                   new Column { DbType = "varchar" }));

            Assert.IsFalse(
                new Column { DbType = "varchar" }.EqualsColumn(
                    new Column { DbType = "int" }));

            Assert.IsFalse(
                new Column { DbType = null }.EqualsColumn(
                    new Column { DbType = "varchar" }));

            Assert.IsFalse(
                new Column { DbType = "varchar" }.EqualsColumn(
                    new Column { DbType = "null" }));

            // Type
            Assert.IsTrue(
               new Column { DbType = "varchar" }.EqualsColumn(
                   new Column { DbType = "varchar" }));

            Assert.IsFalse(
                new Column { ClrType = typeof(int) }.EqualsColumn(
                    new Column { ClrType = typeof(string)}));

            Assert.IsFalse(
                new Column { ClrType = null }.EqualsColumn(
                    new Column { ClrType = typeof(int) }));

            Assert.IsFalse(
                new Column { ClrType = typeof(int) }.EqualsColumn(
                    new Column { ClrType = null }));

            // combine
            Assert.IsTrue(
               new Column { Name = "cola", ClrType = typeof(int), DbType = "varchar" }.EqualsColumn(
                   new Column { Name = "cola", ClrType = typeof(int), DbType = "varchar" }));

            Assert.IsFalse(
               new Column { Name = "cola", ClrType = typeof(int), DbType = "varchar" }.EqualsColumn(
                   new Column { Name = "colb", ClrType = typeof(int), DbType = "varchar" }));

            Assert.IsFalse(
               new Column { Name = "cola", ClrType = typeof(int), DbType = "varchar" }.EqualsColumn(
                   new Column { Name = "cola", ClrType = typeof(DateTime), DbType = "varchar" }));

            Assert.IsFalse(
               new Column { Name = "cola", ClrType = typeof(int), DbType = "varchar" }.EqualsColumn(
                   new Column { Name = "cola", ClrType = typeof(int), DbType = "int" }));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EqualsColumnsThrowsIfParameterIsNull()
        {
            new Column().EqualsColumn(null);
        }
    }
}
