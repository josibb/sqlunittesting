using Data.Tools.UnitTesting.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Equality
{
    public static class ResultSetEqualityComparer
    {
        public static bool EqualResultSet(this ResultSet r1, ResultSet r2)
        {
            if (r1 == null)
                throw new ArgumentNullException("r1");

            if (r2 == null)
                throw new ArgumentNullException("r2");

            if (!r1.Schema.EqualsSchema(r2.Schema))
                return false;

            return r1.Rows.EqualRowCollections(r2.Rows);
        }
    }
}
