using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Data.Tools.UnitTesting.Serialization
{
    public interface IXmlSerializable<TEntity>
    {
        void ReadXml(XmlReader reader, TEntity item);

        void WriteXml(XmlWriter writer, TEntity item);
    }

   

    
}
