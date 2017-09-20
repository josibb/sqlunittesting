using Data.Tools.UnitTesting.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Equality
{
    public static class RowCollectionEqualityComparer
    {
        public static bool EqualRowCollections(this ResultSetRowCollection c1, ResultSetRowCollection c2)
        {
            if (c2 == null)
                throw new ArgumentNullException("c2");

            // check for equal number of rows
            if (c1.Count != c2.Count)
                return false;

            for (var t = 0; t < c1.Count; t++)
            {
                if (!c1[t].EqualRows(c2[t]))
                    return false;
            }

            return true;
        }
    }
}
