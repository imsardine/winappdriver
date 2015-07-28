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

        private IWindowUtils windowUtils;

        public SwitchToWindowHandler(IUIAutomation uiAutomation, IWindowFactory windowFactory, IWindowUtils windowUtils)
        {
            this.uiAutomation = uiAutomation;
            this.windowFactory = windowFactory;
            this.windowUtils = windowUtils;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var request = JsonConvert.DeserializeObject<SwitchToWindowRequest>(body);

            if (request.HandleOrTitle == null)
            {
                session.FocusOnCurrentWindow = false;
                return null;
            }

            if (this.IsValidWindowHandle(request.HandleOrTitle))
            {
                var handle = new IntPtr(int.Parse(request.HandleOrTitle));
                this.SwitchByWindowHandle(handle);
            }
            else
            {
                this.SwitchByWindowTitle(request.HandleOrTitle);
            }

            session.FocusOnCurrentWindow = true;
            return null;
        }

        private bool IsValidWindowHandle(string handleOrTitle)
        {
            int handle;
            if (int.TryParse(handleOrTitle, out handle))
            {
                return this.windowUtils.GetTopLevelWindowHandles().Contains(new IntPtr(handle));
            }
            else
            {
                return false;
            }
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
                int handle;
                if (int.TryParse(title, out handle))
                {
                    // modern apps are not in the list of top-level windows, why?
                    this.SwitchByWindowHandle(new IntPtr(handle));
                }
                else
                {
                    // TODO no such window error
                }
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