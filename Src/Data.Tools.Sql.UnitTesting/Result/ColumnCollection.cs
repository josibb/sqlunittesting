using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Result
{
    public class ColumnCollection : List<Column>
    {
        public virtual Exception Validate(bool checkProperties = false)
        {
            if (checkProperties)
            {
                foreach (var column in this)
                {
                    var ex = column.Validate();
                    if (ex != null)
                        return ex;
                }
            }

            return null;
        }
    }
}
