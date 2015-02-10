namespace WinAppDriver
{
    using System;

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
            var utils = new Utils();
            var manager = new RequestManager(sessionManager);
            manager.AddHandler(new ClickElementHandler());
            manager.AddHandler(new DeleteSessionHandler(sessionManager));
            manager.AddHandler(new FindElementHandler());
            manager.AddHandler(new FindElementsHandler());
            manager.AddHandler(new GetElementTextHandler());
            manager.AddHandler(new NewSessionHandler(sessionManager, utils));
            manager.AddHandler(new ScreenshotHandler());
            manager.AddHandler(new SetElementValueHandler());

            return manager;
        }
    }
}