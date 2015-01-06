using System;
using System.Collections.Generic;

namespace WinAppDriver
{
    internal class SessionManager
    {
        private Dictionary<string, Session> sessions;

        public SessionManager()
        {
            sessions = new Dictionary<string, Session>();
        }

        public Session CreateSession(IApplication application, Capabilities capabilities)
        {
            var id = Guid.NewGuid().ToString();
            var session = new Session(id, application, capabilities);
            sessions[id] = session;
            return session;
        }

        public Session this[string id]
        {
            get { return sessions[id]; }
        }

        public void DeleteSession(string id)
        {
            sessions.Remove(id);
        }
    }
}