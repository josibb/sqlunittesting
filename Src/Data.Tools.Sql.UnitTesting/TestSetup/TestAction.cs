
using Data.Tools.UnitTesting.Configuration;
using Data.Tools.UnitTesting.Result;
using Data.Tools.UnitTesting.TestSetup.Configuration;
using System;

namespace Data.Tools.UnitTesting.TestSetup
{
    public abstract class TestAction
    {
        public string Name { get; set; }

        public ConnectionContext ConnectionContext { get; set; }

        public abstract ActionResult Execute();

        public static TTestAction Create<TTestAction>() where TTestAction: TestAction
        {
            return Activator.CreateInstance<TTestAction>();
        }

        internal Test Test { get; set; }
    }
}

