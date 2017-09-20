using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting.Equality;
using Data.Tools.UnitTesting;
using System.Collections.Generic;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Tests.Equality
{
    [TestClass]
    public class ActionResultEqualityComparerTests
    {
        [TestMethod]
        public void CanEquateActionResults()
        {
            var ac1 = new ActionResult();
            var ac2 = new ActionResult();

            Assert.IsTrue(ac1.EqualsActionResult(ac2));

            ac1.ResultSets.Add(new ResultSet());
            Assert.IsFalse(ac1.EqualsActionResult(ac2));
            ac2.ResultSets.Add(new ResultSet());
            Assert.IsTrue(ac1.EqualsActionResult(ac2));

            ac1.ResultSets.Add(new ResultSet());
            Assert.IsFalse(ac1.EqualsActionResult(ac2));
            ac2.ResultSets.Add(new ResultSet());
            Assert.IsTrue(ac1.EqualsActionResult(ac2));

            // schema
            ac1.ResultSets[0].Schema.Columns.Add(new Column());
            Assert.IsFalse(ac1.EqualsActionResult(ac2));
            ac2.ResultSets[0].Schema.Columns.Add(new Column());
            Assert.IsTrue(ac1.EqualsActionResult(ac2));

            // rows
            ac1.ResultSets[0].Rows.Add(new ResultSetRow());
            Assert.IsFalse(ac1.EqualsActionResult(ac2));
            ac2.ResultSets[0].Rows.Add(new ResultSetRow());
            Assert.IsTrue(ac1.EqualsActionResult(ac2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EquateActionResultToNullThrowsException()
        {
            new ActionResult().EqualsActionResult(null);
        }
    }
}
