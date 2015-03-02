namespace WinAppDriver
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Automation;
    using WinUserWrapper;

    [Route("POST", "/session/:sessionId/element/:id/click")]
    internal class ClickElementHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IWinUserWrap winUser = new WinUserWrap();

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            var element = session.GetUIElement(int.Parse(urlParams["id"]));
            Rect rect = element.Current.BoundingRectangle;

            int x = (int)(rect.Left + (rect.Width / 2));
            int y = (int)(rect.Top + (rect.Height / 2));

            logger.Info("click " + element.Current.Name);
            this.winUser.SetCursorPos(x, y);

            this.winUser.mouse_event((int)MOUSEEVENTF.LEFTDOWN, 0, 0, 0, 0);
            this.winUser.mouse_event((int)MOUSEEVENTF.LEFTUP, 0, 0, 0, 0);
            return null;
        }
    }
}