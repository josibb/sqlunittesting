using Data.Tools.UnitTesting.TestSetup;
using Data.Tools.UnitTesting.TestSetup.Configuration;
using Data.Tools.UnitTesting.Utils;
using System;

namespace Data.Tools.UnitTesting.FluentApi
{
    public static class TestActionExtensions
    {
        public static TTestAction WithName<TTestAction>(this TTestAction action, string name) where TTestAction : TestAction
        {
            name.ThrowIfNull("name");

            action.Name = name;

            return action;
        }

        public static TTestAction WithConnection<TTestAction>(this TTestAction action, ConnectionContext context) where TTestAction : TestAction
        {
            context.ThrowIfNull("context");

            action.ConnectionContext = context;

            return action;
        }

        public static TTestAction WithConnection<TTestAction>(this TTestAction action, string connectionName) where TTestAction : TestAction
        {
            connectionName.ThrowIfNull("connectionName");

            action.Test.ThrowIfNull<InvalidCastException>("Cannot use this fluent method without creating the containing action fluently");
            action.Test.Setup.ThrowIfNull<InvalidOperationException>("Cannot use this fluent method without creating the containing test fluently");

            action.ConnectionContext = action.Test.Setup.Connections[connectionName];

            return action;
        }
    }


}
