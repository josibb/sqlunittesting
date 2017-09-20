using Data.Tools.UnitTesting.TestSetup.Sql;
using Data.Tools.UnitTesting.Utils;
using System;
using System.Data;
using System.Linq;

namespace Data.Tools.UnitTesting.FluentApi
{
    public static class LinkedInputParameterExtensions
    {
        public static LinkedInputParameter LinkTo(this LinkedInputParameter parameter, SqlScriptParameter linkToParameter)
        {
            parameter.LinkedParameter = linkToParameter;
            return parameter;
        }

        public static LinkedInputParameter LinkTo(this LinkedInputParameter parameter, string parameterName)
        {
            parameter.SqlScriptAction.ThrowIfNull<InvalidOperationException>("Cannot use this method without creating the parameter fluently");

            var sourceParameter = (from p in parameter.SqlScriptAction.Parameters where p.Name == parameterName select p).FirstOrDefault();
            sourceParameter.ThrowIfNull<InvalidOperationException>($"Cannot find parameter '{parameterName}' in test");

            parameter.LinkedParameter = sourceParameter;
            return parameter;
        }

        public static LinkedInputParameter LinkTo(this LinkedInputParameter parameter, string actionName, string parameterName)
        {
            parameter.SqlScriptAction.ThrowIfNull<InvalidOperationException>("Cannot use this method without creating the parameter fluently");
            parameter.SqlScriptAction.Test.ThrowIfNull<InvalidOperationException>("Cannot use this method without creating the action fluently");

            var action = (SqlScriptAction)(from a in parameter.SqlScriptAction.Test.Actions where a.GetType() == typeof(SqlScriptAction) && a.Name == actionName select a).FirstOrDefault();
            action.ThrowIfNull<InvalidOperationException>($"Cannot find action '{actionName}' in test");

            var sourceParameter = (from p in action.Parameters where p.Name == parameterName select p).FirstOrDefault();
            sourceParameter.ThrowIfNull<InvalidOperationException>($"Cannot find parameter '{parameterName}' in action '{actionName}'");

            parameter.LinkedParameter = sourceParameter;
            return parameter;
        }
    }


}
