namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using Newtonsoft.Json;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/buttonup")]
    internal class ButtonUpHandler : IHandler
    {
        private IMouse mouse;

        public ButtonUpHandler(IMouse mouse)
        {
            this.mouse = mouse;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var request = JsonConvert.DeserializeObject<ButtonUpRequest>(body);

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

            this.mouse.Up(button);

            return null;
        }

        private class ButtonUpRequest
        {
            [JsonProperty("button")]
            internal string Button { get; set; }
        }
    }
}