using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Tools.UnitTesting.Utils;
using System;

namespace Data.Tools.UnitTesting.Configuration
{
    public class DatabaseUnitTestingSection : ConfigurationSection
    {
        public static DatabaseUnitTestingSection CreateFromConfiguration()
        {
            var section = ConfigurationManager.GetSection("DatabaseUnitTesting");
            section.ThrowIfNull<InvalidOperationException>("Cannot find section 'DatabaseUnitTesting' in configurationfile");

            var databaseUnitTestingSection = section as DatabaseUnitTestingSection;
            databaseUnitTestingSection.ThrowIfNull<InvalidCastException>("Section 'DatabaseUnitTesting' is of invalid type in configuration file");

            return databaseUnitTestingSection;
        }


        [System.Configuration.ConfigurationProperty("Connections")]
        [ConfigurationCollection(typeof(ConnectionElementCollection), AddItemName = "Connection")]
        public ConnectionElementCollection Connections
        {
            get
            {
                object o = this["Connections"];
                return o as ConnectionElementCollection;
            }
        }
    }
}
