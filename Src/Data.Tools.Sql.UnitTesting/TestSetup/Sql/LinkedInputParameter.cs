using Data.Tools.UnitTesting.Utils;
using System;
using System.Data;

namespace Data.Tools.UnitTesting.TestSetup.Sql
{
    public class LinkedInputParameter : InputParameter
    {
        public SqlScriptParameter LinkedParameter { get; set;  }

        protected SqlScriptParameter GetLinkedParameter()
        {
            LinkedParameter.ThrowIfNull<InvalidOperationException>("LinkedParameter is not set");
            return LinkedParameter;
        }

        public override DbType DbType
        {
            get { return GetLinkedParameter().DbType; }
            set { throw new InvalidOperationException("Not supported"); } // must be of same type as input
        }

        public override string Name
        {
            get
            {
                if (base.Name == null)
                    return GetLinkedParameter().Name;
                else
                    return base.Name;
            }
            set { base.Name = value; }
        }

        public override object Value
        {
            get { return GetLinkedParameter().Value; }
            set { throw new InvalidOperationException("Not supported"); }
        }

    }
}

