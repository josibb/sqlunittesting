using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting.TestSetup;
using System.Configuration;
using Data.Tools.UnitTesting.Configuration;

namespace Data.Tools.UnitTesting.Tests.TestSetup
{
    //[TestClass]
    //public class ConnectionContextTests
    //{
    //    [TestMethod]
    //    public void CreatrFromConnectionStringTHrowsWithNullParameter()
    //    {
    //        Assert.ThrowsException<ArgumentNullException>(() =>
    //        {
    //            ConnectionContext.CreateFromConnectionString(null);
    //        });
    //    }

    //    [TestMethod]
    //    public void CannotCreateContextWithoutName()
    //    {
    //        Assert.AreEqual("No name specified for connectionstring", Assert.ThrowsException<InvalidOperationException>(() =>
    //        {
    //            ConnectionContext.CreateFromConnectionString(new ConnectionStringSettings
    //            {
    //                ConnectionString = "abc",
    //                ProviderName = "provider",
    //                Name = null
    //            });
    //        }).Message);
    //    }

    //    [TestMethod]
    //    public void CannotCreateContextWithoutProvider()
    //    {
    //        Assert.AreEqual("No provider specified in connectionString", Assert.ThrowsException<InvalidOperationException>(() =>
    //        {
    //            ConnectionContext.CreateFromConnectionString(new ConnectionStringSettings
    //            {
    //                ConnectionString = "abc",
    //                ProviderName = null,
    //                Name = "name"
    //            });
    //        }).Message);
    //    }

    //    [TestMethod]
    //    public void CannotCreateContextWithoutConnectionString()
    //    {
    //        var ex = Assert.ThrowsException<InvalidOperationException>(() =>
    //        {
    //            ConnectionContext.CreateFromConnectionString(new ConnectionStringSettings
    //            {
    //                ConnectionString = null,
    //                ProviderName = "provider",
    //                Name = "name"
    //            });
    //        });

    //        Assert.AreEqual("No connectionstring specified", ex.Message);
    //    }

    //    [TestMethod]
    //    public void CannotCreateContextWithInvalidProvider()
    //    {
    //        var ex = Assert.ThrowsException<ArgumentException>(() =>
    //        {
    //            ConnectionContext.CreateFromConnectionString(new ConnectionStringSettings
    //            {
    //                ConnectionString = "abc",
    //                ProviderName = "provider",
    //                Name = "abc"
    //            });
    //        });

    //        Assert.AreEqual("Unable to find the requested .Net Framework Data Provider.  It may not be installed.", ex.Message);
    //    }

    //    [TestMethod]
    //    public void CanCreateContext()
    //    {
    //        var cc = ConnectionContext.CreateFromConnectionString(new ConnectionStringSettings
    //        {
    //            ConnectionString = "connectionstring",
    //            ProviderName = "System.Data.SqlClient",
    //            Name = "cs1"
    //        });

    //        Assert.IsNotNull(cc);
    //        Assert.AreEqual("connectionstring", cc.ConnectionString);
    //        Assert.IsNotNull(cc.Provider);
    //    }
    //}
}
