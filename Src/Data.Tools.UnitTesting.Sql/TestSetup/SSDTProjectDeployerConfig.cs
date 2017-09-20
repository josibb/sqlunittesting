using Data.Tools.UnitTesting.TestSetup.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Sql.TestSetup
{
    public class SSDTProjectDeployerConfig: DeployerConfigBase
    {
        public string DatabaseProjectFileName { get; set; }
        public string BuildConfiguration { get; set; }
    }
}
