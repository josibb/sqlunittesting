using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Data.Tools.UnitTesting.Serialization
{
    public interface IXmlSerializer<TEntity>
    {
        TEntity Deserialize(XmlReader reader);
        void Serialize(XmlWriter writer, TEntity item);
    }
}
