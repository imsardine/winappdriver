namespace WinAppDriver.UAC
{
    using System.Threading;
    using System.Windows.Automation;
    using System.Windows.Input;

    internal class UACPromptHandler : IUACPomptHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IUIAutomation uiAutomation;

        private IKeyboard keyboard;

        private Thread threadLazy;

        private bool stopped;

        private bool alive;

        private bool allowed; // true = Yes, false = No

        public UACPromptHandler(IUIAutomation uiAutomation, IKeyboard keyboard)
        {
            this.uiAutomation = uiAutomation;
            this.keyboard = keyboard;
            this.threadLazy = null;
            this.stopped = false;
            this.alive = false;
            this.allowed = false;
        }

        public void Activate(bool allowed)
        {
            logger.Info("Requested to activate the UAC prompt handler; allowed = {0}", allowed);
            this.alive = true;
            this.allowed = allowed;
            var thread = this.GetThread();

            thread.Interrupt();
        }

        public void Deactivate()
        {
            logger.Info("Requested to deactivate the UAC prompt handler.");
            var thread = this.GetThread();
            this.alive = false;
        }

        private void Handle()
        {
            while (!this.stopped)
            {
                while (!this.stopped && this.alive)
                {
                    try
                    {
                        if (this.DismissUACPrompts())
                        {
                            this.alive = false;

                            // wait for the next activation
                            Thread.Sleep(Timeout.Infinite);
                        }
                        else
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    catch (ThreadInterruptedException) 
                    {
                    }
                }
            }
        }

        private bool DismissUACPrompts()
        {
            logger.Info("Trying to find the (focused) UAC elevation prompt.");

            AutomationElement window = null;
            if (this.uiAutomation.TryGetFocusedWindowOrRoot(out window))
            {
                var conditions = new AndCondition(
                    Automation.ContentViewCondition,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane),
                    new PropertyCondition(AutomationElement.ClassNameProperty, "CtrlNotifySink"));

                var elements = window.FindAll(TreeScope.Descendants, conditions);
                if (elements.Count > 0)
                {
                    logger.Info("The UAC elevation prompt found.");

                    // sending keystrokes (Alt + Y / N) is more reliable.
                    this.keyboard.ReleaseAllModifierKeys();
                    this.keyboard.KeyUpOrDown(Key.LeftAlt);
                    this.keyboard.KeyPress(this.allowed ? Key.Y : Key.N);
                    this.keyboard.KeyUpOrDown(Key.LeftAlt);

                    return true;
                }
            }

            return false;
        }

        private Thread GetThread()
        {
            if (this.threadLazy == null)
            {
                var thread = new Thread(this.Handle);
                thread.Name = "UACPromptHandler";
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                this.threadLazy = thread;
                logger.Info("The thread for handling UAC prompts created.");
            }

            return this.threadLazy;
        }
    }
}