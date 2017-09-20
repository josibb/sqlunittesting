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
    public class ResultSetRowCollectionSerializer : IContextXmlSerializer<ResultSetRowCollection, ResultSetRowCollectionSerializerContext, ResultSetRowCollectionSerializerContext>
    {
        private readonly IContextXmlSerializer<ResultSetRow, ResultSetRowSerializerContext, ResultSetRowSerializerContext> rowSerializer = new ResultSetRowSerializer();

        public ResultSetRowCollection Deserialize(XmlReader reader, ResultSetRowCollectionSerializerContext context)
        {
            reader.ThrowIfNull("reader");
            context.ThrowIfNull("context");
            context.Schema.ThrowIfNull("context.Schema");

            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Rows")
            {
                var rows = new ResultSetRowCollection();

                if (reader.ReadToDescendant("Row"))
                {
                    do
                    {
                        rows.Add(rowSerializer.Deserialize(reader, new ResultSetRowSerializerContext { Schema = context.Schema }));

                    } while (reader.ReadToNextSibling("Row"));
                }

                reader.Read();// read end of element

                return rows;
            }
            else
            {
                throw new InvalidOperationException("Reader not at Rows elelement");
            }
        }

        public void Serialize(XmlWriter writer, ResultSetRowCollection rows, ResultSetRowCollectionSerializerContext context)
        {
            writer.ThrowIfNull("writer");
            rows.ThrowIfNull("rows");
            context.ThrowIfNull("context");
            context.Schema.ThrowIfNull("context.Schema");

            var ex = rows.Validate(context.Schema);
            if (ex != null)
                throw new InvalidOperationException("Rows are invalid", ex);

            writer.WriteStartElement("Rows");

            for (var t = 0; t < rows.Count; t++)
            {
                rowSerializer.Serialize(writer, rows[t], new ResultSetRowSerializerContext { Schema = context.Schema });
            }

            writer.WriteEndElement();
        }
    }

    public class ResultSetRowCollectionSerializerContext
    {
        public ResultSetSchema Schema {get; set;}
    }
}
