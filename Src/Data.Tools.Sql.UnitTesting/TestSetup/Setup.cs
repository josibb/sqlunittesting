using Data.Tools.UnitTesting.Configuration;
using System.Collections.Generic;
using Data.Tools.UnitTesting.Utils;
using System;
using Data.Tools.UnitTesting.TestSetup.Configuration;
using System.Data.Common;
using System.Linq;

namespace Data.Tools.UnitTesting.TestSetup
{
    public class Setup : IDisposable
    {
        private readonly TestConfig configuration = null;

        public IDictionary<string, ConnectionContext> Connections { get => configuration.Connections2.ToDictionary(a => a.Name, b => b); }

        public Setup(TestConfig configuration)
        {
            configuration.ThrowIfNull("configuration");

            this.configuration = configuration;
        }

        public void Initialize()
        {
            DeployDatabasesConfiguredForDeploy(configuration.Connections2);
        }
        
        public void Dispose()
        {
            DropDatabasesConfiguredForDrop(configuration.Connections2);
        }

        internal static void DeployDatabasesConfiguredForDeploy(IEnumerable<ConnectionContext> connections)
        {
            /*
             * if a database needs to be created uniquely, change the connectionstring.
             * the deployer itself is responsible for creating the database
             */
            foreach (var c in connections)
            {
                if (c.Deployment != null)
                {
                    if (c.Deployment.CreateUniqueDatabaseName)
                        c.ConnectionString = c.GetConnectionStringForDatabaseFromConnectionContext(c.Deployment.DatabaseDeployer.GetNewUniqueDatabaseName(c));

                    c.Deployment.DatabaseDeployer.DeployDatabase(c.Deployment.DeployerConfig, c.Deployment.ConnectionContext);
                }
            }
        }

        //internal static string GetConnectionStringForDatabaseFromConnectionContext(Data.Tools.UnitTesting.TestSetup.Configuration.ConnectionContext connection, string dbName)
        //{
        //    var cb = connection.Provider.CreateConnectionStringBuilder();
        //    cb.ConnectionString = connection.ConnectionString;

        //    cb["Initial Catalog"] = dbName;

        //    return cb.ConnectionString;
        //}

        internal static void DropDatabasesConfiguredForDrop(IEnumerable<ConnectionContext> connections)
        {
            foreach (var c in connections)
            {
                if (c.Deployment != null && c.Deployment.DropDatabaseOnExit)
                {
                    c.Deployment.DatabaseDeployer.DropDatabase(c);
                }
            }
        }

        #region Default static  

        private static Setup _default = null;
        public static Setup Default
        {
            get
            {
                Setup._default.ThrowIfNull<InvalidOperationException>("Default setup not initialized");
                return Setup._default;
            }
        }

        public static void InitializeDefault(TestConfig configuration)
        {
            if (_default != null)
                throw new InvalidOperationException("Default setup already initialized");

            _default = new Setup(configuration);
            _default.Initialize();
        }

        public static void TeardownDefault()
        {
            var tmp = _default;
            _default = null;

            if (tmp != null)
            {
                tmp.Dispose();
            }
        }

        #endregion

    }
}

