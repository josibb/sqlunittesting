using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Data.Tools.UnitTesting.Utils;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Serialization
{
    public class ResultSetSerializer : IXmlSerializer<ResultSet>
    {
        private IXmlSerializer<ResultSetSchema> schemaSerializer = new ResultSetSchemaSerializer();
        private IContextXmlSerializer<ResultSetRowCollection, ResultSetRowCollectionSerializerContext, ResultSetRowCollectionSerializerContext> rowCollectionSerializer = new ResultSetRowCollectionSerializer();

        public ResultSet Deserialize(XmlReader reader)
        {
            reader.ThrowIfNull("reader");

            var resultSet = new ResultSet();

            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ResultSet")
            {
                resultSet.Schema = ReadSchema(reader);
                resultSet.Rows = ReadRows(reader, resultSet.Schema);
            }
            else
            {
                throw new InvalidOperationException("Reader is not at a ResultSet element");
            }

            return resultSet;
        }

        private ResultSetSchema ReadSchema(XmlReader reader)
        {
            if (!reader.ReadToDescendant("Schema"))
                throw new InvalidOperationException("Cannot find Schema in ResultSet");

            return schemaSerializer.Deserialize(reader); //ReadSchema(reader);
        }

        private ResultSetRowCollection ReadRows(XmlReader reader, ResultSetSchema schema)
        {
            return rowCollectionSerializer.Deserialize(reader, new ResultSetRowCollectionSerializerContext { Schema = schema });
        }

        public void Serialize(XmlWriter writer, ResultSet resultSet)
        {
            writer.ThrowIfNull("writer");
            resultSet.ThrowIfNull("resultSet");

            var ex = resultSet.Validate();
            if (ex != null)
                throw new InvalidOperationException("ResultSet is invalid", ex);

            writer.WriteStartElement("ResultSet");

            schemaSerializer.Serialize(writer, resultSet.Schema);
            rowCollectionSerializer.Serialize(writer, resultSet.Rows, new ResultSetRowCollectionSerializerContext { Schema = resultSet.Schema });

            writer.WriteEndElement();
        }
    }
}
