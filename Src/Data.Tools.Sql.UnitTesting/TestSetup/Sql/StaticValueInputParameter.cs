using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.TestSetup.Sql
{
    public class StaticValueInputParameter : InputParameter
    {
        public override object Value { get; set; }
    }
}
