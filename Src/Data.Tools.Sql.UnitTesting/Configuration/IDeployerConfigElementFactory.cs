using System.Xml;

namespace Data.Tools.UnitTesting.Configuration
{
    public interface IDeployerConfigElementFactory
    {
        DeployerConfigElementBase CreateFromReader(XmlReader reader);
    }
}
