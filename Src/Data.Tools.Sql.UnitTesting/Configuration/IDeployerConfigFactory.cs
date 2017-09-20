using Data.Tools.UnitTesting.TestSetup.Configuration;

namespace Data.Tools.UnitTesting.Configuration
{
    public interface IDeployerConfigFactory
    {
        DeployerConfigBase CreateFromElement(DeployerConfigElementBase deployerElement);
    }

}
