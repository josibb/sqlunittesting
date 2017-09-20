using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Data.Tools.UnitTesting.Serialization
{
    public interface IContextXmlSerializer<TEntity, TReadContext, TWriteContext>
    {
        TEntity Deserialize(XmlReader reader, TReadContext context);
        void Serialize(XmlWriter writer, TEntity item, TWriteContext context);
    }
}
