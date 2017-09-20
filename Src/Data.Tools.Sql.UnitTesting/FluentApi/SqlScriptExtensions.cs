using Data.Tools.UnitTesting.TestSetup.Sql;
using Data.Tools.UnitTesting.Utils;
using System;
using System.Reflection;

namespace Data.Tools.UnitTesting.FluentApi
{
    public static class SqlScriptExtensions
    {
        public static SqlScriptAction AddParameter<TParameter>(this SqlScriptAction scriptAction, Action<TParameter> action) where TParameter : SqlScriptParameter
        {
            var parameter = Activator.CreateInstance<TParameter>();
            parameter.SqlScriptAction = scriptAction;
            scriptAction.Parameters.Add(parameter);
            action(parameter);

            return scriptAction;
        }

        public static SqlScriptAction WithSql(this SqlScriptAction scriptAction, string sql)
        {
            scriptAction.SqlScript = sql;
            return scriptAction;
        }

        public static SqlScriptAction WithSqlFromResource(this SqlScriptAction scriptAction, string resourceName)
        {
            return scriptAction.WithSqlFromResource(Assembly.GetCallingAssembly(), resourceName);
        }

        public static SqlScriptAction WithSqlFromResource(this SqlScriptAction scriptAction, Assembly assembly, string resourceName)
        {
            scriptAction.SqlScript = Resources.GetResourceAsText(assembly, resourceName);
            return scriptAction;
        }
    }


}
