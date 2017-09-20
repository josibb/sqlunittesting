using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using System.IO;
using Data.Tools.UnitTesting;
using System.Xml;
using System.Data;
using System.Collections.Generic;
using Data.Tools.UnitTesting.Tests.Utils;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Tests
{
    [TestClass]
    public class ResultSetSchemaTests
    {
        [TestMethod]
        public void EmptySchemaIsValid()
        {
            Assert.IsNull(new ResultSetSchema().Validate(false));
            Assert.IsNull(new ResultSetSchema().Validate(true));
        }

        [TestMethod]
        public void CanValidateSchemaWith2ValidRows()
        {
            var schema = new ResultSetSchema();
            schema.Columns.Add(new Column { Name = "Name", ClrType = typeof(string), DbType = "varchar" });
            schema.Columns.Add(new Column { Name = "Length", ClrType = typeof(int), DbType = "int" });

            Assert.IsNull(schema.Validate(false));
            Assert.IsNull(schema.Validate(true));
        }

        [TestMethod]
        public void ValidationOfSchemaWith1ValidAnd1InvalidRowsFailsWithRecusriveCheck()
        {
            var schema = new ResultSetSchema();
            schema.Columns.Add(new Column { Name = "Name", ClrType = typeof(string), DbType = "varchar" });
            schema.Columns.Add(new Column { ClrType = typeof(int), DbType = "int" }); // invalid

            Assert.IsNull(schema.Validate(false));
            Assert.IsNull(schema.Validate());

            var ex = schema.Validate(true);
            Assert.IsNotNull(ex);
            Assert.AreSame(typeof(InvalidOperationException), ex.GetType());
            Assert.AreEqual("Schema validation failed", ex.Message);
            Assert.IsNotNull(ex.InnerException);
        }

        [TestMethod]
        public void ValidationOfSchemaWith1ValidAnd1InvalidRowsSucceedsWithoutRecusriveCheck()
        {
            var schema = new ResultSetSchema();
            schema.Columns.Add(new Column { Name = "Name", ClrType = typeof(string), DbType = "varchar" });
            schema.Columns.Add(new Column { ClrType = typeof(int), DbType = "int" }); // invalid

            Assert.IsNull(schema.Validate(false));
            Assert.IsNull(schema.Validate());
        }

        [TestMethod]
        public void ColumnsIsCreatedByDefault()
        {
            var rs = new ResultSetSchema();
            Assert.IsNotNull(rs.Columns);
        }

        [TestMethod]
        public void CanGetColumn()
        {
            var rs = new ResultSetSchema();

            var cola = new Column { Name = "cola" };
            var colb = new Column { Name = "colb" };
            var colc = new Column { Name = "colc" };

            rs.Columns.Add(cola);
            rs.Columns.Add(colb);
            rs.Columns.Add(colc);

            Assert.AreSame(cola, rs.GetColumn("cola"));
            Assert.AreSame(colb, rs.GetColumn("colb"));
            Assert.AreSame(colc, rs.GetColumn("colc"));
        }

        [TestMethod]
        public void GetColumnThrowsExceptionIfColumnIsNotFound()
        {
            var rs = new ResultSetSchema();

            var cola = new Column { Name = "cola" };
            var colb = new Column { Name = "colb" };
            var colc = new Column { Name = "colc" };

            rs.Columns.Add(cola);
            rs.Columns.Add(colb);
            rs.Columns.Add(colc);

            Assert.ThrowsException<InvalidOperationException>(() => { rs.GetColumn("nonexisting"); });
        }

        [TestMethod]
        public void CanCreateFromReader()
        {
            var r = new TestDataReader();
            r.ResultSets.Add(new ResultSet());
            r.ResultSets[0].Schema.Columns.Add(new Column { Name = "cola", ClrType = typeof(DateTime), DbType = "datetime" });
            r.ResultSets[0].Schema.Columns.Add(new Column { Name = "colb", ClrType = typeof(string), DbType = "varchar" });

            var rss = ResultSetSchema.CreateFromReader(r);
            Assert.IsNotNull(rss);
            Assert.IsNotNull(rss.Columns);
            Assert.AreEqual(2, rss.Columns.Count);
            Assert.AreEqual("cola", rss.Columns[0].Name);
            Assert.AreEqual("colb", rss.Columns[1].Name);
            Assert.AreEqual("datetime", rss.Columns[0].DbType);
            Assert.AreEqual("varchar", rss.Columns[1].DbType);
            Assert.AreSame(typeof(DateTime), rss.Columns[0].ClrType);
            Assert.AreEqual(typeof(string), rss.Columns[1].ClrType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CannotCreateFromReaderWhenReaderIsNull()
        {
            ResultSetSchema.CreateFromReader(null);
        }


        
    }

   

}
