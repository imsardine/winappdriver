namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Input;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/element/:id/click")]
    internal class ClickElementHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IMouse mouse;

        private IOverlay overlay;

        private IElementFactory elementFactory;

        public ClickElementHandler(IMouse mouse, IOverlay overlay, IElementFactory elementFactory)
        {
            this.mouse = mouse;
            this.overlay = overlay;
            this.elementFactory = elementFactory;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var id = int.Parse(urlParams["id"]);
            var element = this.elementFactory.GetElement(session.GetUIElement(id));

            if (!element.Visible)
            {
                throw new FailedCommandException("The element is not visible.", 11); // ElementNotVisible
            }

            int x = element.X.Value + (element.Width.Value / 2);
            int y = element.Y.Value + (element.Height.Value / 2);

            this.overlay.Clear();
            this.overlay.Target = new Point(x, y);
            this.overlay.ShowAndWait(session.Capabilities.OverlayTargetDelay);

            this.mouse.MoveTo(x, y);
            this.mouse.Click(MouseButton.Left);

            return null;
        }
    }
}