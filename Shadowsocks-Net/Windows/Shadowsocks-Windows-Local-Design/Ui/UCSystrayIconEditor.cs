using System;
/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shadowsocks_Windows_Local.Ui
{
    public partial class UCSystrayIconEditor : UserControl
    {
        int _iconSize = 50;
        public UCSystrayIconEditor()
        {
            InitializeComponent();
            //_iconSize = btnSystrayIconDefault.Width;
        }

        private void UCSystrayIconEditor_Load(object sender, EventArgs e)
        {
            IconGenerator.GetBitmap(out Bitmap fast, _iconSize, Properties.Settings.Default.SystrayIconFast);
            btnSystrayIconFast.Image = fast;

            IconGenerator.GetBitmap(out Bitmap good, _iconSize, Properties.Settings.Default.SystrayIconGood);
            btnSystrayIconGood.Image = good;

            IconGenerator.GetBitmap(out Bitmap slow, _iconSize, Properties.Settings.Default.SystrayIconSlow);
            btnSystrayIconSlow.Image = slow;

            IconGenerator.GetBitmap(out Bitmap bad, _iconSize, Properties.Settings.Default.SystrayIconBad);
            btnSystrayIconBad.Image = bad;

            IconGenerator.GetBitmap(out Bitmap @default, _iconSize, Properties.Settings.Default.SystrayIconDefault);
            btnSystrayIconDefault.Image = @default;

        }

        private void btnSystrayIcon_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            var name = btn.Tag.ToString();
            colorDialog1.Color = (Color)Properties.Settings.Default.PropertyValues[name].PropertyValue;
            if (DialogResult.OK == colorDialog1.ShowDialog())
            {
                Color c = colorDialog1.Color;

                IconGenerator.GetBitmap(out Bitmap icon, _iconSize, c);
                btn.Image = icon;

                Properties.Settings.Default.PropertyValues[name].PropertyValue = c;
                Properties.Settings.Default.Save();
            }
        }
    }
}
