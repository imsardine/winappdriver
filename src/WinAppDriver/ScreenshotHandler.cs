namespace WinAppDriver
{
    using System;
    using System.IO;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Windows.Forms;
    using System.Collections.Generic;

    [Route("GET", "/session/:sessionId/screenshot")]
    internal class ScreenshotHandler : IHandler
    {
        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            return Convert.ToBase64String(TakeScreenshotAsPng());
        }

        private byte[] TakeScreenshotAsPng()
        {
            var bounds = Screen.PrimaryScreen.Bounds;
            using (var bmp = new Bitmap(bounds.Width, bounds.Height))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    var stream = new MemoryStream();
                    bmp.Save(stream, ImageFormat.Png);
                    return stream.ToArray();
                }
            }
        }
    }
}
