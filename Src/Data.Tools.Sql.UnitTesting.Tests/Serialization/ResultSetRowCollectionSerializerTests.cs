using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Tools.UnitTesting.Tests.Utils;
using Data.Tools.UnitTesting.Serialization;
using Data.Tools.UnitTesting;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Tests.Serialization
{
    [TestClass]
    public class ResultSetRowCollectionSerializerTests
    {
        #region ReadXml tests..

        [TestMethod]
        public void ReadXmlThrowsWithNullReader()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ResultSetRowCollectionSerializer().Deserialize(null, new ResultSetRowCollectionSerializerContext { Schema = new ResultSetSchema() });
            }, "reader");
        }

        [TestMethod]
        public void ReadXmlThrowsWithNullContext()
        {
            using (var r = new TestXmlReader("<Rows/>"))
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new ResultSetRowCollectionSerializer().Deserialize(r.Reader, null);
                }, "context");
            }
        }

        [TestMethod]
        public void ReadXmlThrowsWithNullSchemaInContext()
        {
            using (var r = new TestXmlReader("<Rows/>"))
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new ResultSetRowCollectionSerializer().Deserialize(r.Reader, new ResultSetRowCollectionSerializerContext { Schema = null });
                }, "context.Schema");
            }
        }

        [TestMethod]
        public void ReadXmlThrowsIfReaderAtIncorrectElement()
        {
            using (var r = new TestXmlReader("<abc/>"))
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    new ResultSetRowCollectionSerializer().Deserialize(r.Reader, new ResultSetRowCollectionSerializerContext { Schema = new ResultSetSchema() });
                }, "Reader not at Rows elelement");
            }
        }

        [TestMethod]
        public void CanReadXmlForEmptyCollection()
        {
            using (var r = new TestXmlReader("<Rows/>"))
            {
                var rows = new ResultSetRowCollectionSerializer().Deserialize(r.Reader, new ResultSetRowCollectionSerializerContext { Schema = new ResultSetSchema() });

                Assert.IsNotNull(rows);
                Assert.AreEqual(0, rows.Count);
            }
        }

        [TestMethod]
        public void CanReadXmlFor2Rows()
        {
            using (var r = new TestXmlReader("<Rows><Row><Name>will</Name></Row><Row><Name>tess</Name></Row></Rows>"))
            {
                var schema = new ResultSetSchema();
                schema.Columns.Add(new Column { ClrType = typeof(string), Name = "Name", DbType = "varchar" });

                var rows = new ResultSetRowCollectionSerializer().Deserialize(r.Reader, new ResultSetRowCollectionSerializerContext { Schema = schema });

                Assert.IsNotNull(rows);
                Assert.AreEqual(2, rows.Count);
            }
        }

        #endregion

        #region WriteXml tests..

        [TestMethod]
        public void WriteXmlThrowsWithNullWriter()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ResultSetRowCollectionSerializer().Serialize(null, new ResultSetRowCollection(), new ResultSetRowCollectionSerializerContext { Schema = new ResultSetSchema() });
            }, "writer");
        }

        [TestMethod]
        public void WriteXmlThrowsWithNullContext()
        {
            using (var w = new TestXmlWriter())
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new ResultSetRowCollectionSerializer().Serialize(w.Writer, new ResultSetRowCollection(), null);
                }, "context");
            }
        }

        [TestMethod]
        public void WriteXmlThrowsWithNullSchemaInContext()
        {
            using (var w = new TestXmlWriter())
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new ResultSetRowCollectionSerializer().Serialize(w.Writer, new ResultSetRowCollection(), new ResultSetRowCollectionSerializerContext { Schema = null });
                }, "context.Schema");
            }
        }

        [TestMethod]
        public void WriteXmlThrowsWithNullRows()
        {
            using (var w = new TestXmlWriter())
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new ResultSetRowCollectionSerializer().Serialize(w.Writer, null, new ResultSetRowCollectionSerializerContext { Schema = new ResultSetSchema() });
                }, "rows");
            }
        }

        [TestMethod]
        public void WriteXmlThrowsIfRowCollectionIsInvalid()
        {
            using (var w = new TestXmlWriter())
            {
                var rows = new TestRowCollection { Exception = new Exception() };

                try
                {
                    new ResultSetRowCollectionSerializer().Serialize(w.Writer, rows, new ResultSetRowCollectionSerializerContext { Schema = new ResultSetSchema() });
                }
                catch (Exception ex)
                {
                    Assert.AreSame(typeof(InvalidOperationException), ex.GetType());
                    Assert.AreSame(rows.Exception, ex.InnerException);
                    return;
                }

                Assert.Fail("Expected exception not thrown");
            }
        }

        [TestMethod]
        public void CanwriteXmlFor2Rows()
        {
            using (var w = new TestXmlWriter())
            {
                var rows = new ResultSetRowCollection();
                rows.Add(new ResultSetRow());
                rows.Add(new ResultSetRow());
                rows[0]["Name"] = "john";
                rows[1]["Name"] = "smith";

                var schema = new ResultSetSchema();
                schema.Columns.Add(new Column { ClrType = typeof(string), DbType = "varchar", Name = "Name" });

                new ResultSetRowCollectionSerializer().Serialize(w.Writer, rows, new ResultSetRowCollectionSerializerContext { Schema = schema });

                Assert.AreEqual("<Rows><Row><Name>john</Name></Row><Row><Name>smith</Name></Row></Rows>", w.Xml);
            }
        }

        [TestMethod]
        public void CanWriteXmlFor0Rows()
        {
            using (var w = new TestXmlWriter())
            {
                var rows = new ResultSetRowCollection();

                new ResultSetRowCollectionSerializer().Serialize(w.Writer, rows, new ResultSetRowCollectionSerializerContext { Schema = new ResultSetSchema() });

                Assert.AreEqual("<Rows />", w.Xml);
            }
        }

        #endregion

        private class TestRowCollection : ResultSetRowCollection
        {
            public Exception Exception { get; set; }

            public override Exception Validate(ResultSetSchema schema, bool checkProperties = false)
            {
                return Exception;
            }
        }
    }
}
