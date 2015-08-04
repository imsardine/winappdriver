namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using System.Drawing;
    using Newtonsoft.Json;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/moveto")]
    internal class MoveToHandler : IHandler
    {
        private IMouse mouse;

        private IOverlay overlay;

        private IElementFactory elementFactory;

        public MoveToHandler(IMouse mouse, IOverlay overlay, IElementFactory elementFactory)
        {
            this.mouse = mouse;
            this.overlay = overlay;
            this.elementFactory = elementFactory;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var request = JsonConvert.DeserializeObject<MoveToRequest>(body);

            if (request.ID != null)
            {
                // TODO not visible? scroll into view
                var id = int.Parse(request.ID);
                var element = this.elementFactory.GetElement(session.GetUIElement(id));

                if (!element.Bounds.HasValue)
                {
                    throw new FailedCommandException("Element's bounds are unknown.", 29); // InvalidElementCoordinates
                }

                int x = 0, y = 0;
                if (request.XOffset == null)
                {
                    x = element.X.Value + (element.Width.Value / 2);
                    y = element.Y.Value + (element.Height.Value / 2);
                }
                else
                {
                    x = element.X.Value + int.Parse(request.XOffset);
                    y = element.Y.Value + int.Parse(request.YOffset);
                }

                this.overlay.Clear();
                this.overlay.Target = new Point(x, y);
                this.overlay.ShowAndWait(session.Capabilities.OverlayTargetDelay);

                this.mouse.MoveTo(x, y);
            }
            else
            {
                // relative to current position of the mouse
                int x = int.Parse(request.XOffset);
                int y = int.Parse(request.YOffset);

                Point pos = this.mouse.Position;
                this.overlay.Clear();
                this.overlay.Target = new Point(pos.X + x, pos.Y + y);
                this.overlay.ShowAndWait(session.Capabilities.OverlayTargetDelay);

                this.mouse.Move(x, y);
            }

            return null;
        }

        private class MoveToRequest
        {
            [JsonProperty("element")]
            internal string ID { get; set; }

            [JsonProperty("xoffset")]
            internal string XOffset { get; set; }

            [JsonProperty("yoffset")]
            internal string YOffset { get; set; }
        }
    }
}