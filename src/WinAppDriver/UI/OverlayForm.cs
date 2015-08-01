namespace WinAppDriver.UI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Threading;
    using System.Windows.Forms;

    internal class OverlayForm : Form
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        public OverlayForm()
        {
            this.HighlightedElements = new List<IElement>();

            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;

            // TODO see through, but only works with Color.Gray?
            this.BackColor = Color.Gray;
            this.TransparencyKey = Color.Gray;

            this.Shown += this.OnShown;
            this.Paint += this.OnPaint;
        }

        public IElement ContextElement { get; set; }

        public IList<IElement> HighlightedElements { get; set; }

        public Point? PointTo { get; set; }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        private void OnShown(object sender, EventArgs e)
        {
            var g = this.CreateGraphics();
            var pen = new Pen(Color.Red, 5);
            pen.DashStyle = DashStyle.Dash;
            g.DrawRectangle(pen, this.Bounds);

            Thread.Sleep(3000);
            this.Invalidate();
            this.Hide();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            var contextPen = new Pen(Color.Green, 5);
            contextPen.DashStyle = DashStyle.Dash;
            var elementPen = new Pen(Color.Red, 5);

            g.DrawRectangle(contextPen, this.ContextElement.Bounds);

            bool numbered = this.HighlightedElements.Count > 1;
            for (int i = 0; i < this.HighlightedElements.Count; i++)
            {
                var element = this.HighlightedElements[i];
                var bounds = element.Bounds;

                elementPen.DashStyle = element.Visible ? DashStyle.Solid : DashStyle.Dash;
                g.DrawRectangle(elementPen, element.Bounds);
                if (numbered)
                {
                    g.DrawString(i.ToString(), new Font("Arial", 13), new SolidBrush(Color.Red), new PointF(bounds.X, bounds.Y));
                }
            }
        }
    }
}