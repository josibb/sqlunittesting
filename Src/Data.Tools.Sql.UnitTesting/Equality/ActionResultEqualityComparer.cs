using Data.Tools.UnitTesting;
using Data.Tools.UnitTesting.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Equality
{
    public static class ActionResultEqualityComparer
    {
        public static bool EqualsActionResult(this ActionResult result1, ActionResult result2)
        {
            if (result1 == null)
                throw new ArgumentNullException("result1");

            if (result2 == null)
                throw new ArgumentNullException("result2");

            if (result1.ResultSets == null)
                throw new InvalidOperationException("No resultsets found in result1");

            if (result2.ResultSets == null)
                throw new InvalidOperationException("No resultsets found in result2");

            if (result1.ResultSets.Count != result2.ResultSets.Count)
                return false;

            for (var t = 0; t < result1.ResultSets.Count; t++)
            {
                if (!result1.ResultSets[t].EqualResultSet(result2.ResultSets[t]))
                    return false;
            }

            return true;
        }
    }
}
