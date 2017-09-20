using System;
using System.IO;

namespace Data.Tools.UnitTesting.Tests.Utils
{
    public class TemporaryFile : TemporaryFileBase<TemporaryFile> { }

    public class TemporaryFileBase<TTemporaryFileBase> : IDisposable 
        where TTemporaryFileBase: TemporaryFileBase<TTemporaryFileBase>
    {
        public FileInfo File { get; private set; }

        public void Dispose()
        {
            if (File.Exists)
            {
                File.Delete();
            }
        }

        private void CopyResourceToFile(string resourceName, FileInfo file)
        {
            var assemblyName = this.GetType().Assembly.GetName().Name;
            var fullResourceName = $"{assemblyName}.{resourceName}";

            using (var s = this.GetType().Assembly.GetManifestResourceStream(fullResourceName))
            {
                if (s == null)
                    throw new InvalidOperationException($"Cannot find resource '{fullResourceName}'");

                var fileName = GetUniqueFileName(resourceName);

                using (var f = System.IO.File.Create(file.FullName))
                {
                    s.Seek(0, SeekOrigin.Begin);
                    s.CopyTo(f);
                }
            }
        }

        private static string GetUniqueFileName(string baseName)
        {
            var t = 0;
            while (System.IO.File.Exists($"{baseName}{t}"))
                t++;

            return baseName + t;
        }


        public static TTemporaryFileBase OpenFromResource(string resourceName)
        {
            var result = Create(resourceName);
            result.CopyResourceToFile(resourceName, result.File);
            return result;
        }

        public static TTemporaryFileBase Create(string baseFileName)
        {
            var result = (TTemporaryFileBase)Activator.CreateInstance<TTemporaryFileBase>();

            var fileName = GetUniqueFileName(baseFileName);
            result.File = new FileInfo(fileName);

            return result;
        }
    }
}
