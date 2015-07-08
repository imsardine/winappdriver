namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/element/:id/click")]
    internal class ClickElementHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IMouse mouse;

        private IElementFactory elementFactory;

        public ClickElementHandler(IMouse mouse, IElementFactory elementFactory)
        {
            this.mouse = mouse;
            this.elementFactory = elementFactory;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var id = int.Parse(urlParams["id"]);
            var element = this.elementFactory.GetElement(session.GetUIElement(id));

            int x = element.X + (element.Width / 2);
            int y = element.Y + (element.Height / 2);

            this.mouse.MoveTo(x, y);
            this.mouse.Click(MouseButton.Left);

            return null;
        }
    }
}