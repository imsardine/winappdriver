namespace WinAppDriver.UI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    internal class OverlayForm : Form
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        public OverlayForm()
        {
            this.HighlightedElements = new List<IElement>();

            this.DoubleBuffered = true;

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

        public Point? Target { get; set; }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        private void OnShown(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var contextPen = new Pen(Color.Green, 5);
            contextPen.DashStyle = DashStyle.Dash;
            var elementPen = new Pen(Color.Red, 5);

            if (this.ContextElement != null)
            {
                g.DrawRectangle(contextPen, this.ContextElement.Bounds);
            }

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

            if (this.Target.HasValue)
            {
                Point target = this.Target.Value;
                this.DrawTarget(g, target.X, target.Y, 20);
            }

            contextPen.Dispose();
            elementPen.Dispose();
        }

        private void DrawTarget(Graphics g, int x, int y, int radius)
        {
            var pen = new Pen(Color.FromArgb(87, 85, 86), 3);
            g.DrawEllipse(pen, x - radius, y - radius, radius * 2, radius * 2);
            g.DrawEllipse(pen, x - radius + 10, y - radius + 10, (radius - 10) * 2, (radius - 10) * 2);

            pen.Width = 2;
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            g.DrawLine(pen, x, y - radius - 20, x, y - radius + 10);
            g.DrawLine(pen, x, y + radius + 20, x, y + radius - 10);
            g.DrawLine(pen, x - radius - 20, y, x - radius + 10, y);
            g.DrawLine(pen, x + radius + 20, y, x + radius - 10, y);

            pen.Width = 1;
            pen.Color = Color.FromArgb(235, 80, 49);
            g.DrawEllipse(pen, x - 3, y - 3, 6, 6);

            pen.Dispose();
        }
    }
}