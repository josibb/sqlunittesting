using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Tools.UnitTesting.Utils;

namespace Data.Tools.UnitTesting.Result
{
    public class ResultSetSchema
    {
        private ColumnCollection columns = new ColumnCollection();
        public ColumnCollection Columns { get => columns; set => columns = value; }

        public Column GetColumn(string name)
        {
            var result = Columns.FirstOrDefault(a => a.Name == name);
            if (result == null)
                throw new InvalidOperationException("Cannot find column '" + name + "' in schema");

            return result;
        }

        public virtual Exception Validate(bool checkProperties = false)
        {
            if (columns == null)
                return new InvalidOperationException("Columns is null");

            if (checkProperties)
            {
                var ex = columns.Validate(checkProperties);
                if (ex != null)
                    return new InvalidOperationException("Schema validation failed", ex);
            }

            return null;
        }

        public static ResultSetSchema CreateFromReader(IDataReader reader)
        {
            reader.ThrowIfNull("reader");

            var result = new ResultSetSchema();

            for (var t = 0; t < reader.FieldCount; t++)
            {
                result.Columns.Add(Column.CreateFromReader(reader, t));
            }
            
            return result;
        }
    }
}
