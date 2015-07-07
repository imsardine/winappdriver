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
        private static IDictionary<ControlType, string> controlTypes =
            new Dictionary<ControlType, string>();

        private static IDictionary<string, ControlType> tagNames =
            new Dictionary<string, ControlType>();

        private IElementFactory elementFactory;

        static UIAutomation()
        {
            var types = new List<ControlType>
            {
                ControlType.Button, ControlType.Calendar,
                ControlType.CheckBox, ControlType.ComboBox,
                ControlType.Custom, ControlType.DataGrid,
                ControlType.DataItem, ControlType.Document,
                ControlType.Edit, ControlType.Group,
                ControlType.Header, ControlType.HeaderItem,
                ControlType.Hyperlink, ControlType.Image,
                ControlType.List, ControlType.ListItem,
                ControlType.Menu, ControlType.MenuBar,
                ControlType.MenuItem, ControlType.Pane,
                ControlType.ProgressBar, ControlType.RadioButton,
                ControlType.ScrollBar, ControlType.Separator,
                ControlType.Slider, ControlType.Spinner,
                ControlType.SplitButton, ControlType.StatusBar,
                ControlType.Tab, ControlType.TabItem,
                ControlType.Table, ControlType.Text,
                ControlType.Thumb, ControlType.TitleBar,
                ControlType.ToolBar, ControlType.ToolTip,
                ControlType.Tree, ControlType.TreeItem,
                ControlType.Window
            };

            foreach (var type in types)
            {
                var tag = type.ProgrammaticName.Substring(12); // ControlType.
                controlTypes.Add(new KeyValuePair<ControlType, string>(type, tag));
                tagNames.Add(new KeyValuePair<string, ControlType>(tag, type));
            }
        }

        public UIAutomation(IElementFactory elementFactory)
        {
            this.elementFactory = elementFactory;
        }

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

        public ControlType FromTagName(string tag)
        {
            return tagNames[tag];
        }

        public string ToTagName(ControlType type)
        {
            return controlTypes[type];
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
            var element = this.elementFactory.GetElement(parent);
            writer.WriteStartElement(element.TypeName);
            if (elements != null)
            {
                writer.WriteAttributeString("_index_", elements.Count.ToString());
                elements.Add(parent);
            }

            writer.WriteAttributeString("id", element.ID);
            writer.WriteAttributeString("name", element.Name);
            writer.WriteAttributeString("class", element.ClassName);

            writer.WriteAttributeString("visible", element.Visible ? "true" : "false");
            writer.WriteAttributeString("enabled", element.Enabled ? "true" : "false");
            writer.WriteAttributeString("selected", element.Selected ? "true" : "false");

            writer.WriteAttributeString("x", element.X.ToString());
            writer.WriteAttributeString("y", element.Y.ToString());
            writer.WriteAttributeString("width", element.Width.ToString());
            writer.WriteAttributeString("height", element.Height.ToString());
            writer.WriteAttributeString(
                "bounds",
                string.Format("[{0},{1}][{2},{3}]", element.X, element.Y, element.Width, element.Height));

            var child = walker.GetFirstChild(parent);
            while (child != null)
            {
                this.WalkTree(child, walker, writer, elements);
                child = walker.GetNextSibling(child);
            }

            writer.WriteEndElement();
        }

        private bool IsElementSelected(AutomationElement element)
        {
            object pattern;
            if (element.TryGetCurrentPattern(TogglePattern.Pattern, out pattern))
            {
                var state = ((TogglePattern)pattern).Current.ToggleState;
                return state != ToggleState.Off;
            }
            else
            {
                return false;
            }
        }
    }
}
