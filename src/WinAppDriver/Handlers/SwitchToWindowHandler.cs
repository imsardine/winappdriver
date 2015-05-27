namespace WinAppDriver.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Automation;
    using System.Windows.Input;
    using Newtonsoft.Json;
    using WinUserWrapper;

    [Route("POST", "/session/:sessionId/window")]
    internal class SwitchToWindowHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IUIAutomation uiAutomation;

        private IWinUserWrap winUserWrap;

        private IKeyboard keyboard;

        public SwitchToWindowHandler(IUIAutomation uiAutomation, IWinUserWrap winUserWrap, IKeyboard keyboard)
        {
            this.uiAutomation = uiAutomation;
            this.winUserWrap = winUserWrap;
            this.keyboard = keyboard;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            var request = JsonConvert.DeserializeObject<SwitchToWindowRequest>(body);

            var handles = this.uiAutomation.GetTopLevelWindowHandles();
            if (handles.Contains(request.HandleOrTitle))
            {
                var handle = new IntPtr(int.Parse(request.HandleOrTitle));
                this.SwitchByWindowHandle(handle);
            }
            else
            {
                this.SwitchByWindowTitle(request.HandleOrTitle);
            }

            return null;
        }

        private void SwitchByWindowHandle(IntPtr handle)
        {
            logger.Debug("Window handle ({0}) provided.", handle);
            this.SwitchToWindow(handle);
        }

        private void SwitchByWindowTitle(string title)
        {
            logger.Debug("Window title ({0}) provided?", title);

            var candidates = new List<Tuple<AutomationElement, string>>();
            foreach (var window in this.uiAutomation.GetTopLevelWindows())
            {
                var info = window.Current;
                if (title == info.Name ||
                    title == info.Name + "[HWND:" + info.NativeWindowHandle.ToString() + "]")
                {
                    candidates.Add(new Tuple<AutomationElement, string>(window, info.Name));
                }
            }

            if (candidates.Count == 1)
            {
                logger.Debug("The (unique) window with the title ({0}) found.", title);
                var handle = this.uiAutomation.ToNativeWindowHandle(candidates[0].Item1);
                this.SwitchToWindow(handle);
            }
            else if (candidates.Count == 0)
            {
                // TODO no such window error
            }
            else
            {
                // TODO more than one windows matched -> xxxxx [HWND:nnnnnn]
            }
        }

        private void SwitchToWindow(IntPtr handle)
        {
            // The system automatically enables calls to SetForegroundWindow if the user presses the ALT key
            // http://www.codeproject.com/Tips/76427/How-to-bring-window-to-top-with-SetForegroundWindo.aspx
            try
            {
                this.keyboard.KeyUpOrDown(Key.LeftAlt);
                this.winUserWrap.SetForegroundWindow(handle);
            }
            finally
            {
                this.keyboard.KeyUpOrDown(Key.LeftAlt);
            }
        }

        private class SwitchToWindowRequest
        {
            [JsonProperty("name")]
            internal string HandleOrTitle { get; set; }
        }
    }
}