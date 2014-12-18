using System;

namespace WinAppDriver {

    class RouteAttribute : Attribute {

        public RouteAttribute(string method, string pattern) {
            Method = method;
            Pattern = pattern;
        }

        public string Method { get; private set; }
 
        public string Pattern { get; private set; }

    }

}

