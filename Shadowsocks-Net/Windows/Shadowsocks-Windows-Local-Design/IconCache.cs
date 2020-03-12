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
    static class IconCache
    {
        const int SIZE_ICON = 16;
        static object _lock = new object();

        static Icon _iconDefault = null;
        static Icon _iconFast = null;
        static Icon _iconGood = null;
        static Icon _iconSlow = null;
        static Icon _iconBad = null;

        public static void Reload()
        {
            lock (_lock)
            {
                //_iconDefault?.Dispose();
                _iconDefault = null;

                //_iconFast?.Dispose();
                _iconFast = null;

                //_iconGood?.Dispose();
                _iconGood = null;

                //_iconSlow?.Dispose();
                _iconSlow = null;

                //_iconBad?.Dispose();
                _iconBad = null;
            }
        }

        public static Icon Default
        {
            get
            {
                if (null == _iconDefault)
                {
                    lock (_lock)
                    {
                        _iconDefault = IconGenerator.GetIcon(SIZE_ICON, Properties.Settings.Default.SystrayIconDefault);
                    }
                }
                return _iconDefault;
            }
        }
        public static Icon Fast
        {
            get
            {
                if (null == _iconFast)
                {
                    lock (_lock)
                    {
                        _iconFast = IconGenerator.GetIcon(SIZE_ICON, Properties.Settings.Default.SystrayIconFast);
                    }
                }
                return _iconFast;
            }
        }
        public static Icon Good
        {
            get
            {
                if (null == _iconGood)
                {
                    lock (_lock)
                    {
                        _iconGood = IconGenerator.GetIcon(SIZE_ICON, Properties.Settings.Default.SystrayIconGood);
                    }
                }
                return _iconGood;
            }
        }
        public static Icon Slow
        {
            get
            {
                if (null == _iconSlow)
                {
                    lock (_lock)
                    {
                        _iconSlow = IconGenerator.GetIcon(SIZE_ICON, Properties.Settings.Default.SystrayIconSlow);
                    }
                }
                return _iconSlow;
            }
        }
        public static Icon Bad
        {
            get
            {
                if (null == _iconBad)
                {
                    lock (_lock)
                    {
                        _iconBad = IconGenerator.GetIcon(SIZE_ICON, Properties.Settings.Default.SystrayIconBad);
                    }
                }
                return _iconBad;
            }
        }


    }
}
