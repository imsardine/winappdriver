namespace WinAppDriver
{
    using System;

    public class FailedCommandException : Exception
    {
        public FailedCommandException()
            : base()
        {
        }

        public FailedCommandException(string message, int code)
            : this(message, code, null)
        {
        }

        public FailedCommandException(string message, int code, Exception inner)
            : base(message, inner)
        {
            this.Code = code;
        }

        public int Code { get; set; }
    }
}