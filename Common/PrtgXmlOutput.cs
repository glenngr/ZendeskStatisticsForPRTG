using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Common
{
    public class PrtgXmlOutput
    {
        XmlWriter _xml;
        public PrtgXmlOutput()
        {
            _xml = XmlWriter.Create(Console.Out);
            CreateRoot();
        }
        private void CreateRoot()
        {
            // Root element - start tag
            _xml.WriteStartElement("prtg");
        }

        public void AddChannel(string channelName, string value, bool writeEnd = true) {
            _xml.WriteStartElement("result");

            _xml.WriteElementString("channel", channelName);
            _xml.WriteElementString("value", value);

            if (writeEnd)
            {
                // end result
                _xml.WriteEndElement();
            }

        }

        public void AddChannel(string channelName, string value, Dictionary<string, string> data)
        {
            AddChannel(channelName, value, false);

            foreach (var item in data)
            {
                _xml.WriteElementString(item.Key, item.Value);
                _xml.WriteEndElement();
            }

            _xml.WriteEndElement();
        }

        private void CloseXml() {
            // Root element - end tag
            _xml.WriteEndElement();

            // End Documentd
            _xml.WriteEndDocument();
        }

        public void PrintXml()
        {
            CloseXml();
            // Flush it
            _xml.Flush();
        }
    }
}
