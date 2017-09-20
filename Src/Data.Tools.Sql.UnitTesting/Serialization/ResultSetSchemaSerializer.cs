using System;
using System.Collections.Generic;
using System.Xml;
using Data.Tools.UnitTesting.Utils;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Serialization
{
    public class ResultSetSchemaSerializer : IXmlSerializer<ResultSetSchema>
    {
        private readonly IXmlSerializer<ColumnCollection> columnCollectionSerializer = new ColumnCollectionSerializer();

        public ResultSetSchema Deserialize(XmlReader reader)
        {
            reader.ThrowIfNull("reader");

            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Schema")
            {
                if (reader.ReadToDescendant("Columns"))
                {
                    var result = new ResultSetSchema
                    {
                        Columns = columnCollectionSerializer.Deserialize(reader)
                    };

                    reader.ReadEndElement();

                    return result;
                } else
                {
                    throw new InvalidOperationException("No element Columns found in Schema");
                    //return new ResultSetSchema();
                }
            } else
            {
                throw new InvalidOperationException("Reader not at a Schema element");
            }
        }

        public void Serialize(XmlWriter writer, ResultSetSchema item)
        {
            writer.ThrowIfNull("writer");
            item.ThrowIfNull("item");

            var ex = item.Validate();
            if (ex != null)
                throw new InvalidOperationException("ResultSetSchema is invalid", ex);

            writer.WriteStartElement("Schema");

            columnCollectionSerializer.Serialize(writer, item.Columns);

            writer.WriteEndElement();
        }
    }
}