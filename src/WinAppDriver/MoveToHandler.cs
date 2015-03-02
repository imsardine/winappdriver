namespace WinAppDriver
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Windows;
    using WinUserWrapper;

    [Route("POST", "/session/:sessionId/moveto")]
    class MoveToHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");
        
        private IWinUserWrap winUser = new WinUserWrap();
        
        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            ElementRequest request = JsonConvert.DeserializeObject<ElementRequest>(body);

            var element = session.GetUIElement(int.Parse(request.Element));
            Rect rect = element.Current.BoundingRectangle;

            int x = (int)(rect.Left + (rect.Width / 2));
            int y = (int)(rect.Top + (rect.Height / 2));

            logger.Info("click " + element.Current.Name);
            this.winUser.SetCursorPos(x, y);
            return null;
        }

        private class ElementRequest
        {
            [JsonProperty("element")]
            internal string Element { get; set; }
        }
    }
}
