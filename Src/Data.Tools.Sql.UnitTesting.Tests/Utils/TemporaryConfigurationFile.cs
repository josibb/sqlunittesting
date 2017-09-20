using Data.Tools.UnitTesting.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Tests.Utils
{
    public class TemporaryConfigurationFile: TemporaryFileBase<TemporaryConfigurationFile>
    {
        public DatabaseUnitTestingSection GetConfigSection()
        {
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = this.File.FullName };
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            return (DatabaseUnitTestingSection)config.GetSection("DatabaseUnitTesting");
        }
    }
}
