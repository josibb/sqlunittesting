using Data.Tools.UnitTesting.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Equality
{
    public static class ColumnEqualityComparer
    {
        public static bool EqualsColumn(this Column c1, Column c2)
        {
            if (c2 == null)
                throw new ArgumentNullException("c2");

            return
                object.Equals(c1.Name, c2.Name) &&
                object.Equals(c1.DbType, c2.DbType) &&
                object.Equals(c1.ClrType, c2.ClrType);
        }
    }
}
