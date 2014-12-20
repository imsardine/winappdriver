using System;
using System.Collections.Generic;

namespace WinAppDriver {

    class RequestManager {

        private SessionManager sessionManager;

        private Dictionary<string, List<EndPoint>> endpoints;

        public RequestManager(SessionManager sessionManager) {
           this.sessionManager = sessionManager;
           endpoints = new Dictionary<string, List<EndPoint>> {
               { "GET", new List<EndPoint>() },
               { "POST", new List<EndPoint>() },
               { "DELETE", new List<EndPoint>() },
           };
        }

        public void AddHandler(IHandler handler) {
            // TODO register all handlers automatically, with the help of reflection
            var route = GetRoute(handler);
            endpoints[route.Method].Add(new EndPoint(route.Method, route.Pattern, handler));
        }

        private RouteAttribute GetRoute(object handler) {
            Attribute[] attributes = Attribute.GetCustomAttributes(handler.GetType());
            foreach (var attr in attributes) {
                if (attr is RouteAttribute)
                    return (RouteAttribute)attr;
            }

            return null; // TODO throw exception (programming error)
        }

        public object Handle(string method, string path, string body, out Session session) {
            session = null;
            Dictionary<string, string> urlParams = null;

            foreach (var endpoint in endpoints[method]) {
                if (endpoint.IsMatch(method, path, out urlParams)) {
                    var handler = endpoint.Handler;
                    Console.WriteLine("A corresponding endpoint found: {0}", handler.GetType().FullName);

                    if (urlParams.ContainsKey("sessionId")) {
                        session = sessionManager[urlParams["sessionId"]]; // TODO invalid session?
                    }

                    return endpoint.Handler.Handle(urlParams, body, ref session);
                }
            }

            return null; // TODO throw exception (command not supported)
        }

    }

}

