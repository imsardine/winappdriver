namespace WinAppDriver
{
    using System;

    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class RouteAttribute : Attribute
    {
        public RouteAttribute(string method, string pattern)
        {
            this.Method = method;
            this.Pattern = pattern;
        }

        public string Method { get; private set; }

        public string Pattern { get; private set; }
    }
}