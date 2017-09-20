using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Tools.UnitTesting.Utils;
using System;

namespace Data.Tools.UnitTesting.TestSetup.Configuration
{
    public class ConnectionContext
    {
        public string Name { get; set; }

        public string ProviderName { get; set; }

        public DbProviderFactory Provider { get; set; }

        public string ConnectionString { get; set; }

        public IDbConnection CreateConnection()
        {
            var c = Provider.CreateConnection();
            try
            {
                c.ConnectionString = ConnectionString;
                return c;
            }
            catch
            {
                c.Dispose();
                throw;
            }
        }

        public string GetConnectionStringForDatabaseFromConnectionContext(string dbName)
        {
            var cb = Provider.CreateConnectionStringBuilder();
            cb.ConnectionString = ConnectionString;

            cb["Initial Catalog"] = dbName;
            
            return cb.ConnectionString;
        }

        public DatabaseDeployment Deployment { get; set; }
    }
}
