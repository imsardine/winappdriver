namespace WinAppDriver.UI
{
    using System;
    using System.Windows;
    using System.Windows.Automation;

    internal class Element : IElement
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private AutomationElement element;

        private IUIAutomation uiAutomation;

        private AutomationElement.AutomationElementInformation? infoCache;

        private Rect? rectCache;

        public Element(AutomationElement element, IUIAutomation uiAutomation)
        {
            this.element = element;
            this.uiAutomation = uiAutomation;
        }

        public AutomationElement AutomationElement
        {
            get { return this.element; }
        }

        public string UIFramework
        {
            get
            {
                // "Win32", "WinForm", "DirectUI", "XAML", "WPF", etc.
                return this.Info.FrameworkId;
            }
        }

        public string ID
        {
            get { return this.Info.AutomationId; }
        }

        public IntPtr Handle
        {
            get { return new IntPtr(this.Info.NativeWindowHandle); }
        }

        public string TypeName
        {
            get
            {
                // ControlType.[TypeName]
                return this.Info.ControlType.ProgrammaticName.Substring(12);
            }
        }

        public string Name
        {
            get { return this.Info.Name; }
        }

        public string Value
        {
            get
            {
                object pattern = null;
                if (this.element.TryGetCurrentPattern(ValuePattern.Pattern, out pattern))
                {
                    return ((ValuePattern)pattern).Current.Value;
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                object pattern = null;
                if (this.element.TryGetCurrentPattern(ValuePattern.Pattern, out pattern))
                {
                    ((ValuePattern)pattern).SetValue(value);
                }
            }
        }

        public string ClassName
        {
            get { return this.Info.ClassName; }
        }

        public string Help
        {
            get { return this.Info.HelpText; }
        }

        public bool Focusable
        {
            get { return this.Info.IsKeyboardFocusable; }
        }

        public bool Focused
        {
            get { return this.Info.HasKeyboardFocus; }
        }

        public bool Visible
        {
            get { return !this.Info.IsOffscreen; }
        }

        public bool Enabled
        {
            get { return this.Info.IsEnabled; }
        }

        public bool Selected
        {
            get
            {
                object pattern;
                if (this.element.TryGetCurrentPattern(TogglePattern.Pattern, out pattern))
                {
                    var state = ((TogglePattern)pattern).Current.ToggleState;
                    return state != ToggleState.Off;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Protected
        {
            get { return this.Info.IsPassword; }
        }

        public bool Scrollable
        {
            get
            {
                object pattern;
                return this.element.TryGetCurrentPattern(ScrollPattern.Pattern, out pattern);
            }
        }

        public int X
        {
            get
            {
                double x = this.Rect.X;
                return double.IsInfinity(x) ? int.MaxValue : (int)x;
            }
        }

        public int Y
        {
            get
            {
                double y = this.Rect.Y;
                return double.IsInfinity(y) ? int.MaxValue : (int)y;
            }
        }

        public int Width
        {
            get
            {
                double width = this.Rect.Width;
                return double.IsInfinity(width) ? -1 : (int)width;
            }
        }

        public int Height
        {
            get
            {
                double height = this.Rect.Height;
                return double.IsInfinity(height) ? -1 : (int)height;
            }
        }

        private AutomationElement.AutomationElementInformation Info
        {
            get
            {
                if (this.infoCache == null)
                {
                    this.infoCache = this.element.Current;
                }

                return this.infoCache.Value;
            }
        }

        private Rect Rect
        {
            get
            {
                if (this.rectCache == null)
                {
                    this.rectCache = this.Info.BoundingRectangle;
                }

                return this.rectCache.Value;
            }
        }

        public void ScrollIntoView()
        {
            var container = this.uiAutomation.GetScrollableContainer(this);
            if (container == null)
            {
                return;
            }

            var scrollbar = (ScrollPattern)container.AutomationElement.GetCurrentPattern(
                ScrollPattern.Pattern);

            if (this.Width == -1 || this.Height == -1)
            {
                // invisible elements in some apps (XAML-based only?) may report
                // double.Infinite as their x/y coordinates and dimensions.
                try
                {
                    scrollbar.SetScrollPercent(0, ScrollPattern.NoScroll);
                }
                catch (InvalidOperationException)
                {
                }

                try
                {
                    scrollbar.SetScrollPercent(ScrollPattern.NoScroll, 0);
                }
                catch (InvalidOperationException)
                {
                }

                this.ResetCache();
            }

            // to reveal top, bottom, left and right border respectively.
            bool up = this.Y <= container.Y;
            bool down = this.Y + this.Height >= container.Y + container.Height;
            bool left = this.X <= container.X;
            bool right = this.X + this.Width >= container.X + container.Width;

            if (left ^ right)
            {
                try
                {
                    this.ScrollIntoView(container, scrollbar, true, right);
                }
                catch (InvalidOperationException)
                {
                }
            }

            if (up ^ down)
            {
                try
                {
                    this.ScrollIntoView(container, scrollbar, false, down);
                }
                catch (InvalidOperationException)
                {
                }
            }
        }

        private void ResetCache()
        {
            this.infoCache = null;
            this.rectCache = null;
        }

        private void ScrollIntoView(IElement container, ScrollPattern scrollbar, bool horizontally, bool incrementally)
        {
            // TODO To make scrolling more efficient, make big steps in the beginning.
            var step = incrementally ? ScrollAmount.SmallIncrement : ScrollAmount.SmallDecrement;

            bool done = false;
            do
            {
                // dimensions of partially displayed elements only reveal visible parts. (XAML-based only)
                int before = horizontally ? this.Width : this.Height;
                if (horizontally)
                {
                    scrollbar.Scroll(step, ScrollAmount.NoAmount);
                }
                else
                {
                    scrollbar.Scroll(ScrollAmount.NoAmount, step);
                }

                System.Threading.Thread.Sleep(20); // or the following readings could be stale
                this.ResetCache();
                int after = horizontally ? this.Width : this.Height;

                if (horizontally)
                {
                    if (incrementally)
                    {
                        done = this.X + this.Width <= container.X + container.Width;
                    }
                    else
                    {
                        done = this.X >= container.X;
                    }
                }
                else
                {
                    if (incrementally)
                    {
                        done = this.Y + this.Height <= container.Y + container.Height;
                    }
                    else
                    {
                        done = this.Y >= container.Y;
                    }
                }

                done = done && (after <= before); // leaving the element

                logger.Debug(
                    "Done? {0}; element [{1},{2}][{3},{4}] in the scrollable container [{5},{6}][{7},{8}]",
                    done, this.X, this.Y, this.Width, this.Height, container.X, container.Y, container.Width, container.Height);
            } 
            while (!done);
        }
    }
}