using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting;
using Data.Tools.UnitTesting.Tests.Utils;
using Data.Tools.UnitTesting.Serialization;
using System.Globalization;
using System.Threading;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Tests.Serialization
{
    [TestClass]
    public class ResultSetRowSerializerTests
    {
        #region WriteXml tests..

        [TestMethod]
        public void WriteXmlTHrowsWithNullWriter()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ResultSetRowSerializer().Serialize(null, new ResultSetRow(), new ResultSetRowSerializerContext());
            }, "writer");
        }

        [TestMethod]
        public void WriteXmlThrowsWithNullRow()
        {
            using (var w = new TestXmlWriter())
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new ResultSetRowSerializer().Serialize(w.Writer, null, new ResultSetRowSerializerContext { Schema = new ResultSetSchema() });
                }, "row");
            }
        }

        [TestMethod]
        public void WriteXmlTHrowsWithNullContext()
        {
            using (var w = new TestXmlWriter())
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new ResultSetRowSerializer().Serialize(w.Writer, new ResultSetRow(), null);
                }, "context");
            }
        }

        [TestMethod]
        public void WriteXmlThrowsWithNullSchemainContext()
        {
            using (var w = new TestXmlWriter())
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new ResultSetRowSerializer().Serialize(w.Writer, new ResultSetRow(), new ResultSetRowSerializerContext { Schema = null });
                }, "context.Schema");
            }
        }

        [TestMethod]
        public void CanWriteRowWithVariousTypes()
        {
            var schema = new ResultSetSchema();
            schema.Columns.Add(new Column { Name = "cola", ClrType = typeof(int), DbType = "" });
            schema.Columns.Add(new Column { Name = "colb", ClrType = typeof(string), DbType = "" });
            schema.Columns.Add(new Column { Name = "colc", ClrType = typeof(DateTime), DbType = "" });
            schema.Columns.Add(new Column { Name = "cold", ClrType = typeof(int), DbType = "" });
            schema.Columns.Add(new Column { Name = "cole", ClrType = typeof(int), DbType = "" });

            var row = new ResultSetRow();
            row["cola"] = 12;
            row["colb"] = "hallo";
            row["colc"] = new DateTime(2011, 9, 14, 18, 58, 44).AddMilliseconds(123);
            row["cold"] = null;
            row["cole"] = DBNull.Value;

            using (var writer = new TestXmlWriter())
            {
                new ResultSetRowSerializer().Serialize(writer.Writer, row, new ResultSetRowSerializerContext { Schema = schema });

                var expectedXml = "<Row><cola>12</cola><colb>hallo</colb><colc>2011-09-14T18:58:44.123</colc></Row>";
                Assert.AreEqual(expectedXml, writer.Xml);
            }
        }

        [TestMethod]
        public void CanWrite0ZeroRows()
        {
            using (var writer = new TestXmlWriter())
            {
                new ResultSetRowSerializer().Serialize(writer.Writer, new ResultSetRow(), new ResultSetRowSerializerContext { Schema = new ResultSetSchema() });

                var expectedXml = "<Row />";
                Assert.AreEqual(expectedXml, writer.Xml);
            }
        }

        [TestMethod]
        public void WriteXmlThrowsIfRowIsInvalid()
        {
            using (var writer = new TestXmlWriter())
            {
                var row = new TestRow { Exception = new Exception() };
                try
                {
                    new ResultSetRowSerializer().Serialize(writer.Writer, row, new ResultSetRowSerializerContext { Schema = new ResultSetSchema() });
                }
                catch (Exception ex)
                {
                    Assert.AreSame(typeof(InvalidOperationException), ex.GetType());
                    Assert.AreSame(row.Exception, ex.InnerException);
                    return;
                }

                Assert.Fail("Expected exception not thrown");
            }
        }

        private class TestRow : ResultSetRow
        {
            public Exception Exception { get; set; }
            public override Exception Validate(ResultSetSchema schema)
            {
                return Exception;
            }
        }

        #endregion


        #region ReadXml tests..

        [TestMethod]
        public void ReadXmlThrowsIfColumnNameNotInSchema()
        {
            var schema = new ResultSetSchema();
            schema.Columns.Add(new Column { Name = "ColA", ClrType = typeof(string), DbType = "" });

            var xml = "<Row><ColA>test</ColA><colb>hey</colb></Row>";
            using (var r = new TestXmlReader(xml))
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    new ResultSetRowSerializer().Deserialize(r.Reader, new ResultSetRowSerializerContext { Schema = schema });
                }, "Cannot find column 'colb' in schema");
            }
        }

        [TestMethod]
        public void ReadXmlTHrowsWithNullReader()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ResultSetRowSerializer().Deserialize(null, new ResultSetRowSerializerContext { Schema = new ResultSetSchema() });
            }, "reader");
        }

        [TestMethod]
        public void ReadXmlThrowsWithNullContext()
        {
            using (var r = new TestXmlReader("<Row/>"))
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new ResultSetRowSerializer().Deserialize(r.Reader, null);
                }, "context");
            }
        }

        [TestMethod]
        public void ReadXmlthrowsWithNullSchemaInContext()
        {
            using (var r = new TestXmlReader("<Row/>"))
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new ResultSetRowSerializer().Deserialize(r.Reader, new ResultSetRowSerializerContext { Schema = null });
                }, "context.Schema");
            }
        }

        [TestMethod]
        public void CannotReadRowIfRowNameDoesNotExistInSchema()
        {
            var xml = "<Row><cola>12</cola></Row>";

            using (var r = TestXmlReader.Create(xml))
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    new ResultSetRowSerializer().Deserialize(r.Reader, new ResultSetRowSerializerContext { Schema = new ResultSetSchema() });
                }, "Cannot find column 'cola' in schema");
            }
        }

        [TestMethod]
        public void ReadXmlThrowsIfReaderAtNoRowElement()
        {
            using (var r = new TestXmlReader("<abc></abc>"))
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    new ResultSetRowSerializer().Deserialize(r.Reader, new ResultSetRowSerializerContext { Schema = new ResultSetSchema() });
                }, "Cannot read row from reader. LocalName='abc'");
            }
        }

        [TestMethod]
        public void CanReadEmptyRow()
        {
            using (var r = TestXmlReader.Create("<Row />"))
            {
                var row = new ResultSetRowSerializer().Deserialize(r.Reader, new ResultSetRowSerializerContext { Schema = new ResultSetSchema() });

                Assert.IsNotNull(row);
                Assert.AreEqual(0, row.Count);
            }
        }

        [TestMethod]
        public void CanReadRowWith2Columns()
        {
            var xml = "<Row><cola>11</cola><colb>value2</colb></Row>";

            var schema = new ResultSetSchema();
            schema.Columns.Add(new Column { Name = "cola", ClrType = typeof(int) });
            schema.Columns.Add(new Column { Name = "colb", ClrType = typeof(string) });

            using (var r = TestXmlReader.Create(xml))
            {
                var row = new ResultSetRowSerializer().Deserialize(r.Reader, new ResultSetRowSerializerContext { Schema = schema });

                Assert.IsNotNull(row);
                Assert.AreEqual(2, row.Count);
                Assert.AreEqual(11, row["cola"]);
                Assert.AreSame(typeof(int), row["cola"].GetType());
                Assert.AreEqual("value2", row["colb"]);
                Assert.AreSame(typeof(string), row["colb"].GetType());
            }
        }

        [TestMethod]
        public void CanReadRowWith4ColumnsAnd1NullValue()
        {
            var xml = "<Row><cola>12</cola><colb>hallo</colb><colc>2011-09-14T18:58:44.123</colc><cold>13.66666</cold></Row>";

            var schema = new ResultSetSchema();
            schema.Columns.Add(new Column { Name = "cola", ClrType = typeof(int) });
            schema.Columns.Add(new Column { Name = "colb", ClrType = typeof(string) });
            schema.Columns.Add(new Column { Name = "colc", ClrType = typeof(DateTime) });
            schema.Columns.Add(new Column { Name = "cold", ClrType = typeof(decimal) });
            schema.Columns.Add(new Column { Name = "cole", ClrType = typeof(float) });

            using (var r = TestXmlReader.Create(xml))
            {
                var row = new ResultSetRowSerializer().Deserialize(r.Reader, new ResultSetRowSerializerContext { Schema = schema });

                Assert.IsNotNull(row);
                Assert.AreEqual(5, row.Count);
                Assert.AreEqual(12, row["cola"]);
                Assert.AreSame(typeof(int), row["cola"].GetType());
                Assert.AreEqual("hallo", row["colb"]);
                Assert.AreSame(typeof(string), row["colb"].GetType());
                Assert.AreEqual(new DateTime(2011, 9, 14, 18, 58, 44).AddMilliseconds(123), row["colc"]);
                Assert.AreSame(typeof(DateTime), row["colc"].GetType());
                Assert.AreEqual(13.66666m, row["cold"]);
                Assert.AreSame(typeof(decimal), row["cold"].GetType());
                Assert.AreEqual(DBNull.Value, row["cole"]);
                Assert.AreSame(DBNull.Value.GetType(), row["cole"].GetType());
            }
        }

        [TestMethod]
        public void ReadXmlAddsNullValuesForNotDefinedRowValuesInXml()
        {
            var xml = "<Row><colb>value2</colb></Row>";

            var schema = new ResultSetSchema();
            schema.Columns.Add(new Column { Name = "cola", ClrType = typeof(int) });
            schema.Columns.Add(new Column { Name = "colb", ClrType = typeof(string) });

            using (var r = TestXmlReader.Create(xml))
            {
                var row = new ResultSetRowSerializer().Deserialize(r.Reader, new ResultSetRowSerializerContext { Schema = schema });

                Assert.IsNotNull(row);
                Assert.AreEqual(2, row.Count);
                Assert.AreEqual(DBNull.Value, row["cola"]);
                Assert.AreSame(DBNull.Value.GetType(), row["cola"].GetType());
                Assert.AreEqual("value2", row["colb"]);
                Assert.AreSame(typeof(string), row["colb"].GetType());
            }
        }

        [TestMethod]
        public void ReadXmlTHrowsIfReaderIsNotAtRowElement()
        {
            var xml = "<abc />";

            using (var r = TestXmlReader.Create(xml))
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    new ResultSetRowSerializer().Deserialize(r.Reader, new ResultSetRowSerializerContext { Schema = new ResultSetSchema() });
                }, "Reader not at Row element");
            }
        }


        #endregion

        [TestMethod]
        public void CanWriteAndReadRow()
        {
            var schema = new ResultSetSchema();
            schema.Columns.Add(new Column { Name = "cola", ClrType = typeof(int), DbType = "" });
            schema.Columns.Add(new Column { Name = "colb", ClrType = typeof(string), DbType = "" });
            schema.Columns.Add(new Column { Name = "colc", ClrType = typeof(DateTime), DbType = "" });
            schema.Columns.Add(new Column { Name = "cold", ClrType = typeof(int), DbType = "" });
            schema.Columns.Add(new Column { Name = "cole", ClrType = typeof(int), DbType = "" });

            var row = new ResultSetRow();
            row["cola"] = 12;
            row["colb"] = "hallo";
            row["colc"] = new DateTime(2011, 9, 14, 18, 58, 44).AddMilliseconds(123);
            row["cold"] = null;
            row["cole"] = DBNull.Value;

            using (var writer = new TestXmlWriter())
            {
                new ResultSetRowSerializer().Serialize(writer.Writer, row, new ResultSetRowSerializerContext { Schema = schema });

                using (var r = new TestXmlReader(writer.Xml))
                {
                    var row2 = new ResultSetRowSerializer().Deserialize(r.Reader, new ResultSetRowSerializerContext { Schema = schema });

                    Assert.IsNotNull(row2);
                    Assert.AreEqual(5, row2.Count);

                    Assert.AreEqual(12, row2["cola"]);
                    Assert.AreSame(typeof(int), row2["cola"].GetType());
                    Assert.AreEqual("hallo", row2["colb"]);
                    Assert.AreSame(typeof(string), row2["colb"].GetType());
                    Assert.AreEqual(new DateTime(2011, 9, 14, 18, 58, 44).AddMilliseconds(123), row2["colc"]);
                    Assert.AreSame(typeof(DateTime), row2["colc"].GetType());
                    Assert.AreEqual(DBNull.Value, row2["cold"]);
                    Assert.AreSame(DBNull.Value.GetType(), row2["cold"].GetType());
                    Assert.AreEqual(DBNull.Value, row2["cole"]);
                    Assert.AreSame(DBNull.Value.GetType(), row2["cole"].GetType());
                }
            }
        }

        #region ReadXmlIsLocalCultureIndependant..

        [TestMethod]
        public void SerializationAndDeserializationOfValuesIsLocalCultureIndependant()
        {
            // execute write and read using defferent cultures and check that datetime and decimal values are serialized and deserialized ok
            // use threads to avoid changing the current culture for th executing thread.

            var schema = new ResultSetSchema();
            schema.Columns.Add(new Column { Name = "Cola", ClrType = typeof(DateTime), DbType = "" });
            schema.Columns.Add(new Column { Name = "Colb", ClrType = typeof(decimal), DbType = "" });

            var row = new ResultSetRow();
            row["Cola"] = new DateTime(2011, 9, 14, 18, 58, 44).AddMilliseconds(123);
            row["Colb"] = 18.66m;

            var writeContext = new ThreadContext
            {
                Row = row,
                Schema = schema
            };

            var writeThread = new Thread(WriteXml);
            writeThread.CurrentCulture = new CultureInfo("nl-NL");
            writeThread.Start(writeContext);
            if (!writeThread.Join(2000))
                Assert.Fail("WriteXml did not finish within time");
            if (writeContext.Exception != null)
                throw writeContext.Exception;


            var readContext = new ThreadContext
            {
                Xml = writeContext.Xml,
                Schema = schema
            };
            var readThread = new Thread(ReadXml);
            readThread.CurrentCulture = new CultureInfo("en-US");
            readThread.Start(readContext);
            if (!readThread.Join(2000))
                Assert.Fail("ReadXml did not finish within time");
            if (readContext.Exception != null)
                throw readContext.Exception;

            var _row = readContext.Row;
            Assert.IsNotNull(_row);
            Assert.AreEqual(2, _row.Count);
            Assert.AreEqual(new DateTime(2011, 9, 14, 18, 58, 44).AddMilliseconds(123), _row["Cola"]);
            Assert.AreSame(typeof(DateTime), _row["Cola"].GetType());
            Assert.AreEqual(18.66m, _row["Colb"]);
            Assert.AreSame(typeof(Decimal), _row["Colb"].GetType());
        }

        private class ThreadContext
        {
            public ResultSetSchema Schema { get; set; }
            public ResultSetRow Row { get; set; }
            public string Xml { get; set; }
            public Exception Exception { get; set; }
        }

        private void ReadXml(object context)
        {
            var ctx = context as ThreadContext;
            if (ctx == null)
                throw new ArgumentException("context is not ReadXmlContext");

            try
            {
                using (var r = new TestXmlReader(ctx.Xml))
                {
                    ctx.Row = new ResultSetRowSerializer().Deserialize(r.Reader, new ResultSetRowSerializerContext { Schema = ctx.Schema });
                }
            }
            catch (Exception ex)
            {
                ctx.Exception = ex;
                throw;
            }
        }

        private void WriteXml(object context)
        {
            var ctx = context as ThreadContext;
            if (ctx == null)
                throw new ArgumentException("context is not ThreadContext");

            try
            {

                using (var w = new TestXmlWriter())
                {
                    new ResultSetRowSerializer().Serialize(w.Writer, ctx.Row, new ResultSetRowSerializerContext { Schema = ctx.Schema });

                    ctx.Xml = w.Xml;
                }
            }
            catch (Exception ex)
            {
                ctx.Exception = ex;
                throw;
            }
        }

        #endregion


        

    }
}
