using Data.Tools.UnitTesting.TestSetup.Sql;
using System.Data;

namespace Data.Tools.UnitTesting.FluentApi
{
    public static class SqlParameterExtensions
    {
        public static SqlScriptParameter WithDbType(this SqlScriptParameter parameter, DbType dbType)
        {
            parameter.DbType = dbType;
            return parameter;
        }

        public static SqlScriptParameter WithName(this SqlScriptParameter parameter, string name)
        {
            parameter.Name = name;
            return parameter;
        }

        public static SqlScriptParameter WithValue(this SqlScriptParameter parameter, object value)
        {
            parameter.Value = value;
            return parameter;
        }
    }


}
