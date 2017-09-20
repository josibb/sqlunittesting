using Data.Tools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using Data.Tools.UnitTesting.Serialization;
using Data.Tools.UnitTesting.Utils;
using System.Reflection;

namespace Data.Tools.UnitTesting.Result
{
    [XmlRoot("ActionResult", Namespace = "http://tempuri.org/ActionResult.xsd")]
    public class ActionResult : IXmlSerializable
    {
        private readonly IXmlSerializable<ActionResult> serializer;

        public long EllapsedSqlMilliseconds { get; set; }

        public IList<ResultSet> ResultSets { get; set; }

        public void Serialize(Stream stream)
        {
            stream.ThrowIfNull("stream");

            var serializer = new XmlSerializer(typeof(ActionResult));

            serializer.Serialize(stream, this);
        }

        public void Serialize(string fileName)
        {
            fileName.ThrowIfNull("fileName");

            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                Serialize(fs);
            }
        }

        public string Serialize()
        {
            using (var sr = new StringWriter())
            {
                new XmlSerializer(typeof(ActionResult)).Serialize(sr, this);

                return sr.ToString();
            }
        }

        public static ActionResult CreateFromReader(IDataReader reader)
        {
            reader.ThrowIfNull("reader");

            var result = new ActionResult();

            do
            {
                var resultSet = ResultSet.CreateFromReader(reader);
                if (resultSet != null)
                {
                    result.ResultSets.Add(resultSet);
                }
            } while (reader.NextResult());

            return result;
        }

        public static ActionResult Deserialize(Stream s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            using (var reader = CreateValidatingXmlReader(s))
            {
                return (ActionResult)new XmlSerializer(typeof(ActionResult)).Deserialize(reader);
            }
        }

        public static ActionResult ReadFromResource(string resourceName)
        {
            using (var s = Resources.GetResourceStreamFromAssembly(Assembly.GetCallingAssembly(), resourceName))
            {
                return Deserialize(s);
            }
        }
        public static ActionResult ReadFromResource(Assembly assembly, string resourceName)
        {
            using (var s = Resources.GetResourceStreamFromAssembly(assembly, resourceName))
            {
                return Deserialize(s);
            }
        }

        private static XmlReader CreateValidatingXmlReader(Stream s)
        {
            using (var rs = Resources.GetResourceStreamFromAssembly(typeof(ActionResult).Assembly, "Serialization.ActionResult.xsd"))
            {
                var settings = new XmlReaderSettings
                {
                    ValidationType = ValidationType.Schema,
                    IgnoreWhitespace = true
                };
                using (var r = XmlReader.Create(rs))
                {
                    settings.Schemas.Add("http://tempuri.org/ActionResult.xsd", r);
                }
                settings.ValidationEventHandler += (object sender, ValidationEventArgs e) => { throw e.Exception; }; // ust throw ex
                
                return XmlReader.Create(s, settings);
            }
        }

        public ActionResult(IXmlSerializable<ActionResult> serializer)
        {
            this.serializer = serializer.ThrowIfNull("serializer");
            ResultSets = new List<ResultSet>();
        }

        public ActionResult() : this(new ActionResultSerializer()) { }

        public void ReadXml(XmlReader reader)
        {
            serializer.ReadXml(reader, this);
        }

        public void WriteXml(XmlWriter writer)
        {
            serializer.WriteXml(writer, this);
        }

        public virtual Exception Validate(bool recursive = false)
        {
            if (ResultSets == null)
                return new InvalidOperationException("ResultSets is null");

            if (recursive)
            {
                foreach (var rs in this.ResultSets)
                {
                    var ex = rs.Validate(true);
                    if (ex != null)
                        return new InvalidOperationException("ResultSet validation failed", ex);
                }
            }

            return null;
        }

        public XmlSchema GetSchema()
        {
            return null; // leave here for IXmlSerialize .NET purposes
        }
    }
}

