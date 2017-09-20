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
    public class RowCollectionEqualityComparerTests
    {
        [TestMethod]
        public void CanEquateRowValues()
        {
            var rows1 = new ResultSetRowCollection();
            var rows2 = new ResultSetRowCollection();

            Assert.IsTrue(rows1.EqualRowCollections(rows2));

            rows1.Add(new ResultSetRow());
            rows2.Add(new ResultSetRow());
            Assert.IsTrue(rows1.EqualRowCollections(rows2));

            rows1[0]["cola"] = 99;
            Assert.IsFalse(rows1.EqualRowCollections(rows2));
            rows2[0]["cola"] = 99;
            Assert.IsTrue(rows1.EqualRowCollections(rows2));

            rows1.Add(new ResultSetRow());
            rows2.Add(new ResultSetRow());
            Assert.IsTrue(rows1.EqualRowCollections(rows2));

            rows1[1]["abc"] = "hi";
            Assert.IsFalse(rows1.EqualRowCollections(rows2));
            rows2[1]["abc"] = "hi";
            Assert.IsTrue(rows1.EqualRowCollections(rows2));

            rows1.Add(new ResultSetRow());
            rows2.Add(new ResultSetRow());
            rows1[1]["test"] = "klm";
            rows2[2]["test2"] = "klm";
            Assert.IsFalse(rows1.EqualRowCollections(rows2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EquateToNullThrows()
        {
            new ResultSetRowCollection().EqualRowCollections(null);
        }
    }
}
