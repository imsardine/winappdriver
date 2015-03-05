namespace WinAppDriver
{
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Automation;
    using System.Xml;

    [Route("GET", "/session/:sessionId/source")]
    internal class GetSourceHandler : IHandler
    {
        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            var control = new PropertyCondition(AutomationElement.IsControlElementProperty, true);
            var visible = new PropertyCondition(AutomationElement.IsOffscreenProperty, false);
            var walker = new TreeWalker(new AndCondition(control, visible));

            var stringWriter = new StringWriter();
            XmlWriter writer = new XmlTextWriter(stringWriter);

            writer.WriteStartDocument();
            writer.WriteStartElement("WinAppDriver");
            this.WalkNode(AutomationElement.RootElement, walker, writer);
            writer.WriteEndDocument();

            return stringWriter.ToString();
        }

        private void WalkNode(AutomationElement element, TreeWalker walker, XmlWriter writer)
        {
            var child = walker.GetFirstChild(element);
            while (child != null)
            {
                var info = child.Current;
                writer.WriteStartElement(info.ControlType.ProgrammaticName);
                writer.WriteAttributeString("id", info.AutomationId);
                writer.WriteAttributeString("name", info.Name);
                writer.WriteAttributeString("class", info.ClassName);

                this.WalkNode(child, walker, writer);
                writer.WriteEndElement();

                child = walker.GetNextSibling(child);
            }
        }
    }
}