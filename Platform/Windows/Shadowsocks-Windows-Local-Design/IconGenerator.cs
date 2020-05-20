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
        public static Icon GetIcon(int size, Color fgColor, Color bgColor = default)
        {
            var img = GetImage(size, fgColor, bgColor);
            return Icon.FromHandle(img.GetHicon());
        }

        public static Bitmap GetImage(int size, Color fgColor, Color bgColor = default)
        {
            using (var rawImg = GetImage(fgColor, bgColor))
            {
                var newImg = new Bitmap(size, size, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(newImg))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.Clear(default == bgColor ? Color.Transparent : bgColor);

                    g.DrawImage(rawImg, new Rectangle(0, 0, size, size), new Rectangle(0, 0, rawImg.Width, rawImg.Height), GraphicsUnit.Pixel);
                }
                return newImg;
            }
        }

        public static Bitmap GetImage(Color fgColor, Color bgColor = default)
        {
            var bmp = new Bitmap(331, 331, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Clear(default == bgColor ? Color.Transparent : bgColor);

                using (var path = GetSquareGraphicsPath())
                {
                    using (Brush b = new SolidBrush(fgColor))
                    {
                        g.FillPath(b, path);
                    }
                }
            }

            return bmp;
        }

        static GraphicsPath GetSquareGraphicsPath()
        {
            var path = new GraphicsPath();
            path.AddPolygon(new PointF[]
            {
                new PointF(0, 165+9),
                new PointF(331, 0+9),
                new PointF(270, 254+9),
                new PointF(143, 212.14F+9),
                new PointF(256, 77+9),
                new PointF(106.18F,200+9),
            });

            path.AddPolygon(new PointF[]
            {
                new PointF(186.05F, 241.33F+9),
                new PointF(143, 227.13F+9),
                new PointF(143, 313+9),
            });
            return path;
        }

        static GraphicsPath GetRawGraphicsPath()
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
    }
}
