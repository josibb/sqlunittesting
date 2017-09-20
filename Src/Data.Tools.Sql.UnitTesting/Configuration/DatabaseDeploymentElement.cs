using Data.Tools.UnitTesting.TestSetup;
using System;
using System.Configuration;
using System.Xml;
using Data.Tools.UnitTesting.TestSetup.Configuration;

namespace Data.Tools.UnitTesting.Configuration
{


    public class DatabaseDeploymentElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (String)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("createUniqueDatabaseName", IsRequired = false, DefaultValue = false)]
        public bool CreateUniqueDatabaseName
        {
            get { return (bool)this["createUniqueDatabaseName"]; }
            set { this["createUniqueDatabaseName"] = value; }
        }

        [ConfigurationProperty("dropDatabaseOnExit", IsRequired = false, DefaultValue = false)]
        public bool DropDatabaseOnExit
        {
            get { return (bool)this["dropDatabaseOnExit"]; }
            set { this["dropDatabaseOnExit"] = value; }
        }

        // by creating a deployerConfig element by default we keep the configuration behavior the same as in:
        //  all properties are initialized, use elementinformation to determine whether the element was existing during deserialization
        private DeployerConfigElementBase deployerConfig = new DeployerConfigElementBase();
        public DeployerConfigElementBase DeployerConfig { get => deployerConfig; }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            if (elementName == "DeployerConfig")
            {
                var type = System.Type.GetType(this.Type, true);

                var configFactory = Activator.CreateInstance(type) as IDeployerConfigElementFactory;
                if (configFactory == null)
                    throw new InvalidOperationException($"Type '{this.Type}' does not implement '{typeof(IDeployerConfigElementFactory).Name}'");

                this.deployerConfig = configFactory.CreateFromReader(reader);

                return true;
            }
            else
                return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }
    }
}
