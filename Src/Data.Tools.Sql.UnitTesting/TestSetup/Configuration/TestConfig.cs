using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Data.Tools.UnitTesting.Utils;
using System;
using System.Data.Common;

namespace Data.Tools.UnitTesting.TestSetup.Configuration
{
    public class TestConfig
    {
        private IList<ConnectionContext> connections2 = new List<ConnectionContext>();

        public IList<ConnectionContext> Connections2 { get => connections2; internal set { connections2 = value; } }


        //private IDictionary<string, ConnectionContext> connections = new Dictionary<string, ConnectionContext>();
        //public IDictionary<string, ConnectionContext> Connections { get => connections; internal set { connections = value; } }
    }
}

