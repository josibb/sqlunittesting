using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Tools.UnitTesting.Utils;
using System.Data;

namespace Data.Tools.UnitTesting.Result
{
    public class ResultSetRow : Dictionary<string, object>
    {
        public static ResultSetRow CreateFromReader(IDataReader reader)
        {
            reader.ThrowIfNull("reader");

            var row = new ResultSetRow();

            for (var t = 0; t < reader.FieldCount; t++)
            {
                row[reader.GetName(t)] = reader.GetValue(t);
            }

            return row;
        }

        /// <summary>
        /// Validates row names and value types against schema
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public virtual Exception Validate(ResultSetSchema schema)
        {
            schema.ThrowIfNull("schema");

            var ex = schema.Validate(true);
            if (ex != null)
                return new InvalidOperationException("Invalid schema", ex);

            ex = CheckForEmptyValuesWithoutColumnName();
            if (ex != null)
                return ex;

            ex = CheckForInvalidCOlumnNames(schema);
            if (ex != null)
                return ex;

            ex = CheckForInvalidValueTypes(schema);
            if (ex != null)
                return ex;

            return null; // nothing to validate here TODO: validate against given schema!!
        }

        private Exception CheckForInvalidValueTypes(ResultSetSchema schema)
        {
            var invalidColumn = this.Where(kv => schema.Columns.Any(column => column.Name == kv.Key && kv.Value != null && kv.Value != DBNull.Value && kv.Value.GetType() != column.ClrType))
                .Select(kv => new { ColumnName = kv.Key, ValueType = kv.Value.GetType().FullName }).FirstOrDefault();

            if (invalidColumn != null)
                return new InvalidOperationException($"Value for '{ invalidColumn.ColumnName}' is of type '{invalidColumn.ValueType}' while schema defines type '{schema.GetColumn(invalidColumn.ColumnName).ClrType.FullName}'");
            
            return null;
        }

        private Exception CheckForInvalidCOlumnNames(ResultSetSchema schema)
        {
            var columnName = this.Keys.Where(key => !schema.Columns.Any(column => column.Name == key)).FirstOrDefault();
            if (columnName != null)
                return new InvalidOperationException($"Column '{columnName}' is not defined in schema");

            return null;
        }

        private Exception CheckForEmptyValuesWithoutColumnName()
        {
            if (this.Keys.Where(a => string.IsNullOrWhiteSpace(a)).FirstOrDefault() != null)
                return new InvalidOperationException("Row contains a value without column name (null/empty)");
            else return null;
        }
    }
}
