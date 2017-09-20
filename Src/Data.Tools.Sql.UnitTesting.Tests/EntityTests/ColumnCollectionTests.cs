using Data.Tools.UnitTesting;
using Data.Tools.UnitTesting.Result;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Tests.EntityTests
{
    [TestClass]
    public class ColumnCollectionTests
    {
        [TestMethod]
        public void CanValidate()
        {
            var cc = new ColumnCollection();

            Assert.IsNull(cc.Validate(true));
            Assert.IsNull(cc.Validate(false));

            // add invalid column
            cc.Add(new Column { ClrType = typeof(string), DbType="nvarchar" });
            Assert.IsNull(cc.Validate());
            Assert.IsNotNull(cc.Validate(true));
        }
    }
}
