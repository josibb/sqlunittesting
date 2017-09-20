using Data.Tools.UnitTesting.Configuration;
using Data.Tools.UnitTesting.TestSetup.Configuration;
using System.Configuration;
using System.Xml;

namespace Data.Tools.UnitTesting.Sql.Configuration
{
    public class SSDTProjectDeployerConfigElement : DeployerConfigElementBase
    {
        public static SSDTProjectDeployerConfigElement CreateFromReader(XmlReader reader)
        {
            var result = new SSDTProjectDeployerConfigElement();
            result.DeserializeElement(reader, false);
            return result;
        }

        [ConfigurationProperty("databaseProjectFileName", IsRequired = true)]
        public string DatabaseProjectFileName
        {
            get { return (string)this["databaseProjectFileName"]; }
            set { this["databaseProjectFileName"] = value; }
        }

        [ConfigurationProperty("buildConfiguration", IsRequired = true)]
        public string BuildConfiguration
        {
            get { return (string)this["buildConfiguration"]; }
            set { this["buildConfiguration"] = value; }
        }
    }
}
