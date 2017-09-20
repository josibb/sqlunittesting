using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.IO;
using Data.Tools.UnitTesting.Configuration;
using Data.Tools.UnitTesting.Tests.Utils;
using Data.Tools.UnitTesting.TestSetup;
using Data.Tools.UnitTesting.TestSetup.Configuration;
using System.Xml;
using Data.Tools.UnitTesting.Utils;

namespace Data.Tools.UnitTesting.Tests
{
    [TestClass]
    public class DatabaseUnitTestingSectionTests
    {
        [TestMethod]
        public void CanReadConfigurationSection()
        {
            using (var file = TemporaryConfigurationFile.OpenFromResource("XmlFiles.ConfigFileWithDatabaseDeployment_FormatWithDeploymentInConnection.xml"))
            {
                var section = file.GetConfigSection();

                Assert.IsNotNull(section);
                Assert.IsTrue(section.ElementInformation.IsPresent);
                Assert.IsNotNull(section.Connections);
                Assert.IsTrue(section.Connections.ElementInformation.IsPresent);
                Assert.IsTrue(section.Connections.ElementInformation.IsCollection);
                Assert.AreEqual(3, section.Connections.Count);

                // check first connection without databasedeployment configuration
                Assert.IsNotNull(section.Connections[0]);
                Assert.IsTrue(section.Connections[0].ElementInformation.IsPresent);
                Assert.AreEqual("test1", section.Connections[0].Name);
                Assert.AreEqual("p1", section.Connections[0].ProviderName);
                Assert.AreEqual("c1", section.Connections[0].ConnectionString);
                Assert.IsNotNull(section.Connections[0].DatabaseDepoyment); // instance should be there although it doesnt exist in config. use elementinformation
                Assert.IsFalse(section.Connections[0].DatabaseDepoyment.ElementInformation.IsPresent);
                Assert.IsNotNull(section.Connections[0].DatabaseDepoyment.DeployerConfig);
                Assert.IsFalse(section.Connections[0].DatabaseDepoyment.DeployerConfig.ElementInformation.IsPresent);

                // check second connection with databasdedeployment configuration and deployerconfig element
                Assert.IsNotNull(section.Connections[1]);
                Assert.IsTrue(section.Connections[1].ElementInformation.IsPresent);
                Assert.AreEqual("test2", section.Connections[1].Name);
                Assert.AreEqual("p2", section.Connections[1].ProviderName);
                Assert.AreEqual("c2", section.Connections[1].ConnectionString);
                Assert.IsNotNull(section.Connections[1].DatabaseDepoyment);
                Assert.IsTrue(section.Connections[1].DatabaseDepoyment.ElementInformation.IsPresent);
                Assert.IsTrue(section.Connections[1].DatabaseDepoyment.CreateUniqueDatabaseName);
                Assert.IsTrue(section.Connections[1].DatabaseDepoyment.DropDatabaseOnExit);
                Assert.IsNotNull(section.Connections[1].DatabaseDepoyment.DeployerConfig);
                Assert.IsTrue(section.Connections[1].DatabaseDepoyment.DeployerConfig.ElementInformation.IsPresent);
                Assert.AreSame(typeof(TestDeployerConfigElement), section.Connections[1].DatabaseDepoyment.DeployerConfig.GetType());
                Assert.AreEqual("testvalue", ((TestDeployerConfigElement)section.Connections[1].DatabaseDepoyment.DeployerConfig).TestProperty);

                // check third connection with databasdedeployment configuration without deployerconfig element
                Assert.IsNotNull(section.Connections[2]);
                Assert.IsTrue(section.Connections[2].ElementInformation.IsPresent);
                Assert.AreEqual("test3", section.Connections[2].Name);
                Assert.AreEqual("p3", section.Connections[2].ProviderName);
                Assert.AreEqual("c3", section.Connections[2].ConnectionString);
                Assert.IsNotNull(section.Connections[2].DatabaseDepoyment);
                Assert.IsTrue(section.Connections[2].DatabaseDepoyment.ElementInformation.IsPresent);
                Assert.IsFalse(section.Connections[2].DatabaseDepoyment.CreateUniqueDatabaseName);
                Assert.IsFalse(section.Connections[2].DatabaseDepoyment.DropDatabaseOnExit);
                Assert.IsNotNull(section.Connections[2].DatabaseDepoyment.DeployerConfig);
                Assert.IsFalse(section.Connections[2].DatabaseDepoyment.DeployerConfig.ElementInformation.IsPresent);
            }
        }

        //[TestMethod]
        //public void CanReadSectionWithDatabaseDeploymentAndWithoutRequiredAttributes()

        //[TestMethod]
        //public void CanReadSectionWithRequiredAttributesInDatabaseDeployment()

        //[TestMethod]
        //public void CanReadSectionWithoutDatabaseDeployment()
    }

    /// <summary>
    /// testclass to test configuration reading
    /// </summary>
    public class TestDeployer : IDeployerConfigElementFactory, IDeployerConfigFactory, IDatabaseDeployer
    {
        public DeployerConfigBase CreateFromElement(DeployerConfigElementBase element)
        {
            if (element.IsAvailable())
            {
                var tde = (TestDeployerConfigElement)element;
                return new TestDeployerConfig { TestProperty = tde.TestProperty };
            } else return null;
            
        }

        public DeployerConfigElementBase CreateFromReader(XmlReader reader)
        {
            return TestDeployerConfigElement.CreateFromReader(reader);
        }

        public void DeployDatabase(DeployerConfigBase config, ConnectionContext connection)
        {
            throw new NotImplementedException();
        }

        public void DropDatabase(ConnectionContext connection)
        {
            throw new NotImplementedException();
        }

        public string GetNewUniqueDatabaseName(ConnectionContext connection)
        {
            throw new NotImplementedException();
        }
    }

    public class TestDeployerConfig: DeployerConfigBase
    {
        public string TestProperty { get; set; }
    }

    public class TestDeployerConfigElement : DeployerConfigElementBase
    {
        public static TestDeployerConfigElement CreateFromReader(XmlReader reader)
        {
            var result = new TestDeployerConfigElement();
            result.DeserializeElement(reader, false);
            return result;
        }

        [ConfigurationProperty("testProperty", IsRequired = true)]
        public string TestProperty
        {
            get { return (string)this["testProperty"]; }
            set { this["testProperty"] = value; }
        }
    }
}
