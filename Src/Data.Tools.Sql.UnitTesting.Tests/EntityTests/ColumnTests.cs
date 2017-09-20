using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using Data.Tools.UnitTesting;
using System.IO;
using Data.Tools.UnitTesting.Tests.Utils;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Tests
{
    [TestClass]
    public class ColumnTests
    {
        [TestMethod]
        public void CanCreateFromReader()
        {
            var r = new TestDataReader();
            r.ResultSets.Add(new ResultSet());
            r.ResultSets[0].Schema.Columns.Add(new Column { Name = "cola", ClrType = typeof(string), DbType = "varchar" });
            r.ResultSets[0].Schema.Columns.Add(new Column { Name = "colb", ClrType = typeof(int), DbType = "int" });

            var c1 = Column.CreateFromReader(r, 0);
            var c2 = Column.CreateFromReader(r, 1);

            Assert.IsNotNull(c1);
            Assert.AreEqual("cola", c1.Name);
            Assert.AreEqual("varchar", c1.DbType);
            Assert.AreSame(typeof(string), c1.ClrType);

            Assert.IsNotNull(c2);
            Assert.AreEqual("colb", c2.Name);
            Assert.AreEqual("int", c2.DbType);
            Assert.AreSame(typeof(int), c2.ClrType);
        }

        [TestMethod]
        public void CreateFromReaderThrowsIfReaderIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Column.CreateFromReader(null, 0);
            }, "reader");
        }

        [TestMethod]
        public void CanValidate()
        {
            Exception ex;
            ex = new Column { Name = null, ClrType = typeof(string), DbType = "int" }.Validate();
            Assert.IsNotNull(ex);
            Assert.AreEqual("Name is null", ex.Message);

            ex = new Column { Name = " ", ClrType = typeof(string), DbType = "int" }.Validate();
            Assert.IsNotNull(ex);
            Assert.AreEqual("Name is empty", ex.Message);

            ex = new Column { Name = "cola", ClrType = null, DbType = "int" }.Validate();
            Assert.IsNotNull(ex);
            Assert.AreEqual("ClrType is null", ex.Message);

            ex = new Column { Name = "cola", ClrType = typeof(int), DbType = null }.Validate();
            Assert.IsNotNull(ex);
            Assert.AreEqual("DbType is null", ex.Message);

            ex = new Column { Name = "cola", ClrType = typeof(int), DbType = "int" }.Validate();
            Assert.IsNull(ex);
        }

        [TestMethod]
        public void SettingClrTypeAffectInternalClrTypeCorrectly()
        {
            var c = new Column { ClrType = typeof(string) };
            Assert.AreEqual("System.String", c.InternalClrType);

            c = new Column { ClrType = null };
            Assert.IsNull(c.InternalClrType);
        }

        [TestMethod]
        public void SettingInternalClrTypeAffectsClrTypeCorrectly()
        {
            var c = new Column { InternalClrType = "System.String" };
            Assert.AreEqual(typeof(string), c.ClrType);

            c = new Column { InternalClrType = null };
            Assert.IsNull(c.ClrType);
        }
    }
}

