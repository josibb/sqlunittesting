using Data.Tools.UnitTesting.TestSetup;
using Microsoft.Data.Tools.Schema.Sql.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Tools.UnitTesting.TestSetup.Configuration;
using Data.Tools.UnitTesting.Utils;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using System.Xml;
using Data.Tools.UnitTesting.Configuration;
using Data.Tools.UnitTesting.Sql.Configuration;
using Data.Tools.UnitTesting.Sql.TestSetup;

namespace Data.Tools.UnitTesting.Sql
{
    public class SSDTProjectDeployer : IDatabaseDeployer, IDeployerConfigElementFactory, IDeployerConfigFactory
    {
        public DeployerConfigElementBase CreateFromReader(XmlReader reader)
        {
            return SSDTProjectDeployerConfigElement.CreateFromReader(reader);
        }

        public DeployerConfigBase CreateFromElement(DeployerConfigElementBase deployerElement)
        {
            if (deployerElement.IsAvailable())
            {
                var el = (SSDTProjectDeployerConfigElement)deployerElement;

                el.BuildConfiguration.ThrowIfNullOrEmpty<InvalidOperationException>("BuildConfiguration is null or empty");
                el.DatabaseProjectFileName.ThrowIfNullOrEmpty<InvalidOperationException>("DatabaseProjectFileName is null or empty");

                return new SSDTProjectDeployerConfig
                {
                    BuildConfiguration = el.BuildConfiguration,
                    DatabaseProjectFileName = el.DatabaseProjectFileName
                };
            }
            else return null;
        }

        public void DeployDatabase(DeployerConfigBase config, UnitTesting.TestSetup.Configuration.ConnectionContext connection)
        {
            config.ThrowIfNull("config");
            connection.ThrowIfNull("connection");
            connection.ConnectionString.ThrowIfNull<InvalidOperationException>("ConnectionString is null");
            connection.ProviderName.ThrowIfNull<InvalidOperationException>("ProviderName is null");

            var deployerConfig = config as SSDTProjectDeployerConfig;
            deployerConfig.ThrowIfNull<InvalidCastException>($"Cannot cast config of type '{config.GetType().Name}' to '{typeof(SSDTProjectDeployerConfig).Name}'");

            InternalDeploy.Deploy(
                deployerConfig.DatabaseProjectFileName,
                deployerConfig.BuildConfiguration,
                connection.ProviderName,
                connection.ConnectionString
                );
        }

        public void DropDatabase(Data.Tools.UnitTesting.TestSetup.Configuration.ConnectionContext connection)
        {
            // connect to master database and execute drop database statement for databasename in connectionstring of connection
            connection.ThrowIfNull("databaseDeployment");

            using (var cn = connection.Provider.CreateConnection())
            {
                cn.ConnectionString = connection.GetConnectionStringForDatabaseFromConnectionContext("master");
                cn.Open();

                using (var cm = cn.CreateCommand())
                {
                    cm.CommandText = $"DROP DATABASE [{GetDatabaseNameFromConnectionContext(connection)}]";
                    cm.ExecuteNonQuery();
                }
            }
        }

        public string GetNewUniqueDatabaseName(Data.Tools.UnitTesting.TestSetup.Configuration.ConnectionContext connection)
        {
            connection.ThrowIfNull("connection");

            using (var cn = connection.Provider.CreateConnection())
            {
                cn.ConnectionString = connection.GetConnectionStringForDatabaseFromConnectionContext("master");
                cn.Open();

                var dbName = $"{GetDatabaseNameFromConnectionContext(connection)}-{DateTime.UtcNow.ToString("o")}";

                while (DoesDatabaseExist(cn, dbName))
                {
                    dbName = $"{GetDatabaseNameFromConnectionContext(connection)}-{DateTime.UtcNow.ToString("o")}";
                }

                return dbName;
                //return GetConnectionStringForDatabaseNameFromConnectionContext(connection, dbName);
            }
        }

        internal static string GetDatabaseNameFromConnectionContext(Data.Tools.UnitTesting.TestSetup.Configuration.ConnectionContext connectionContext)
        {
            var cb = connectionContext.Provider.CreateConnectionStringBuilder();
            cb.ConnectionString = connectionContext.ConnectionString;

            object dbName;
            if (!cb.TryGetValue("Initial Catalog", out dbName))
                throw new InvalidOperationException($"Cannot find databasename in connectionstring '{connectionContext.ConnectionString}'");

            return dbName.ToString();
        }

        internal static bool DoesDatabaseExist(IDbConnection cn, string databaseName)
        {
            using (var cm = cn.CreateCommand())
            {
                cm.CommandText = "SELECT COUNT(*) FROM [master].[sys].[sysdatabases] WHERE name = @name";
                var par = cm.CreateParameter();
                par.DbType = DbType.String;
                par.Direction = ParameterDirection.Input;
                par.ParameterName = "@name";
                par.Value = databaseName;
                cm.Parameters.Add(par);
                var count = (int)cm.ExecuteScalar();
                return count > 0;
            }
        }


    }

    internal class InternalDeploy : SqlDatabaseTestService
    {
        public static void Deploy(string databaseProjectFileName, string configuration, string providerInvariantName, string connectionString)
        {
            InternalDeploy.DeployDatabaseProject(databaseProjectFileName, configuration, providerInvariantName, connectionString);
        }
    }
}
