namespace WinAppDriver
{
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Automation;
    using System.Xml;
    using System.Xml.XPath;

    internal class UIAutomation : IUIAutomation
    {
        public string DumpXml(AutomationElement root)
        {
            return this.DumpXmlImpl(root, null);
        }

        public string DumpXml(AutomationElement root, out IList<AutomationElement> elements)
        {
            elements = new List<AutomationElement>();
            return this.DumpXmlImpl(root, elements);
        }

        public AutomationElement FindFirstByXPath(AutomationElement root, string xpath)
        {
            IList<AutomationElement> nodes;
            string xml = this.DumpXml(root, out nodes);

            var doc = new XPathDocument(new StringReader(xml));
            XPathNavigator node = doc.CreateNavigator().SelectSingleNode(xpath);
            if (node == null)
            {
                return null;
            }
            else
            {
                var index = int.Parse(node.GetAttribute("_index_", ""));
                return nodes[index];
            }
        }

        public IList<AutomationElement> FindAllByXPath(AutomationElement root, string xpath)
        {
            IList<AutomationElement> elements;
            string xml = this.DumpXml(root, out elements);

            var doc = new XPathDocument(new StringReader(xml));
            XPathNodeIterator nodes = doc.CreateNavigator().Select(xpath);

            var results = new List<AutomationElement>();
            foreach (XPathNavigator node in nodes)
            {
                var index = int.Parse(node.GetAttribute("_index_", ""));
                results.Add(elements[index]);
            }

            return results;
        }

        private string DumpXmlImpl(AutomationElement start, IList<AutomationElement> elements)
        {
            var control = new PropertyCondition(AutomationElement.IsControlElementProperty, true);
            var visible = new PropertyCondition(AutomationElement.IsOffscreenProperty, false);
            var walker = new TreeWalker(new AndCondition(control, visible));

            var stringWriter = new StringWriter();
            XmlWriter writer = new XmlTextWriter(stringWriter);

            writer.WriteStartDocument();
            writer.WriteStartElement("WinAppDriver");
            this.WalkTree(AutomationElement.RootElement, walker, writer, elements);
            writer.WriteEndDocument();

            return stringWriter.ToString();
        }

        private void WalkTree(AutomationElement parent, TreeWalker walker, XmlWriter writer, IList<AutomationElement> elements)
        {
            var child = walker.GetFirstChild(parent);
            while (child != null)
            {
                var info = child.Current;
                writer.WriteStartElement(info.ControlType.ProgrammaticName);
                if (elements != null)
                {
                    writer.WriteAttributeString("_index_", elements.Count.ToString());
                    elements.Add(child);
                }

                writer.WriteAttributeString("id", info.AutomationId);
                writer.WriteAttributeString("name", info.Name);
                writer.WriteAttributeString("class", info.ClassName);

                this.WalkTree(child, walker, writer, elements);
                writer.WriteEndElement();

                child = walker.GetNextSibling(child);
            }
        }
    }
}