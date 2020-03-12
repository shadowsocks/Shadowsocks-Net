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
    using Properties;
    public partial class UCSystrayIconEditor : UserControl
    {
        const int SIZE_ICON = 50;
        public UCSystrayIconEditor()
        {
            InitializeComponent();
            //_iconSize = btnSystrayIconDefault.Width;
        }

        private void UCSystrayIconEditor_Load(object sender, EventArgs e)
        {

            btnSystrayIconFast.Image = IconGenerator.GetImage(SIZE_ICON, Settings.Default.SystrayIconFast);
            btnSystrayIconGood.Image = IconGenerator.GetImage(SIZE_ICON, Settings.Default.SystrayIconGood);
            btnSystrayIconSlow.Image = IconGenerator.GetImage(SIZE_ICON, Settings.Default.SystrayIconSlow);
            btnSystrayIconBad.Image = IconGenerator.GetImage(SIZE_ICON, Settings.Default.SystrayIconBad);
            btnSystrayIconDefault.Image = IconGenerator.GetImage(SIZE_ICON, Settings.Default.SystrayIconDefault);
        }

        private void btnSystrayIcon_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            var name = btn.Tag.ToString();
            colorDialog1.Color = (Color)Settings.Default.PropertyValues[name].PropertyValue;
            if (DialogResult.OK == colorDialog1.ShowDialog())
            {
                Color newColor = colorDialog1.Color;
               
                btn.Image = IconGenerator.GetImage(SIZE_ICON, newColor);

                Settings.Default.PropertyValues[name].PropertyValue = newColor;
                //Settings.Default.Save();
                IconCache.Reload();
            }
        }
    }
}
