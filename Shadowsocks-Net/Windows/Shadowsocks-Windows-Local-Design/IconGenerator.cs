/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Shadowsocks_Windows_Local
{
    static class IconGenerator
    {
        public static void GetBitmap(out Bitmap bitmap, Color fgColor, Color bgColor = default)
        {
            using (var rawImg = GetImage(fgColor))
            {
                var size = Math.Max(rawImg.Width, rawImg.Height);
                bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);

                using (var g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.Clear(default == bgColor ? Color.Transparent : bgColor);

                    g.DrawImage(rawImg, (size - rawImg.Width) / 2F, (size - rawImg.Height) / 2F);

                }
            }
        }

        public static void GetIcon(out Icon icon, Color fgColor, Color bgColor = default)
        {
            GetBitmap(out Bitmap bitmap, fgColor, bgColor);
            icon = Icon.FromHandle(bitmap.GetHicon());
        }

        public static GraphicsPath GetGraphicsPath()
        {
            var path = new GraphicsPath();
            path.AddPolygon(new PointF[]
            {
                new PointF(0, 165),
                new PointF(331, 0),
                new PointF(270, 254),
                new PointF(143, 212.14F),
                new PointF(256, 77),
                new PointF(106.18F,200),
            });

            path.AddPolygon(new PointF[]
            {
                new PointF(186.05F, 241.33F),
                new PointF(143, 227.13F),
                new PointF(143, 313),
            });
            return path;
        }

        static Bitmap GetImage(Color color)
        {
            var bmp = new Bitmap(331, 313, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Clear(Color.Transparent);

                using (var path = GetGraphicsPath())
                {
                    using (Brush b = new SolidBrush(color))
                    {
                        g.FillPath(b, path);
                    }
                }
            }


            return bmp;
        }
    }
}
