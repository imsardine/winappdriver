namespace WinAppDriver
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal class EndPoint
    {
        private static Regex paramRegex = new Regex("/:([^/]+)");

        private string method;

        private string pattern;

        private Regex regex;

        public EndPoint(string method, string pattern, IHandler handler)
        {
            this.method = method;
            this.pattern = pattern;
            this.regex = this.ToRegex(pattern);
            this.Handler = handler;
        }

        public IHandler Handler { get; private set; }

        public bool IsMatch(string method, string path, out Dictionary<string, string> urlParams)
        {
            urlParams = null;
            if (method != this.method)
            {
                return false;
            }

            var match = this.regex.Match(path);
            if (!match.Success)
            {
                return false;
            }

            urlParams = new Dictionary<string, string>();
            foreach (string name in this.regex.GetGroupNames())
            {
                urlParams[name] = match.Groups[name].Value;
            }

            return true;
        }

        private Regex ToRegex(string pattern)
        { // TODO no /wd/hub/ hard coded
            string pattern_converted = string.Format(
                "^/wd/hub{0}$",
                paramRegex.Replace(pattern, "/(?<${1}>[^/]+)"));
            Console.WriteLine("Pattern conversion; {0} => {1}", pattern, pattern_converted);
            return new Regex(pattern_converted);
        }
    }
}