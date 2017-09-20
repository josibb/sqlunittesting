using Data.Tools.UnitTesting.TestSetup.Configuration;
using Data.Tools.UnitTesting.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Tools.UnitTesting.TestSetup;

namespace Data.Tools.UnitTesting.Configuration
{
    public static class ConfigurationFileTestConfigFactory
    {
        public static TestConfig CreateFromConfigurationFile()
        {
            return CreateFromConfiguration(DatabaseUnitTestingSection.CreateFromConfiguration());
        }

        internal static TestConfig CreateFromConfiguration(DatabaseUnitTestingSection section)
        {
            section.ThrowIfNull("section");

            return new TestConfig
            {
                Connections2 = CreateDatabaseConnectionsFromConnectionsElementCollection(section.Connections)
            };
        }

        internal static IList<ConnectionContext> CreateDatabaseConnectionsFromConnectionsElementCollection(ConnectionElementCollection element)
        {
            var result = new List<ConnectionContext>();

            if (element != null && element.ElementInformation != null && element.ElementInformation.IsPresent)
            {
                for (var t=0; t< element.Count; t++)
                {
                    result.Add(CreateConnectionContext(element[t]));
                }
            }

            return result;
        }

        internal static ConnectionContext CreateConnectionContext(ConnectionElement element)
        {
            element.ThrowIfNull("element");
            element.Name.ThrowIfNullOrEmpty<InvalidOperationException>("No name specified for connectionstring");
            element.ProviderName.ThrowIfNullOrEmpty<InvalidOperationException>("No provider specified in connectionString");
            element.ConnectionString.ThrowIfNullOrEmpty<InvalidOperationException>("No connectionstring specified");

            var result = new ConnectionContext
            {
                Name = element.Name,
                ConnectionString = element.ConnectionString,
                Provider = DbProviderFactories.GetFactory(element.ProviderName),
                ProviderName = element.ProviderName,
            };

            result.Deployment = CreateDatabaseDeploymentFromElement(element.DatabaseDepoyment, result);

            return result;
        }

        internal static DatabaseDeployment CreateDatabaseDeploymentFromElement(DatabaseDeploymentElement element, ConnectionContext connectionContext)
        {
            if (element != null && element.ElementInformation != null && element.ElementInformation.IsPresent)
            {
                return new DatabaseDeployment
                {
                    ConnectionContext = connectionContext,
                    CreateUniqueDatabaseName = element.CreateUniqueDatabaseName,
                    DropDatabaseOnExit = element.DropDatabaseOnExit,
                    DatabaseDeployer = CreateDatabaseDeployerFromElement(element),
                    DeployerConfig = CreateDeployerConfigFromElement(element.DeployerConfig, element.Type)
                };
            }
            else return null;
        }

        internal static IDatabaseDeployer CreateDatabaseDeployerFromElement(DatabaseDeploymentElement element)
        {
            if (element != null && element.ElementInformation != null && element.ElementInformation.IsPresent)
            {
                element.Type.ThrowIfNullOrEmpty<InvalidOperationException>("Deployer is null or empty");

                Type type = Type.GetType(element.Type, true);
                return (IDatabaseDeployer)Activator.CreateInstance(type);
            }
            else return null;
        }

        internal static DeployerConfigBase CreateDeployerConfigFromElement(DeployerConfigElementBase element, string configFactoryType)
        {
            if (element.IsAvailable())
            {
                configFactoryType.ThrowIfNullOrEmpty<InvalidOperationException>("ConfigFactoryType is null or empty");

                Type type = Type.GetType(configFactoryType, true);
                var factory = (IDeployerConfigFactory)Activator.CreateInstance(type);

                return factory.CreateFromElement(element);
            }
            else return null;
        }
    }
}
