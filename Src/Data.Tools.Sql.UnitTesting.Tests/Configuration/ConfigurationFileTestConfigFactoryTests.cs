using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting.Configuration;
using Data.Tools.UnitTesting.Tests.Utils;

namespace Data.Tools.UnitTesting.Tests.Configuration
{
    [TestClass]
    public class ConfigurationFileTestConfigFactoryTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var file = TemporaryConfigurationFile.OpenFromResource("XmlFiles.ConfigFileWith3Connections.xml"))
            {
                var section = file.GetConfigSection();
                var config = ConfigurationFileTestConfigFactory.CreateFromConfiguration(section);

                Assert.IsNotNull(config);
                Assert.IsNotNull(config.Connections2);
                Assert.AreEqual(3, config.Connections2.Count);

                var con0 = config.Connections2[0];
                Assert.IsNotNull(con0);
                Assert.AreEqual("con0", con0.Name);
                Assert.AreEqual("System.Data.SqlClient", con0.ProviderName);
                Assert.IsNotNull(con0.Provider);
                Assert.AreEqual("c1", con0.ConnectionString);
                Assert.IsNull(con0.Deployment);

                var con1 = config.Connections2[1];
                Assert.IsNotNull(con1);
                Assert.AreEqual("con1", con1.Name);
                Assert.AreEqual("System.Data.SqlClient", con1.ProviderName);
                Assert.IsNotNull(con1.Provider);
                Assert.AreEqual("c2", con1.ConnectionString);
                Assert.IsNotNull(con1.Deployment);
                Assert.AreSame(con1, con1.Deployment.ConnectionContext);
                Assert.IsTrue(con1.Deployment.CreateUniqueDatabaseName);
                Assert.IsTrue(con1.Deployment.DropDatabaseOnExit);
                Assert.IsNotNull(con1.Deployment.DeployerConfig);
                Assert.AreSame(typeof(TestDeployerConfig), con1.Deployment.DeployerConfig.GetType());
                Assert.AreEqual("testvalue", ((TestDeployerConfig)con1.Deployment.DeployerConfig).TestProperty);
                Assert.IsNotNull(con1.Deployment.DatabaseDeployer);
                Assert.AreSame(typeof(TestDeployer), con1.Deployment.DatabaseDeployer.GetType());

                var con2 = config.Connections2[2];
                Assert.IsNotNull(con2);
                Assert.AreEqual("con2", con2.Name);
                Assert.AreEqual("System.Data.SqlClient", con2.ProviderName);
                Assert.IsNotNull(con2.Provider);
                Assert.AreEqual("c3", con2.ConnectionString);
                Assert.IsNotNull(con2.Deployment);
                Assert.AreSame(con2, con2.Deployment.ConnectionContext);
                Assert.IsFalse(con2.Deployment.CreateUniqueDatabaseName);
                Assert.IsFalse(con2.Deployment.DropDatabaseOnExit);
                Assert.IsNull(con2.Deployment.DeployerConfig);
                Assert.IsNotNull(con2.Deployment.DatabaseDeployer);
                Assert.AreSame(typeof(TestDeployer), con2.Deployment.DatabaseDeployer.GetType());
            }


        }
    }
}
