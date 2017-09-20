using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Utils
{
    public static class ConfigurationElementExtensions
    {
        public static bool IsAvailable(this ConfigurationElement element)
        {
            return (element != null && element.ElementInformation != null && element.ElementInformation.IsPresent);
        }
    }
}
