using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Data.Tools.UnitTesting.Utils;
using Data.Tools.UnitTesting.Result;

namespace Data.Tools.UnitTesting.Serialization
{
    public class ActionResultSerializer: IXmlSerializable<ActionResult>
    {
        private readonly IXmlSerializer<ResultSet> resultSetSerializer = new ResultSetSerializer(); //todo inject

        public void ReadXml(XmlReader reader, ActionResult actionResult)
        {
            reader.ThrowIfNull("reader");
            actionResult.ThrowIfNull("actionResult");

            /*
             * Reading Action result contains the tag <ActioNResult> while writing, the xml serializer
             * already writes this to the writer.
             * Therefore when reading, Expect the ActionResult node, and when writing, do not write this element
             */
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ActionResult")
            {
                if (reader.ReadToDescendant("ResultSets"))
                {
                    actionResult.ResultSets = new List<ResultSet>();

                    if (reader.ReadToDescendant("ResultSet"))
                    {
                        do
                        {
                            actionResult.ResultSets.Add(resultSetSerializer.Deserialize(reader));
                        }
                        while (reader.ReadToNextSibling("ResultSet"));
                    }
                    else
                    {
                        // no resultsets found
                    }
                }
                else
                {
                    throw new InvalidOperationException("No ResultSets element found in ActionResult");
                }
            }
            else
            {
                throw new InvalidOperationException("Reader not at ActionResult element");
            }
        }

        public void WriteXml(XmlWriter writer, ActionResult actionResult)
        {
            writer.ThrowIfNull("writer");
            actionResult.ThrowIfNull("actionResult");

            var ex = actionResult.Validate(false);
            if (ex != null)
                throw new InvalidOperationException("ActionResult validation failed", ex);

            /*
             * Reading Action result contains the tag <ActioNResult> while writing, the xml serializer
             * already writes this to the writer.
             * Therefore when reading, Expect the ActionResult node, and when writing, do not write this element
             */

            writer.WriteStartElement("ResultSets");

            for (var t = 0; t < actionResult.ResultSets.Count; t++)
            {
                resultSetSerializer.Serialize(writer, actionResult.ResultSets[t]);
            }

            writer.WriteEndElement();
        }
    }


}
