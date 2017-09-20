using Data.Tools.UnitTesting.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Equality
{
    public static class RowEqualityComparer
    {
        public static bool EqualRows(this ResultSetRow r1, ResultSetRow r2)
        {
            if (r1 == null)
                throw new ArgumentNullException("r1");

            if (r2 == null)
                throw new ArgumentNullException("r2");

            if (r1.Count != r2.Count)
                return false;

            foreach (var kr1 in r1)
            {
                var value1 = kr1.Value;

                object value2;
                if (!r2.TryGetValue(kr1.Key, out value2))
                    return false;

                if (!EqualValues(kr1.Value, value2))
                    return false;
            }

            return true;
        }

        public static bool EqualValues(object value1, object value2)
        {
            // Dbnull equals null
            if (!
                        (
                        (value1 == null && value2 == DBNull.Value) ||
                        (value1 == DBNull.Value && value2 == null)
                        ))
            {
                if (!object.Equals(value1, value2))
                {
                    return false;
                }
            }

            return true;
        }
    }

    
}
