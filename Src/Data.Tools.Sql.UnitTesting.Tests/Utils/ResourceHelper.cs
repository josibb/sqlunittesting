using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Tests.Utils
{
    public class ResourceHelper
    {
        public static Stream GetResource(string xmlFile)
        {
            var resourceName = $"Data.Tools.UnitTesting.Tests.XmlFiles.{xmlFile}";
            var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (s == null)
                throw new InvalidOperationException($"Cannot find embedded resource '{resourceName}'");

            return s;
        }
    }
}
