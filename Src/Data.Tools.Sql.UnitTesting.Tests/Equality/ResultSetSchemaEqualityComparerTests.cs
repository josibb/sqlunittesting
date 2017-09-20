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
    public class ResultSetSchemaEqualityComparerTests
    {
        [TestMethod]
        public void CanEquateResultSetSchema()
        {
            var s1 = new ResultSetSchema();
            var s2 = new ResultSetSchema();

            Assert.IsTrue(s1.EqualsSchema(s2));

            s1.Columns.Add(new Column { Name = "cola", ClrType = typeof(string), DbType = "" });
            Assert.IsFalse(s1.EqualsSchema(s2));
            s2.Columns.Add(new Column { Name = "cola", ClrType = typeof(string), DbType = "" });
            Assert.IsTrue(s1.EqualsSchema(s2));

            s1.Columns.Add(new Column { Name = "colb", ClrType = typeof(string), DbType = "" });
            Assert.IsFalse(s1.EqualsSchema(s2));
            s2.Columns.Add(new Column { Name = "colabc", ClrType = typeof(string), DbType = "" });
            Assert.IsFalse(s1.EqualsSchema(s2));
        }



        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EqualsSchemaTHrowsIfParameter0IsNull()
        {
            new ResultSetSchema().EqualsSchema(null);
        }
    }
}
