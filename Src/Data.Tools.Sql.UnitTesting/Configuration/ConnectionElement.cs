using System;
using System.Configuration;

namespace Data.Tools.UnitTesting.Configuration
{
    public class ConnectionElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("providerName", IsRequired = true)]
        public string ProviderName
        {
            get { return (String)this["providerName"]; }
            set { this["providerName"] = value; }
        }

        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return (String)this["connectionString"]; }
            set { this["connectionString"] = value; }
        }

        [ConfigurationProperty("DatabaseDeployment", IsRequired = false)]
        public DatabaseDeploymentElement DatabaseDepoyment
        {
            get { return (DatabaseDeploymentElement)this["DatabaseDeployment"]; }
            set { this["DatabaseDepoyment"] = value; }
        }
    }
}
