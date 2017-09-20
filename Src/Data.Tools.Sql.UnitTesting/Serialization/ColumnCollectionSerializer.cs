using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Data.Tools.UnitTesting.Utils;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Serialization
{
    public class ColumnCollectionSerializer : IXmlSerializer<ColumnCollection>
    {
        private readonly IXmlSerializer<Column> columnSerializer = new ColumnSerializer();

        public ColumnCollection Deserialize(XmlReader reader)
        {
            reader.ThrowIfNull("reader");

            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Columns")
            {
                var columns = new ColumnCollection();

                if (reader.ReadToDescendant("Column"))
                {
                    do
                    {
                        columns.Add(columnSerializer.Deserialize(reader));
                    }
                    while (reader.ReadToNextSibling("Column"));
                }

                reader.Read(); // read end of node (cannot use readendelement because if there are 0 rows, there might not be an end elememt (/>)

                return columns;
            } else
            {
                throw new InvalidOperationException("Reader not at a Columns element");
            }
        }

        public void Serialize(XmlWriter writer, ColumnCollection columnCollection)
        {
            writer.ThrowIfNull("writer");
            columnCollection.ThrowIfNull("columnCollection");

            var ex = columnCollection.Validate();
            if (ex != null)
                throw new InvalidOperationException("ColumnCollection is invalid", ex);

            writer.WriteStartElement("Columns");
            for (var t = 0; t < columnCollection.Count; t++)
            {
                columnSerializer.Serialize(writer, columnCollection[t]);
            }
            writer.WriteEndElement();

        }
    }
}
