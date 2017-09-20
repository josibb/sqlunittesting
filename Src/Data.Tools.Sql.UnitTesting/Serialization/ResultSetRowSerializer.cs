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
    public class ResultSetRowSerializer : IContextXmlSerializer<ResultSetRow, ResultSetRowSerializerContext, ResultSetRowSerializerContext>
    {
        public ResultSetRow Deserialize(XmlReader reader, ResultSetRowSerializerContext context)
        {
            reader.ThrowIfNull("reader");
            context.ThrowIfNull("context");
            context.Schema.ThrowIfNull("context.Schema");

            if (reader.LocalName != "Row")
                throw new InvalidOperationException("Reader not at Row element");

            var row = new ResultSetRow();

            if (reader.Read())
            {
                while (reader.LocalName != "Row")
                {
                    var columnName = reader.LocalName;
                    var column = context.Schema.GetColumn(columnName);

                    row[columnName] = reader.ReadElementContentAs(column.ClrType, null);
                }
            }

            AddNonExistingColumnValuesToRowsWithDbNullValueFromSchema(row, context.Schema);

            return row;
        }

        public static void AddNonExistingColumnValuesToRowsWithDbNullValueFromSchema(ResultSetRow row, ResultSetSchema schema)
        {
            foreach (var nullColumn in schema.Columns.Where(a => !row.ContainsKey(a.Name)))
            {
                row[nullColumn.Name] = DBNull.Value;
            }
        }

        public void Serialize(XmlWriter writer, ResultSetRow row, ResultSetRowSerializerContext context)
        {
            writer.ThrowIfNull("writer");
            row.ThrowIfNull("row");
            context.ThrowIfNull("context");
            context.Schema.ThrowIfNull("context.Schema");

            var ex = row.Validate(context.Schema);
            if (ex != null)
                throw new InvalidOperationException("Row is invalid", ex);

            writer.WriteStartElement("Row");

            foreach (var kv in row)
            {
                if (kv.Value != null && kv.Value != DBNull.Value)
                {
                    writer.WriteStartElement(kv.Key);
                    writer.WriteValue(kv.Value);
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }
    }

    public class ResultSetRowSerializerContext
    {
        public ResultSetSchema Schema { get; set; }
    }
}
