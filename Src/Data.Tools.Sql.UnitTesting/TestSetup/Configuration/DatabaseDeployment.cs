namespace Data.Tools.UnitTesting.TestSetup.Configuration
{
    public class DatabaseDeployment
    {
        public ConnectionContext ConnectionContext { get; set; }

        public IDatabaseDeployer DatabaseDeployer { get; set; }

        public DeployerConfigBase DeployerConfig { get; set; }

        public bool CreateUniqueDatabaseName { get; set; }

        public bool DropDatabaseOnExit { get; set; }
    }
}

