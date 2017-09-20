using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Data.Tools.UnitTesting.Tests.Utils
{
    public class TestXmlWriter : IDisposable
    {
        public XmlWriter Writer { get; private set; }

        public StringBuilder stringBuilder { get; private set; }

        public string Xml { get { Writer.Flush(); return stringBuilder.ToString(); } }

        public TestXmlWriter()
        {
            stringBuilder = new StringBuilder();
            Writer = XmlWriter.Create(stringBuilder, SerializerHelper.DefaultSettings);
        }

        public void Dispose()
        {
            if (Writer != null)
            {
                Writer.Dispose();
                Writer = null;
            }
        }
    }
}
