namespace WinAppDriver.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Automation;
    using System.Xml;
    using System.Xml.XPath;

    internal class UIAutomation : IUIAutomation
    {
        public AutomationElement GetFocusedWindowOrRoot()
        {
            var walker = TreeWalker.ContentViewWalker;
            var node = AutomationElement.FocusedElement;
            var parent = node;

            while (parent != AutomationElement.RootElement)
            {
                node = parent;
                parent = walker.GetParent(node);
            }

            return node;
        }

        public ISet<AutomationElement> GetTopLevelWindows()
        {
            var windows = new HashSet<AutomationElement>();
            var walker = TreeWalker.ControlViewWalker;

            var child = walker.GetFirstChild(AutomationElement.RootElement);
            while (child != null)
            {
                windows.Add(child);
                child = walker.GetNextSibling(child);
            }

            return windows;
        }

        public IntPtr ToNativeWindowHandle(AutomationElement element)
        {
            var handle = (int)element.GetCurrentPropertyValue(
                AutomationElement.NativeWindowHandleProperty, true);
            return new IntPtr(handle);
        }

        public AutomationElement FromNativeWindowHandle(IntPtr handle)
        {
            return AutomationElement.FromHandle(handle);
        }

        public string DumpXml(AutomationElement start)
        {
            return this.DumpXmlImpl(start, null);
        }

        public string DumpXml(AutomationElement start, out IList<AutomationElement> elements)
        {
            elements = new List<AutomationElement>();
            return this.DumpXmlImpl(start, elements);
        }

        public AutomationElement FindFirstByXPath(AutomationElement start, string xpath)
        {
            IList<AutomationElement> nodes;
            string xml = this.DumpXml(start, out nodes);

            var doc = new XPathDocument(new StringReader(xml));
            XPathNavigator node = doc.CreateNavigator().SelectSingleNode(xpath);
            if (node == null)
            {
                return null;
            }
            else
            {
                var index = int.Parse(node.GetAttribute("_index_", string.Empty));
                return nodes[index];
            }
        }

        public IList<AutomationElement> FindAllByXPath(AutomationElement start, string xpath)
        {
            IList<AutomationElement> elements;
            string xml = this.DumpXml(start, out elements);

            var doc = new XPathDocument(new StringReader(xml));
            XPathNodeIterator nodes = doc.CreateNavigator().Select(xpath);

            var results = new List<AutomationElement>();
            foreach (XPathNavigator node in nodes)
            {
                var index = int.Parse(node.GetAttribute("_index_", string.Empty));
                results.Add(elements[index]);
            }

            return results;
        }

        private string DumpXmlImpl(AutomationElement start, IList<AutomationElement> elements)
        {
            var stringWriter = new StringWriter();
            XmlWriter writer = new XmlTextWriter(stringWriter);

            writer.WriteStartDocument();
            writer.WriteStartElement("WinAppDriver");
            this.WalkTree(start, TreeWalker.ControlViewWalker, writer, elements);
            writer.WriteEndDocument();

            return stringWriter.ToString();
        }

        private void WalkTree(AutomationElement parent, TreeWalker walker, XmlWriter writer, IList<AutomationElement> elements)
        {
            var info = parent.Current;
            writer.WriteStartElement(info.ControlType.ProgrammaticName);
            if (elements != null)
            {
                writer.WriteAttributeString("_index_", elements.Count.ToString());
                elements.Add(parent);
            }

            writer.WriteAttributeString("id", info.AutomationId);
            writer.WriteAttributeString("name", info.Name);
            writer.WriteAttributeString("class", info.ClassName);

            var rect = info.BoundingRectangle;
            writer.WriteAttributeString("visible", info.IsOffscreen ? "false" : "true");
            writer.WriteAttributeString("x", ((int)rect.X).ToString());
            writer.WriteAttributeString("y", ((int)rect.Y).ToString());
            writer.WriteAttributeString("width", ((int)rect.Width).ToString());
            writer.WriteAttributeString("height", ((int)rect.Height).ToString());

            var child = walker.GetFirstChild(parent);
            while (child != null)
            {
                this.WalkTree(child, walker, writer, elements);
                child = walker.GetNextSibling(child);
            }

            writer.WriteEndElement();
        }
    }
}