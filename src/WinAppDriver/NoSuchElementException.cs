namespace WinAppDriver
{
    using System;
    using System.Text;

    internal class NoSuchElementException : FailedCommandException
    {
        public NoSuchElementException() : base() { }

        public NoSuchElementException(string strategy, string locator) : this(strategy, locator, null) { }

        public NoSuchElementException(string strategy, string locator, Exception inner) :
            base(FormatMessage(strategy, locator), 7, inner)
        {
        }

        private static string FormatMessage(string strategy, string locator)
        {
            return String.Format("Not found; strategy = {0}, locator = {1}.", strategy, locator);
        }

        public string Strategy { get; set; }

        public string Locator { get; set; }
    }
}
