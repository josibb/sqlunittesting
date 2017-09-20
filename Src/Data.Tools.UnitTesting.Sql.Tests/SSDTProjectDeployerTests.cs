using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using Data.Tools.UnitTesting.TestSetup.Configuration;
using System.Data.SqlClient;
using Data.Tools.UnitTesting.Sql.TestSetup;

namespace Data.Tools.UnitTesting.Sql.Tests
{
    [TestClass]
    public class SSDTProjectDeployerTests
    {
        [TestMethod]
        public void CanGetDatabaseNameFromConnectionContext()
        {
            var cc = new ConnectionContext
            {
                ConnectionString = @"Data Source=.\SQLExpress;Initial Catalog=abc;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True",
                Provider = DbProviderFactories.GetFactory("System.Data.SqlClient")
            };

            Assert.AreEqual("abc", SSDTProjectDeployer.GetDatabaseNameFromConnectionContext(cc));
        }

        [TestMethod]
        public void CanGetConnectionStringForDatabaseNameFromConnectionContext()
        {
            var cc = new ConnectionContext
            {
                ConnectionString = @"Data Source=.\SQLExpress;Initial Catalog=abc;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True",
                Provider = DbProviderFactories.GetFactory("System.Data.SqlClient")
            };

            var cs = cc.GetConnectionStringForDatabaseFromConnectionContext("klm");

            Assert.AreNotEqual(cc.ConnectionString, cs);
            cc.ConnectionString = cs;
            Assert.AreEqual("klm", SSDTProjectDeployer.GetDatabaseNameFromConnectionContext(cc));

        }

        private const string DbConnectionString = @"Data Source=.\SQLExpress;Integrated Security=True;Persist Security Info=False;Pooling=False;Connect Timeout=60";

        [TestMethod]
        public void CanCheckIfDatabaseExists()
        {
            using (var cn = new SqlConnection(DbConnectionString))
            {
                cn.Open();

                Assert.IsTrue(SSDTProjectDeployer.DoesDatabaseExist(cn, "master"));
                Assert.IsFalse(SSDTProjectDeployer.DoesDatabaseExist(cn, "oaisdfjosajasijsdaij1234"));
            }
        }

        private string GetDatabaseName(ConnectionContext cc)
        {
            var cb = cc.Provider.CreateConnectionStringBuilder();
            cb.ConnectionString = cc.ConnectionString;

            object dbName;
            if (cb.TryGetValue("Initial Catalog", out dbName))
                return dbName.ToString();
            else
                throw new InvalidOperationException($"Cannot find databasename in connectionstring '{cc.ConnectionString}'");
        }


        [TestMethod]
        [TestCategory("L2")]
        public void CanDeployDatabase_CanCheckExists_CanDrop()
        {
            string CreateConnectionStringForDatabase(string databaseName)
            {
                return $"{DbConnectionString};Initial Catalog={databaseName}";
            }

            ConnectionContext CreateConnectionContext(string connectionString)
            {
                return new ConnectionContext
                {
                    ConnectionString = connectionString,
                    Provider = DbProviderFactories.GetFactory("System.Data.SqlClient"),
                    ProviderName = "System.Data.SqlClient"
                };
            }

            var newUniqueDbName = new SSDTProjectDeployer().GetNewUniqueDatabaseName(CreateConnectionContext(CreateConnectionStringForDatabase("SqlDeployerTest")));
            var newUniqueDbConnectionString = CreateConnectionStringForDatabase(newUniqueDbName);
            var newUniqueDbConnectionContext = CreateConnectionContext(newUniqueDbConnectionString);
            
            // check that database does NOT exist
            // - use deployer function
            // - 
            using (var cn = new SqlConnection(CreateConnectionStringForDatabase("master")))
            {
                cn.Open();

                Assert.IsFalse(SSDTProjectDeployer.DoesDatabaseExist(cn, newUniqueDbName));

                using (var cm = cn.CreateCommand())
                {
                    cm.CommandText = $"SELECT COUNT(*) FROM [{newUniqueDbName}].dbo.Test";
                    try
                    {
                        cm.ExecuteNonQuery();
                        Assert.Fail("expected exception not thrown");
                    }
                    catch (Exception ex)
                    {
                        Assert.AreSame(typeof(SqlException), ex.GetType());
                    }
                }
            }


            // deploy database to unique new db
            new SSDTProjectDeployer().DeployDatabase(new SSDTProjectDeployerConfig
            {
                DatabaseProjectFileName = @"..\..\..\Data.Tools.UnitTesting.Sql.Tests.SSDT\Data.Tools.UnitTesting.Sql.Tests.SSDT.sqlproj",
                BuildConfiguration = "Debug"
            }, newUniqueDbConnectionContext);

            // check that database exists and deployed correctly (run query against it)
            using (var cn = new SqlConnection(newUniqueDbConnectionContext.ConnectionString))
            {
                cn.Open();

                Assert.IsTrue(SSDTProjectDeployer.DoesDatabaseExist(cn, newUniqueDbName));

                using (var cm = cn.CreateCommand())
                {
                    cm.CommandText = "SELECT COUNT(*) FROM Test";
                    Assert.AreEqual(0, cm.ExecuteScalar());
                }
            }

            new SSDTProjectDeployer().DropDatabase(newUniqueDbConnectionContext);

            using (var cn = new SqlConnection(CreateConnectionStringForDatabase("master")))
            {
                cn.Open();

                Assert.IsFalse(SSDTProjectDeployer.DoesDatabaseExist(cn, newUniqueDbName));
            }
        }

        public void CanDropDatabase()
        {

        }


        /*
         * deploy database test (add ssdt project (simple one)
         * drop database
         * databaseexist test (check for tempdb / master).
         * 
         */
    }
}
