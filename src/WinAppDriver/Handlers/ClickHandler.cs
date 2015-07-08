namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using Newtonsoft.Json;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/click")]
    internal class ClickHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IMouse mouse;

        public ClickHandler(IMouse mouse)
        {
            this.mouse = mouse;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            ClickRequest request = JsonConvert.DeserializeObject<ClickRequest>(body);

            var button = MouseButton.Left; // default
            switch (request.Button)
            {
                case "1":
                    button = MouseButton.Middle;
                    break;

                case "2":
                    button = MouseButton.Right;
                    break;
            }

            this.mouse.Click(button);

            return null;
        }

        private class ClickRequest
        {
            [JsonProperty("button")]
            internal string Button { get; set; }
        }
    }
}