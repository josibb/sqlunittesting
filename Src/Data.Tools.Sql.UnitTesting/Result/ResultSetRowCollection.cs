using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Tools.UnitTesting.Utils;

namespace Data.Tools.UnitTesting.Result
{
    public class ResultSetRowCollection : List<ResultSetRow>
    {
        public static ResultSetRowCollection CreateFromReader(IDataReader reader)
        {
            reader.ThrowIfNull("rader");

            var rows = new ResultSetRowCollection();

            while (reader.Read())
            {
                rows.Add(ResultSetRow.CreateFromReader(reader));
            }

            return rows;
        }

        public virtual Exception Validate(ResultSetSchema schema, bool recursive = false)
        {
            schema.ThrowIfNull("schema");

            if (recursive)
            {
                foreach (var row in this)
                {
                    var ex = row.Validate(schema);
                    if (ex != null)
                        return new InvalidOperationException("Found invalid row(s)", ex);
                }
            }

            return null;
        }
    }
}
