using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting.TestSetup;
using System.Configuration;

namespace Data.Tools.UnitTesting.Tests.TestSetup
{
    [TestClass]
    public class TestTests
    {
        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void CreateConnectionsFromConfigThrowsWithNullParameter()
        //{
        //    Test.CreateConnectionsFromConfig(null);
        //}

        //[TestMethod]
        //public void CanCreateConnectionsFromConfig()
        //{
        //    var csc = new ConnectionStringSettingsCollection();
        //    csc.Add(new ConnectionStringSettings { ConnectionString = "cs1", Name = "name1", ProviderName = "System.Data.SqlClient" });
        //    csc.Add(new ConnectionStringSettings { ConnectionString = "cs2", Name = "name2", ProviderName = "System.Data.SqlClient" });

        //    var connections = Test.CreateConnectionsFromConfig(csc);
        //    Assert.IsNotNull(connections);
        //    Assert.AreEqual(2, connections.Count);

        //    Assert.IsNotNull(connections["name1"]);
        //    Assert.AreEqual("cs1", connections["name1"].ConnectionString);
        //    Assert.IsNotNull(connections["name1"].Provider);

        //    Assert.IsNotNull(connections["name2"]);
        //    Assert.AreEqual("cs2", connections["name2"].ConnectionString);
        //    Assert.IsNotNull(connections["name2"].Provider);
        //}
    }
}
