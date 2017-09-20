using Data.Tools.UnitTesting;
using Data.Tools.UnitTesting.Result;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Tests.Utils
{
    public class TestDataReader : IDataReader
    {
        private IList<ResultSet> resultSets = new List<ResultSet>();
        public IList<ResultSet> ResultSets { get => resultSets; }

        private int ActiveResultSetIndex = 0;
        private int ActiveRowIndexInactiveResultSet = -1;

        public bool NextResult()
        {
            if (ActiveResultSetIndex >= ResultSets.Count - 1)
                return false;

            ActiveResultSetIndex++;
            ActiveRowIndexInactiveResultSet = -1;
            return true;
        }

        public bool Read()
        {
            var ar = GetActiveResultSet(false);
            if (ar == null)
                return false;

            if (ActiveRowIndexInactiveResultSet >= ar.Rows.Count - 1)
            {
                return false;
            }
            else
            {
                ActiveRowIndexInactiveResultSet++;
                return true;
            }
        }

        private ResultSet GetActiveResultSet(bool throwIfNull)
        {
            if (ActiveResultSetIndex < ResultSets.Count)
                return ResultSets[ActiveResultSetIndex];
            else
            {
                if (throwIfNull)
                    throw new InvalidOperationException("No active resultset");

                return null;
            }
        }

        private ResultSetRow GetActiveResultSetRow()
        {
            var ar = GetActiveResultSet(true);

            if (ActiveRowIndexInactiveResultSet < ar.Rows.Count)
            {
                return ar.Rows[ActiveRowIndexInactiveResultSet];
            }
            else
            {
                throw new InvalidOperationException("no rows in resultset");
            }
        }



        public string GetName(int i)
        {
            return GetActiveResultSet(true).Schema.Columns[i].Name;
        }

        public string GetDataTypeName(int i)
        {
            return GetActiveResultSet(true).Schema.Columns[i].DbType;
        }

        public Type GetFieldType(int i)
        {
            return GetActiveResultSet(true).Schema.Columns[i].ClrType;
        }

        public int FieldCount
        {
            get
            {
                var ar = GetActiveResultSet(false);
                if (ar == null)
                    return 0;
                else
                {
                    return ar.Schema.Columns.Count;
                }
            }
        }

        public object GetValue(int i)
        {
            return GetActiveResultSetRow()[this.GetName(i)];
        }







        public object this[int i] => throw new NotImplementedException();

        public object this[string name] => throw new NotImplementedException();

        public int Depth => throw new NotImplementedException();

        public bool IsClosed => throw new NotImplementedException();

        public int RecordsAffected => throw new NotImplementedException();

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }



        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }



        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }



        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }



        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }


        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }


    }
}
