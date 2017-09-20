using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Data.Tools.UnitTesting.Tests.Utils
{
    public class TestXmlReader : IDisposable
    {
        private StringReader sr;
        public XmlReader Reader { get; private set; }

        public TestXmlReader(string xml)
        {
            sr = new StringReader(xml);

            var settings = new XmlReaderSettings();
            settings.Schemas.Add("http://tempuri.org/ActionResult.xsd", "Serialization\\ActionResult.xsd");
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += Settings_ValidationEventHandler;
            Reader = XmlReader.Create(sr, settings);

            Reader.MoveToContent();
        }

        private void Settings_ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            throw e.Exception;
        }

        public static TestXmlReader Create(string xml)
        {
            return new TestXmlReader(xml);
        }

        public void Dispose()
        {
            if (Reader != null)
            {
                Reader.Dispose();
                Reader = null;
            }

            if (sr != null)
            {
                sr.Dispose();
                sr = null;
            }
        }
    }
}
