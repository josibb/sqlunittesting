using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Data.Tools.UnitTesting.Utils;

namespace Data.Tools.UnitTesting.Result
{
    public class ResultSet
    {
        private ResultSetSchema schema = new ResultSetSchema();
        public ResultSetSchema Schema { get => schema; set => schema = value; }

        private ResultSetRowCollection rows = new ResultSetRowCollection();

        public ResultSetRowCollection Rows { get => rows; set => rows = value; }

        public virtual Exception Validate(bool recursive = false)
        {
            if (schema == null)
                return new InvalidOperationException("Schema is null");

            if (rows == null)
                return new InvalidOperationException("Rows is null");

            if (recursive)
            {
                Exception ex;

                ex = schema.Validate(recursive);
                if (ex != null)
                    return new InvalidOperationException("Schema is invalid", ex); 

                ex = Rows.Validate(schema, recursive);
                if (ex != null)
                    return new InvalidOperationException("Rows are invalid", ex);
            }

            return null;
        }

        /// <summary>
        /// Creates a resultset from reader
        /// If there are no columns in the schema (fieldcount = 0), null is returned
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ResultSet CreateFromReader(IDataReader reader)
        {
            reader.ThrowIfNull("reader");

            var schema = ResultSetSchema.CreateFromReader(reader);
            if (schema.Columns.Count == 0)
                return null;

            return new ResultSet
            {
                Schema = schema,
                Rows = ResultSetRowCollection.CreateFromReader(reader)
            };
        }

        
    }
}
