using Data.Tools.UnitTesting.Result;
using Data.Tools.UnitTesting.TestSetup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Data.Tools.UnitTesting.TestSetup.Sql
{
    public class SqlScriptAction : TestAction
    {
        public string SqlScript { get; set; }
        public IList<SqlScriptParameter> Parameters { get; private set; }

        public SqlScriptAction()
        {
            Parameters = new List<SqlScriptParameter>();
        }

        public override ActionResult Execute()
        {
            using (var cn = ConnectionContext.CreateConnection())
            {
                cn.Open();

                using (var cm = cn.CreateCommand())
                {
                    cm.CommandText = SqlScript;

                    var outputPars = new Dictionary<SqlScriptParameter, IDbDataParameter>();

                    foreach (var par in Parameters)
                    {
                        var parameter = cm.CreateParameter();
                        parameter.ParameterName = par.Name;
                        parameter.DbType = par.DbType;
                        parameter.Direction = par.Direction;

                        switch (par.Direction)
                        {
                            case ParameterDirection.Input:
                                parameter.Value = par.Value;
                                break;
                            case ParameterDirection.Output:
                                outputPars[par] = parameter;
                                break;
                            default:
                                throw new InvalidOperationException("notsupported");
                        }

                        cm.Parameters.Add(parameter);
                    }

                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    using (var dr = cm.ExecuteReader())
                    {
                        stopwatch.Stop();

                        var result = ActionResult.CreateFromReader(dr);

                        result.EllapsedSqlMilliseconds = stopwatch.ElapsedMilliseconds;

                        foreach (var par in outputPars)
                        {
                            par.Key.Value = par.Value.Value;
                        }

                        return result;
                    }

                }
            }
        }
    }
}

