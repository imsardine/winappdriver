namespace WinAppDriver.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Automation;
    using Newtonsoft.Json;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/window")]
    internal class SwitchToWindowHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IUIAutomation uiAutomation;

        private IWindowFactory windowFactory;

        public SwitchToWindowHandler(IUIAutomation uiAutomation, IWindowFactory windowFactory)
        {
            this.uiAutomation = uiAutomation;
            this.windowFactory = windowFactory;
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
            this.windowFactory.GetWindow(handle).BringToFront();
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
                this.windowFactory.GetWindow(handle).BringToFront();
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

        private class SwitchToWindowRequest
        {
            [JsonProperty("name")]
            internal string HandleOrTitle { get; set; }
        }
    }
}