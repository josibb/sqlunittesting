using Data.Tools.UnitTesting.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Equality
{
    public static class ResultSetSchemaEqualityComparer
    {
        public static bool EqualsSchema(this ResultSetSchema schema1, ResultSetSchema schema2)
        {
            if (schema2 == null)
                throw new ArgumentNullException("schema2");

            if (schema1.Columns.Count != schema2.Columns.Count)
                return false;

            for (var t = 0; t < schema1.Columns.Count; t++)
            {
                if (!schema1.Columns[t].EqualsColumn(schema2.Columns[t]))
                    return false;
            }

            return true;
        }
    }
}
