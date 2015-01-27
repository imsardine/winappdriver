namespace WinAppDriver
{
    using System;
    using System.Diagnostics;

    internal interface ILogger
    {
        bool IsFatalEnabled();

        bool IsErrorEnabled();

        bool IsWarnEnabled();

        bool IsInfoEnabled();

        bool IsDebugEnabled();

        void Fatal(string message);

        void Fatal(string message, params object[] args);

        void Fatal(string message, Exception exception);

        void Error(string message);

        void Error(string message, params object[] args);

        void Error(string message, Exception exception);

        void Warn(string message);

        void Warn(string message, params object[] args);

        void Warn(string message, Exception exception);

        void Info(string message);

        void Info(string message, params object[] args);

        void Info(string message, Exception exception);

        void Debug(string message);

        void Debug(string message, params object[] args);

        void Debug(string message, Exception exception);
    }

    internal class Logger : ILogger
    {
        private TraceSource trace;

        private Logger(string name)
        {
            this.trace = new TraceSource(name);
        }

        public static Logger GetLogger(string name)
        {
            return new Logger(name);
        }

        public bool IsFatalEnabled()
        {
            return this.trace.Switch.ShouldTrace(TraceEventType.Critical);
        }

        public bool IsErrorEnabled()
        {
            return this.trace.Switch.ShouldTrace(TraceEventType.Error);
        }

        public bool IsWarnEnabled()
        {
            return this.trace.Switch.ShouldTrace(TraceEventType.Warning);
        }

        public bool IsInfoEnabled()
        {
            return this.trace.Switch.ShouldTrace(TraceEventType.Information);
        }

        public bool IsDebugEnabled()
        {
            return this.trace.Switch.ShouldTrace(TraceEventType.Verbose);
        }

        public void Fatal(string message)
        {
            this.trace.TraceEvent(TraceEventType.Critical, 1, message);
        }

        public void Fatal(string message, params object[] args)
        {
            if (this.IsFatalEnabled())
            {
                this.Fatal(string.Format(message, args));
            }
        }

        public void Fatal(string message, Exception exception)
        {
            if (this.IsFatalEnabled())
            {
                message += "\n" + exception.ToString();
                this.Fatal(message);
            }
        }

        public void Error(string message)
        {
            this.trace.TraceEvent(TraceEventType.Error, 2, message);
        }

        public void Error(string message, params object[] args)
        {
            if (this.IsErrorEnabled())
            {
                this.Error(string.Format(message, args));
            }
        }

        public void Error(string message, Exception exception)
        {
            if (this.IsErrorEnabled())
            {
                message += "\n" + exception.ToString();
                this.Error(message);
            }
        }

        public void Warn(string message)
        {
            this.trace.TraceEvent(TraceEventType.Warning, 3, message);
        }

        public void Warn(string message, params object[] args)
        {
            if (this.IsWarnEnabled())
            {
                this.Warn(string.Format(message, args));
            }
        }

        public void Warn(string message, Exception exception)
        {
            if (this.IsWarnEnabled())
            {
                message += "\n" + exception.ToString();
                this.Warn(message);
            }
        }

        public void Info(string message)
        {
            this.trace.TraceEvent(TraceEventType.Information, 4, message);
        }

        public void Info(string message, params object[] args)
        {
            if (this.IsInfoEnabled())
            {
                this.Info(string.Format(message, args));
            }
        }

        public void Info(string message, Exception exception)
        {
            if (this.IsInfoEnabled())
            {
                message += "\n" + exception.ToString();
                this.Info(message);
            }
        }

        public void Debug(string message)
        {
            this.trace.TraceEvent(TraceEventType.Verbose, 5, message);
        }

        public void Debug(string message, params object[] args)
        {
            if (this.IsDebugEnabled())
            {
                this.Debug(string.Format(message, args));
            }
        }

        public void Debug(string message, Exception exception)
        {
            if (this.IsDebugEnabled())
            {
                message += "\n" + exception.ToString();
                this.Debug(message);
            }
        }
    }
}