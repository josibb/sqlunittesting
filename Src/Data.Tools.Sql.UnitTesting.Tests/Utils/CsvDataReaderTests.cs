using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting.Utils;
using Data.Tools.UnitTesting.Result;
using System.Globalization;

namespace Data.Tools.UnitTesting.Tests.Utils
{
    [TestClass]
    public class CsvDataReaderTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var data = new CsvData
            {
                ColumnNames = new string[] { "cola", "colb", "colc" },
                RowValues = new string[][] {
                    new string[] { "vala1", "13-01-1979", "12,5" }, //row0
                    new string[] { "vala2", "29-04-1980", "-14,4" }  //row1
                },
                CultureInfo = CultureInfo.GetCultureInfo("nl-NL")
            };

            var schemaColumns = new ColumnCollection();
            schemaColumns.Add(new Column { Name = "cola", ClrType = typeof(string) });
            schemaColumns.Add(new Column { Name = "colb", ClrType = typeof(DateTime) });
            schemaColumns.Add(new Column { Name = "colc", ClrType = typeof(decimal) });

            var dr = new CsvDataReader(data, schemaColumns, "NULL");

            Assert.IsTrue(dr.Read());
            Assert.AreEqual("vala1", dr.GetValue(0));
            Assert.AreEqual(new DateTime(1979, 1, 13), dr.GetValue(1));
            Assert.AreEqual(12.5m, dr.GetValue(2));

            Assert.IsTrue(dr.Read());
            Assert.AreEqual("vala2", dr.GetValue(0));
            Assert.AreEqual(new DateTime(1980, 4, 29), dr.GetValue(1));
            Assert.AreEqual(-14.4m, dr.GetValue(2));

            Assert.IsFalse(dr.Read());
        }
    }
}
