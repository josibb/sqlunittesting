using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using System.Xml;
using Data.Tools.UnitTesting.Equality;
using Data.Tools.UnitTesting.TestSetup;
using Data.Tools.UnitTesting.TestSetup.Sql;
using Data.Tools.UnitTesting.Utils;
using Data.Tools.UnitTesting.FluentApi;
using System.Collections.Generic;
using System.Linq;
using Data.Tools.UnitTesting.Result;

namespace ExampleDb.SSDT.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void FluentTest1()
        {
            var testResults = Setup.Default.CreateTest()
                .AddAction<SqlScriptAction>(a => a
                    .WithName("setupdata")
                    .WithConnection("ExampleDb-Test")
                    .WithSqlFromResource("XmlFiles.InsertTouAndCosts.sql")
                    .AddParameter<InputParameter>(p => p.WithDbType(DbType.DateTime).WithName("@dt").WithValue(new DateTime(2001, 06, 20)))
                    .AddParameter<OutputParameter>(p => p.WithDbType(DbType.Int32).WithName("@touid"))
                    .AddParameter<OutputParameter>(p => p.WithDbType(DbType.Int32).WithName("@costid"))
                )
                .AddAction<SqlScriptAction>(a => a
                    .WithName("test")
                    .WithConnection("ExampleDb-Test")
                    .WithSqlFromResource("XmlFiles.ExecP_AvgCost_Monthly.sql")
                    .AddParameter<LinkedInputParameter>(p => p.LinkTo("setupdata", "@touid"))
                    .AddParameter<LinkedInputParameter>(p => p.LinkTo("setupdata", "@costid"))
                )
                .Execute();

            SqlAssert.Equals(ActionResult.ReadFromResource("XmlFiles.TimeSerieResult.xml"), testResults["test"]);
            SqlAssert.MaxEllapsedSqlMilliseconds(testResults["test"], 100);
        }

        [TestMethod]
        public void SqlTest1()
        {
            var test = new Test();

            var sql = this.GetType().Assembly.GetResourceAsText("XmlFiles.InsertTouAndCosts.sql");

            var action = new SqlScriptAction { Name = "pretest", SqlScript = sql };
            var touid = new OutputParameter { DbType = DbType.Int32, Name = "@touid" };
            var costid = new OutputParameter { DbType = DbType.Int32, Name = "@costid" };
            var dt = new InputParameter { DbType = DbType.DateTime, Name = "@dt", Value = new DateTime(2001, 06, 20) };
            action.Parameters.Add(touid);
            action.Parameters.Add(costid);
            action.Parameters.Add(dt);
            action.ConnectionContext = Setup.Default.Connections["ExampleDb-Test"];
            test.Actions.Add(action);

            sql = this.GetType().Assembly.GetResourceAsText("XmlFiles.ExecP_AvgCost_Monthly.sql");
            action = new SqlScriptAction { Name = "test", SqlScript = sql };
            action.ConnectionContext = Setup.Default.Connections["ExampleDb-Test"];
            action.Parameters.Add(new LinkedInputParameter { LinkedParameter = touid });
            action.Parameters.Add(new LinkedInputParameter { LinkedParameter = costid });
            test.Actions.Add(action);

            var testResults = test.Execute();

            var expectedResult = ActionResult.ReadFromResource(Assembly.GetExecutingAssembly(), "XmlFiles.TimeSerieResult.xml");

            if (!testResults["test"].EqualsActionResult(expectedResult))
            {
                var dbResult = testResults["test"].Serialize();
                Assert.AreEqual(expectedResult.Serialize(), dbResult);
            }
        }
    }
}


