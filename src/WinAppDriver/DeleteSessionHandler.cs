namespace WinAppDriver
{
    using System.Collections.Generic;

    [Route("DELETE", "/session/:sessionId")]
    internal class DeleteSessionHandler : IHandler
    {
        private SessionManager sessionManager;

        public DeleteSessionHandler(SessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            var app = new StoreApplication(session.Capabilities.AppUserModelId);
            app.Terminate();
            this.sessionManager.DeleteSession(session.ID);
            app.Restore();

            return null;
        }
    }
}