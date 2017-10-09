using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting.Utils;
using System.Globalization;

namespace Data.Tools.UnitTesting.Tests.UtilsTests
{
    //[TestClass]
    //public class UnitTest1
    //{
    //    [TestMethod]
    //    public void CanParseValue()
    //    {
    //        var enCulture = CultureInfo.GetCultureInfo("en-US");
    //        var dutCulture = CultureInfo.GetCultureInfo("nl-NL");

    //        var i = UntypedDataSource.ParseValue(typeof(int), "12", enCulture);
    //        Assert.AreSame(typeof(int), i.GetType());
    //        Assert.AreEqual(12, i);

    //        i = UntypedDataSource.ParseValue(typeof(int), "12", dutCulture);
    //        Assert.AreSame(typeof(int), i.GetType());
    //        Assert.AreEqual(12, i);

    //        var f = UntypedDataSource.ParseValue(typeof(decimal), "12.4", enCulture);
    //        Assert.AreSame(typeof(decimal), f.GetType());
    //        Assert.AreEqual(12.4m, f);

    //        f = UntypedDataSource.ParseValue(typeof(decimal), "12,4", dutCulture);
    //        Assert.AreSame(typeof(decimal), f.GetType());
    //        Assert.AreEqual(12.4m, f);
    //    }

    //    [TestMethod]
    //    public void ParseValueThrowsIfIncorrectText()
    //    {
    //        Assert.ThrowsException<FormatException>(() => {
    //            var r = UntypedDataSource.ParseValue(typeof(DateTime), "abc", CultureInfo.GetCultureInfo("en-US"));
    //        });
    //    }
    //}
}
