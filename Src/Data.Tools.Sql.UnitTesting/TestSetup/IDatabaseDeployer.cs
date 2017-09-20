using Data.Tools.UnitTesting.TestSetup.Configuration;
using System.Xml;

namespace Data.Tools.UnitTesting.TestSetup
{
    public interface IDatabaseDeployer
    {
        void DeployDatabase(DeployerConfigBase config, UnitTesting.TestSetup.Configuration.ConnectionContext connection);

        void DropDatabase(ConnectionContext connection);

        string GetNewUniqueDatabaseName(ConnectionContext connection);
    }
}

