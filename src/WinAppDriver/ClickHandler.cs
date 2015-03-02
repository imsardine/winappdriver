namespace WinAppDriver
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Windows;
    using WinUserWrapper;

    [Route("POST", "/session/:sessionId/click")]
    internal class ClickHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IWinUserWrap winUser = new WinUserWrap();

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            ClickRequest request = JsonConvert.DeserializeObject<ClickRequest>(body);

            switch (request.ButtonType)
            {
                case "0":
                    this.winUser.mouse_event((int)MOUSEEVENTF.LEFTDOWN, 0, 0, 0, 0);
                    this.winUser.mouse_event((int)MOUSEEVENTF.LEFTUP, 0, 0, 0, 0);
                    break;
                case "1":
                    this.winUser.mouse_event((int)MOUSEEVENTF.MIDDLEDOWN, 0, 0, 0, 0);
                    this.winUser.mouse_event((int)MOUSEEVENTF.MIDDLEUP, 0, 0, 0, 0);
                    break;
                case "2":
                    this.winUser.mouse_event((int)MOUSEEVENTF.RIGHTDOWN, 0, 0, 0, 0);
                    this.winUser.mouse_event((int)MOUSEEVENTF.RIGHTUP, 0, 0, 0, 0);
                    break;
                default:
                    this.winUser.mouse_event((int)MOUSEEVENTF.LEFTDOWN, 0, 0, 0, 0);
                    this.winUser.mouse_event((int)MOUSEEVENTF.LEFTUP, 0, 0, 0, 0);
                    break;
            }
            return null;
        }

        private class ClickRequest
        {
            [JsonProperty("button")]
            internal string ButtonType { get; set; }
        }
    }
}
