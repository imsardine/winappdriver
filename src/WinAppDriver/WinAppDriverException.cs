namespace WinAppDriver
{
    using System;

    public class WinAppDriverException : Exception
    {
        public WinAppDriverException()
            : base()
        {
        }

        public WinAppDriverException(string message)
            : base(message)
        {
        }

        public WinAppDriverException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}