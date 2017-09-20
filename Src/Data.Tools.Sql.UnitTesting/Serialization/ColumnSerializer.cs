using System;
using System.Xml;
using Data.Tools.UnitTesting.Utils;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Serialization
{
    public class ColumnSerializer : IXmlSerializer<Column>
    {
        public Column Deserialize(XmlReader reader)
        {
            reader.ThrowIfNull("reader");

            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Column")
            {
                return new Column
                {
                    Name = reader["name"],
                    DbType = reader["dbType"],
                    InternalClrType = reader["clrType"]
                };
            }
            else
            {
                throw new InvalidOperationException("Reader not at a Column element");
            }
        }

        public void Serialize(XmlWriter writer, Column column)
        {
            writer.ThrowIfNull("writer");
            column.ThrowIfNull("column");

            var ex = column.Validate();
            if (ex != null)
                throw new InvalidOperationException("Column is invalid", ex);

            writer.WriteStartElement("Column");

            writer.WriteAttributeString("name", column.Name);
            writer.WriteAttributeString("dbType", column.DbType);
            writer.WriteAttributeString("clrType", column.InternalClrType);

            writer.WriteEndElement();
        }

        
    }
}