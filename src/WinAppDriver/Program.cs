namespace WinAppDriver
{
    using System;
    using SystemWrapper.Windows.Input;
    using WinUserWrapper;

    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var sessionMgr = new SessionManager();
            var requestMgr = InitRequestManager(sessionMgr);

            var driver = new Server(requestMgr);
            driver.Start();
        }

        private static RequestManager InitRequestManager(SessionManager sessionManager)
        {
            var manager = new RequestManager(sessionManager);
            var utils = new Utils();
            var keyboard = new Keyboard(new KeyboardWrap(), new KeyInteropWrap(), new WinUserWrap());
            var uiAutomation = new UIAutomation();

            manager.AddHandler(new ClickElementHandler());
            manager.AddHandler(new DeleteSessionHandler(sessionManager));
            manager.AddHandler(new FindElementHandler(uiAutomation));
            manager.AddHandler(new FindElementsHandler(uiAutomation));
            manager.AddHandler(new GetElementTextHandler());
            manager.AddHandler(new GetSourceHandler(uiAutomation));
            manager.AddHandler(new NewSessionHandler(sessionManager, utils));
            manager.AddHandler(new ScreenshotHandler());
            manager.AddHandler(new SendKeysHandler(keyboard));
            manager.AddHandler(new SetElementValueHandler());

            return manager;
        }
    }
}