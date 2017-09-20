using Data.Tools.UnitTesting.TestSetup;
using Data.Tools.UnitTesting.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.FluentApi
{

    public static class TestExtensions
    {
        public static Test AddAction<TTestAction>(this Test test, Action<TTestAction> action) where TTestAction : TestAction
        {
            action.ThrowIfNull("action");

            var a = Activator.CreateInstance<TTestAction>();
            a.Test = test;
            test.Actions.Add(a);
            action(a);

            return test;
        }

        public static Test Run(this Test test)
        {
            test.Execute();
            return test;
        }
    }

}
