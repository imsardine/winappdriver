namespace WinAppDriver.UI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    internal class Overlay : IOverlay
    {
        private OverlayForm form;
        private Thread uiThread;

        public IElement ContextElement
        {
            set
            {
                this.form.ContextElement = value;
            }
        }

        public IElement HighlightedElement
        {
            set
            {
                var elements = this.form.HighlightedElements;
                elements.Clear();

                if (value != null)
                {
                    elements.Add(value);
                }
            }
        }

        public IList<IElement> HighlightedElements
        {
            set
            {
                var elements = this.form.HighlightedElements;
                elements.Clear();

                foreach (var element in value)
                {
                    elements.Add(element);
                }
            }
        }

        public Point? PointTo
        {
            set
            {
                this.form.PointTo = value;
            }
        }

        public void Open()
        {
            this.form = new OverlayForm();
            this.uiThread = new Thread(new ThreadStart(this.Start));
            this.uiThread.Name = "UI";
            this.uiThread.Start();
        }

        public void Clear()
        {
            this.HighlightedElement = null;
            this.PointTo = null;
        }

        public void Show()
        {
            this.form.Invoke(new Action(delegate()
            {
                this.form.Invalidate();
                this.form.Show();
            }));

            Thread.Sleep(3000);

            this.form.Invoke(new Action(delegate()
            {
                this.form.Hide();
            }));
        }

        private void Start()
        {
            Application.EnableVisualStyles();
            Application.Run(this.form);
        }
    }
}