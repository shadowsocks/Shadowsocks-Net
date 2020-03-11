using System;
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
        public UCSystrayIconEditor()
        {
            InitializeComponent();
        }

        private void UCSystrayIconEditor_Load(object sender, EventArgs e)
        {
            IconGenerator.GetBitmap(out Bitmap fast, Properties.Settings.Default.SystrayIconFast);
            btnSystrayIconFast.BackgroundImage = fast;

            IconGenerator.GetBitmap(out Bitmap good, Properties.Settings.Default.SystrayIconGood);
            btnSystrayIconGood.BackgroundImage = good;

            IconGenerator.GetBitmap(out Bitmap slow, Properties.Settings.Default.SystrayIconSlow);
            btnSystrayIconSlow.BackgroundImage = slow;

            IconGenerator.GetBitmap(out Bitmap bad, Properties.Settings.Default.SystrayIconBad);
            btnSystrayIconBad.BackgroundImage = bad;

        }

        private void btnSystrayIcon_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            var name = btn.Tag.ToString();

            if (DialogResult.OK == colorDialog1.ShowDialog())
            {
                Color c = colorDialog1.Color;

                IconGenerator.GetBitmap(out Bitmap icon, c);
                btn.BackgroundImage = icon;

                Properties.Settings.Default.PropertyValues[name].PropertyValue = c;
                Properties.Settings.Default.Save();
            }
        }
    }
}
