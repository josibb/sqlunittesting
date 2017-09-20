using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Data.Tools.UnitTesting.TestSetup.Sql
{
    public class InputParameter : SqlScriptParameter
    {
        //public override object Value { get; set; }

        public override ParameterDirection Direction
        {
            get { return ParameterDirection.Input; }
        }
    }
}
