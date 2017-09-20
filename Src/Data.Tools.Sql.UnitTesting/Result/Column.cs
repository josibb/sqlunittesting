using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Data.Tools.UnitTesting.Utils;

namespace Data.Tools.UnitTesting.Result
{
    public class Column
    {
        public string Name { get; set; }

        public string DbType { get; set; }

        public string InternalClrType
        {
            get
            {
                if (ClrType == null)
                    return null;
                else
                    return ClrType.FullName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    ClrType = null;
                else
                    ClrType = System.Type.GetType(value, true);
            }
        }

        public Type ClrType { get; set; }

        /// <summary>
        /// Validation. If valid, null is returned. Else the exception
        /// </summary>
        /// <returns></returns>
        public virtual Exception Validate()
        {
            if (Name == null)
                return new InvalidOperationException("Name is null");

            if (string.IsNullOrWhiteSpace(Name))
                return new InvalidOperationException("Name is empty");

            if (DbType == null)
                return new InvalidOperationException("DbType is null");

            if (InternalClrType == null)
                return new InvalidOperationException("ClrType is null");

            return null;
        }

        public static Column CreateFromReader(IDataReader reader, int fieldIndex)
        {
            reader.ThrowIfNull("reader");

            return new Column
            {
                Name = reader.GetName(fieldIndex),
                DbType = reader.GetDataTypeName(fieldIndex),
                ClrType = reader.GetFieldType(fieldIndex)
            };
        }
    }
}
