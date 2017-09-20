using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Data.Tools.UnitTesting.Tests
{
    public class SerializerHelper
    {
        public static XmlWriterSettings DefaultSettings
        {
            get
            {
                var settings = new XmlWriterSettings();
                settings.Indent = false;
                settings.OmitXmlDeclaration = true;
                settings.NewLineChars = "";
                settings.NewLineHandling = NewLineHandling.None;
                
                return settings;
            }
        }

        public static string SerializeObject(object obj, XmlWriterSettings settings)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var serializer = new XmlSerializer(obj.GetType());

            using (var sw = new StringWriter(CultureInfo.InvariantCulture))
            {
                using (var xw = XmlWriter.Create(sw, settings))
                {
                    serializer.Serialize(xw, obj, ns);

                    return sw.ToString();
                }
            }
        }
        public static string SerializeObject(object obj)
        {
            return SerializeObject(obj, DefaultSettings);
        }

        public static T Deserialize<T>(string xml)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var serializer = new XmlSerializer(typeof(T));

            using (var sr = new StringReader(xml))
            {
                return (T)serializer.Deserialize(sr);
            }
        }

        public static T Deserialize<T>(XmlReader reader)
        {
            var serializer = new XmlSerializer(typeof(T));

            return (T)serializer.Deserialize(reader);
        }
    }
}
