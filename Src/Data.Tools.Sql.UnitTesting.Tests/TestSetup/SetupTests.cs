using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting.TestSetup;
using Data.Tools.UnitTesting.TestSetup.Configuration;
using System.Data.Common;
using System.Collections.Generic;

namespace Data.Tools.UnitTesting.Tests.TestSetup
{
    [TestClass]
    public class SetupTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructWithNullConfigTHrows()
        {
            new Setup(null);
        }

        [TestMethod]
        public void ConnectionsReturnSameDictionaryAsConfiguration()
        {
            var config = new TestConfig();
            config.Connections["con1"] = new ConnectionContext();
            config.Connections["con2"] = new ConnectionContext();

            var setup = new Setup(config);
            Assert.AreSame(config.Connections, setup.Connections);
        }

        [TestMethod]
        public void CanInitializeDefault()
        {
            var config = new TestConfig();
            config.Connections["con1"] = new ConnectionContext();
            config.Connections["con2"] = new ConnectionContext();

            Setup.TeardownDefault();
            Setup.InitializeDefault(config);

            Assert.IsNotNull(Setup.Default);
            Assert.AreSame(config.Connections, Setup.Default.Connections);
        }

        [TestMethod]
        public void DefaultThrowsIfNotInitialized()
        {
            Setup.TeardownDefault();

            var ex = Assert.ThrowsException<InvalidOperationException>(() =>
            {
                var t = Setup.Default;
            });

            Assert.AreEqual("Default setup not initialized", ex.Message);
        }

        [TestMethod]
        public void DefaultInitializetionTwiceTHrows()
        {
            Setup.TeardownDefault();

            Setup.InitializeDefault(new TestConfig());

            var ex = Assert.ThrowsException<InvalidOperationException>(() =>
            {
                Setup.InitializeDefault(new TestConfig());
            });

            Assert.AreEqual("Default setup already initialized", ex.Message);
        }

        [TestMethod]
        public void DatabasesAreDeployed()
        {
            var dbDeployer = new TestDatabaseDeployer();

            var config = new TestConfig();

            var con1 =
             new ConnectionContext
             {
                 Provider = DbProviderFactories.GetFactory("System.Data.Sqlclient"),
                 Name = "con1",
                 ProviderName = "System.Data.SqlClient",
                 ConnectionString = "1"
             };
            config.Connections["con1"] = con1;

            var con2 =
             new ConnectionContext
             {
                 Provider = DbProviderFactories.GetFactory("System.Data.Sqlclient"),
                 Name = "con1",
                 ProviderName = "System.Data.SqlClient",
                 ConnectionString = "2"
             };
            config.Connections["con2"] = con2;

            //config.DatabaseDeployments.Add(new DatabaseDeployment
            //{
            //    ConnectionContext = config.Connections["con1"],
            //    DatabaseDeployer = dbDeployer,
            //    DatabaseProjectBuildConfiguration = "buildconfig1",
            //    CreateUniqueDatabaseName = true,
            //    DatabaseProjectFileName = "ssdt.csproj",
            //    DropDatabaseOnExit = true
            //});

            //config.DatabaseDeployments.Add(new DatabaseDeployment
            //{
            //    ConnectionContext = config.Connections["con2"],
            //    DatabaseDeployer = dbDeployer,
            //    DatabaseProjectBuildConfiguration = "buildconfig2",
            //    CreateUniqueDatabaseName = false,
            //    DatabaseProjectFileName = "ssdt2.csproj",
            //    DropDatabaseOnExit = false

            //});

            var setup = new Setup(config);
            setup.Initialize();

            // get unique database should be invoked once for first deployment
            Assert.AreEqual(1, dbDeployer.GetUniqueDatabaseName_connections.Count);
            Assert.AreSame(con1, dbDeployer.GetUniqueDatabaseName_connections[0]);

            // for unique database, the connection should be replaced in the config with the connection with the unique databasename in it
            Assert.AreNotSame(con1, config.Connections["con1"]);
            //Assert.AreNotSame(con1, config.DatabaseDeployments[0].ConnectionContext);
            //Assert.AreSame(config.Connections["con1"], config.DatabaseDeployments[0].ConnectionContext);
            //Assert.AreEqual("Initial Catalog=uniquedbname", config.Connections["con1"].ConnectionString);
            //Assert.AreSame(con2, config.Connections["con2"]);
            //Assert.AreSame(con2, config.DatabaseDeployments[1].ConnectionContext);

            //// deploy db should be invoked twice for both deployments
            //Assert.AreEqual(2, dbDeployer.DeployDatabase_databaseDeployments.Count);
            //Assert.AreNotSame(con1, dbDeployer.DeployDatabase_databaseDeployments[0]); // must be replaced for unique db
            //Assert.AreSame(config.DatabaseDeployments[0], dbDeployer.DeployDatabase_databaseDeployments[0]);
            //Assert.AreSame(config.DatabaseDeployments[1], dbDeployer.DeployDatabase_databaseDeployments[1]);
        }

        [TestMethod]
        public void DatabasesAreDropped()
        {
            var dbDeployer = new TestDatabaseDeployer();

            var config = new TestConfig();

            var con1 =
             new ConnectionContext
             {
                 Provider = DbProviderFactories.GetFactory("System.Data.Sqlclient"),
                 Name = "con1",
                 ProviderName = "System.Data.SqlClient",
                 ConnectionString = "1"
             };
            config.Connections["con1"] = con1;

            var con2 =
             new ConnectionContext
             {
                 Provider = DbProviderFactories.GetFactory("System.Data.Sqlclient"),
                 Name = "con1",
                 ProviderName = "System.Data.SqlClient",
                 ConnectionString = "2"
             };
            config.Connections["con2"] = con2;

            config.DatabaseDeployments.Add(new DatabaseDeployment
            {
                ConnectionContext = config.Connections["con1"],
                DatabaseDeployer = dbDeployer,
                DatabaseProjectBuildConfiguration = "buildconfig1",
                CreateUniqueDatabaseName = true,
                DatabaseProjectFileName = "ssdt.csproj",
                DropDatabaseOnExit = true
            });

            config.DatabaseDeployments.Add(new DatabaseDeployment
            {
                ConnectionContext = config.Connections["con2"],
                DatabaseDeployer = dbDeployer,
                DatabaseProjectBuildConfiguration = "buildconfig2",
                CreateUniqueDatabaseName = false,
                DatabaseProjectFileName = "ssdt2.csproj",
                DropDatabaseOnExit = true
            });

            config.DatabaseDeployments.Add(new DatabaseDeployment
            {
                ConnectionContext = config.Connections["con2"],
                DatabaseDeployer = dbDeployer,
                DatabaseProjectBuildConfiguration = "buildconfig2",
                CreateUniqueDatabaseName = false,
                DatabaseProjectFileName = "ssdt2.csproj",
                DropDatabaseOnExit = false

            });

            var setup = new Setup(config);
            setup.Initialize();

            Assert.AreEqual(0, dbDeployer.DropDatabase_connections.Count);

            setup.Dispose();

            Assert.AreEqual(2, dbDeployer.DropDatabase_connections.Count);
            Assert.AreSame(config.Connections["con1"], dbDeployer.DropDatabase_connections[0]);
            Assert.AreSame(config.Connections["con2"], dbDeployer.DropDatabase_connections[1]);

        }

        private class TestDatabaseDeployer : IDatabaseDeployer
        {
            public IList<DatabaseDeployment> DeployDatabase_databaseDeployments = new List<DatabaseDeployment>();
            public IList<ConnectionContext> DropDatabase_connections = new List<ConnectionContext>();
            public IList<ConnectionContext> GetUniqueDatabaseName_connections = new List<ConnectionContext>();

            public void DeployDatabase(DatabaseDeployment databaseDeployment)
            {
                DeployDatabase_databaseDeployments.Add(databaseDeployment);
            }

            public void DropDatabase(ConnectionContext connection)
            {
                DropDatabase_connections.Add(connection);
            }

            public ConnectionContext GetUniqueDatabaseName(ConnectionContext connection)
            {
                GetUniqueDatabaseName_connections.Add(connection);

                return new ConnectionContext
                {
                    ConnectionString = "Initial Catalog=uniquedbname",
                    Name = connection.Name,
                    Provider = connection.Provider,
                    ProviderName = connection.ProviderName
                };
            }
        }
    }
}
