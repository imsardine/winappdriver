namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/moveto")]
    internal class MoveToHandler : IHandler
    {
        private IMouse mouse;

        private IElementFactory elementFactory;

        public MoveToHandler(IMouse mouse, IElementFactory elementFactory)
        {
            this.mouse = mouse;
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

                int x = 0, y = 0;
                if (request.XOffset == null)
                {
                    x = element.X + (element.Width / 2);
                    y = element.Y + (element.Height / 2);
                }
                else
                {
                    x = element.X + int.Parse(request.XOffset);
                    y = element.Y + int.Parse(request.YOffset);
                }

                this.mouse.MoveTo(x, y);
            }
            else
            {
                // relative to current position of the mouse
                int x = int.Parse(request.XOffset);
                int y = int.Parse(request.YOffset);

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