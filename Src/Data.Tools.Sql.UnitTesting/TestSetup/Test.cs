using Data.Tools.UnitTesting.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Data.Tools.UnitTesting.Utils;
using System;
using System.Data.Common;
using System.Configuration;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.TestSetup
{
    public class Test
    {
        public IList<TestAction> Actions { get; private set; }

        public Test()
        {
            Actions = new List<TestAction>();
        }

        public IDictionary<string, ActionResult> Execute()
        {
            var results = new Dictionary<string, ActionResult>();

            foreach (var action in Actions)
            {
                var r = action.Execute();
                results[action.Name] = r;
            }

            return results;
        }

        internal Setup Setup { get; set; }
    }
}

