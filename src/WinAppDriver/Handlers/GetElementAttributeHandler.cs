namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using WinAppDriver.UI;

    [Route("GET", "/session/:sessionId/element/:id/attribute/:name")]
    internal class GetElementAttributeHandler : IHandler
    {
        private IElementFactory elementFactory;

        public GetElementAttributeHandler(IElementFactory elementFactory)
        {
            this.elementFactory = elementFactory;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var id = int.Parse(urlParams["id"]);
            var name = urlParams["name"];
            var element = this.elementFactory.GetElement(session.GetUIElement(id));

            switch (name)
            {
                // visible="True" enabled="True" selected="False" x="114" y="437" width="640" height="17" bounds="[114,437][640,17]">
                case "id":
                    return element.ID;

                case "name":
                    return element.Name;

                case "class":
                    return element.ClassName;

                case "visible":
                    return element.Visible ? "true" : "false";

                case "enabled":
                    return element.Enabled ? "true" : "false";

                case "selected":
                    return element.Selected ? "true" : "false";

                case "x":
                    return element.X.ToString();

                case "y":
                    return element.Y.ToString();

                case "width":
                    return element.Width.ToString();

                case "height":
                    return element.Height.ToString();

                case "bounds":
                    return string.Format("[{0},{1}][{2},{3}]", element.X, element.Y, element.Width, element.Height);

                default:
                    return null;
            }
        }
    }
}