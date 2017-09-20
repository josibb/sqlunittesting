using System;
using System.Data;

namespace Data.Tools.UnitTesting.TestSetup.Sql
{
    public abstract class SqlScriptParameter
    {
        public virtual string Name { get; set; }
        public virtual DbType DbType { get; set; }
        public abstract ParameterDirection Direction { get; }

        public virtual Object Value { get; set; }

        internal SqlScriptAction SqlScriptAction { get; set; }
    }
}
