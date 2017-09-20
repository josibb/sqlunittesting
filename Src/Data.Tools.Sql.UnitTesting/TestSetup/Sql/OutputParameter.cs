using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Data.Tools.UnitTesting.TestSetup.Sql
{
    public class OutputParameter : SqlScriptParameter
    {
        public override ParameterDirection Direction
        {
            get { return ParameterDirection.Output; }
        }

        public override object Value { get; set; }
        //private object value;
        //public override object Value { get { return value; } set { this.value = value; }
    }
}
