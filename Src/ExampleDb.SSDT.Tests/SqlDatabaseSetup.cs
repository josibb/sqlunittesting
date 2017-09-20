using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Tools.Schema.Sql.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting.TestSetup;
using Data.Tools.UnitTesting.TestSetup.Sql;
using System.Data.SqlClient;
using Data.Tools.UnitTesting.Configuration;
using Data.Tools.UnitTesting.FluentApi;

namespace ExampleDb.SSDT.Tests
{
    [TestClass()]
    public class SqlDatabaseSetup
    {

        [AssemblyInitialize()]
        public static void InitializeAssembly(TestContext ctx)
        {
            Setup.InitializeDefault(ConfigurationFileTestConfigFactory.CreateFromConfigurationFile());

            SqlAssert.AssertionHandler = AssertionHandler;
        }

        private static void AssertionHandler(string assertionName, string message)
        {
            throw new AssertFailedException($"{assertionName} failed; ${message}");
        }

        [AssemblyCleanup]
        public static void TearDown()
        {
            Setup.TeardownDefault();
        }

    }
}
