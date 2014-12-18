using System;
using System.Collections.Generic;

namespace WinAppDriver {

    class SessionManager {

        private Dictionary<string, Session> sessions;

        public SessionManager() {
            sessions = new Dictionary<string, Session>();
        }
        
        public Session CreateSession(Capabilities capabilities) {
            var id = Guid.NewGuid().ToString();
            var session = new Session(id, capabilities);
            sessions[id] = session;
            return session;
        }
        
        public Session this[string id] {
            get { return sessions[id]; }
        }
        
    }

}

