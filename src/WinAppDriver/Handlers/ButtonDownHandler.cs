namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using Newtonsoft.Json;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/buttondown")]
    internal class ButtonDownHandler : IHandler
    {
        private IMouse mouse;

        public ButtonDownHandler(IMouse mouse)
        {
            this.mouse = mouse;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var request = JsonConvert.DeserializeObject<ButtonDownRequest>(body);

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

            this.mouse.Down(button);

            return null;
        }

        private class ButtonDownRequest
        {
            [JsonProperty("button")]
            internal string Button { get; set; }
        }
    }
}