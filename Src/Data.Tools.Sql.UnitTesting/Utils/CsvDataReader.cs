using Data.Tools.UnitTesting.Result;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Utils
{
    public class CsvData
    {
        public string[] ColumnNames { get; set; }

        public string[][] RowValues { get; set; }

        public CultureInfo CultureInfo { get; set; }
    }

    public class CsvDataReader: IDataReader
    {
        public CsvData Data { get; private set; }

        public ColumnCollection SchemaColumns { get; private set; }

        public string NullValue { get; private set; }

        private int rowIndex = -1;

        public CsvDataReader(CsvData data, ColumnCollection schemaColumns, string nullValue)
        {
            data.ThrowIfNull("data");
            schemaColumns.ThrowIfNull("schemaColumns");
            nullValue.ThrowIfNull("nullValue");

            data.ColumnNames.ThrowIfNull<InvalidOperationException>("data.ColumnNames is null");
            data.RowValues.ThrowIfNull<InvalidOperationException>("data.RowValues is null");
            data.CultureInfo.ThrowIfNull<InvalidOperationException>("data.CultureInfo is null");

            Data = data;
            SchemaColumns = schemaColumns;
            NullValue = nullValue;

            CheckColumnAvailableInSchema(Data.ColumnNames, SchemaColumns);
        }

        internal void CheckColumnAvailableInSchema(string[] dataColumnNames, ColumnCollection schemaColumns)
        {
            var columnsNotInSchema = schemaColumns.Where(c1 => !dataColumnNames.Any(c2 => c2 == c1.Name));

            if (columnsNotInSchema.Count() > 0)
            {
                throw new InvalidOperationException($"The columns '{columnsNotInSchema}' are not defined in the schema");
            }
        }

        internal static object ParseValue(Type type, IConvertible value, CultureInfo culture)
        {
            return value.ToType(type, culture);
        }

        #region IDataReader..

        public string GetName(int i)
        {
            return Data.ColumnNames[i];
        }

        public int FieldCount => Data.ColumnNames.Length;

        public bool Read()
        {
            return ++rowIndex < Data.RowValues.Length;
        }

        public bool NextResult()
        {
            return false; // only for 1 result
        }

        public object GetValue(int i)
        {
            var rawValue = Data.RowValues[rowIndex][i];
            if (rawValue == NullValue || rawValue == null)
                return DBNull.Value;
            else
                return ParseValue(SchemaColumns[i].ClrType, rawValue, Data.CultureInfo); 
        }

        public void Dispose()
        {
        }

        #region not implemented..

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
            //return SchemaColumns[i].ClrType;
        }

        public int Depth => throw new NotImplementedException();

        public bool IsClosed => throw new NotImplementedException();

        public int RecordsAffected => throw new NotImplementedException();


        public object this[string name] => throw new NotImplementedException();

        public object this[int i] => throw new NotImplementedException();

        public void Close()
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }
        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
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

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }





    /*


    public class UntypedData
    {
        public ResultSetSchema Schema { get; set; }

        public Stream Data { get; set; }

        public CultureInfo Culture { get; set; }

        public static object ParseValue(Type type, IConvertible value, CultureInfo culture)
        {
            return value.ToType(type, culture);
        }
    }

    public class CsvData: UntypedData
    {
        public string ColumnSeparator { get; set; }

        public Encoding Encoding { get; set; }

    }



    public class UntypedDataSetReader : IDataReader
    {
        private IEnumerable<UntypedData> DataSet { get; set; }
        private int DataIndex = -1;
        private int DataRowIndex = -1;

        public UntypedDataSetReader(IEnumerable<UntypedData> dataSet)
        {
            dataSet.ThrowIfNull("dataSet");

            this.DataSet = DataSet;
        }


        private static IDictionary<string, object>[] ConvertUntypedData(string[][] data, ResultSetSchema schema)
        {
            for (var lineIndex=0; lineIndex<data.Length; lineIndex++)
            {
                var lineValues = data[lineIndex];
                for (var columnIndex=0; columnIndex<lineValues.Length; columnIndex++)
                {
                    var value = lineValues[columnIndex];

                    //schema.Columns[columnIndex].
                }
            }

            foreach (var lineValues in data)
            {
                
            }

            return null;
        }

        private static string[][] GetCsvDataFromStream(Stream csv, string columnSeparator, bool skipFirstHeaderRow)
        {
            using (var sr = new StreamReader(csv))
            {
                var result = new List<string[]>();

                if (!sr.EndOfStream && skipFirstHeaderRow)
                    sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    var columnValues = GetColumnValuesFromLine(sr.ReadLine(), columnSeparator);

                    result.Add(columnValues);
                }

                return result.ToArray();
            }
        }

        internal static string[] GetColumnValuesFromLine(string line, string columnSeparator)
        {
            return line.Split(new string[] { columnSeparator }, StringSplitOptions.None);
        }

        #region IDataReader implementation

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            throw new NotImplementedException();
        }


        public object this[int i] => throw new NotImplementedException();

        public object this[string name] => throw new NotImplementedException();

        public int Depth => throw new NotImplementedException();

        public bool IsClosed => throw new NotImplementedException();

        public int RecordsAffected => throw new NotImplementedException();

        public int FieldCount => throw new NotImplementedException();

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

        public string GetDataTypeName(int i)
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

        public Type GetFieldType(int i)
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

        public string GetName(int i)
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

        public object GetValue(int i)
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

        

        #endregion

    
    }

    */
}
