using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Utils
{
    public static class Resources
    {
        public static Stream GetResourceStreamFromAssembly(Assembly assembly, string resourceName)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            if (resourceName == null)
                throw new ArgumentNullException("resourceName");

            var fullResourceName = $"{assembly.GetName().Name}.{resourceName}";

            var s = assembly.GetManifestResourceStream(fullResourceName);
            if (s == null)
                throw new InvalidOperationException($"Cannot find embedded resource '{fullResourceName}'");

            return s;
        }

        public static string GetResourceAsText(this Assembly assembly, string resourceName)
        {
            assembly.ThrowIfNull("assembly");
            resourceName.ThrowIfNull("resourceName");

            using (var s = GetResourceStreamFromAssembly(assembly, resourceName))
            {
                using (var tr = new StreamReader(s))
                {
                    return tr.ReadToEnd();
                }
            }
        }
    }
}
