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

namespace Shadowsocks_Windows_Local.Ui
{
    public partial class FrmBootstrap : Form
    {
        public FrmBootstrap()
        {
            this.BackColor = Color.LightGray;
            this.TransparencyKey = Color.LightGray;
          
            InitializeComponent();

            
            IconGenerator.GetBitmap(out Bitmap logo, Properties.Settings.Default.SystrayIconDefault);

            this.BackgroundImageLayout = ImageLayout.Zoom;
            this.BackgroundImage = logo;
          
        }
        private void FrmBootstrap_Load(object sender, EventArgs e)
        {


            Task.Factory.StartNew(() =>
            {
                Task.Delay(1000).Wait();
                this.UiThread(() =>
                {
                    //this.WindowState = FormWindowState.Minimized;
                    this.Hide();
                });
            });
        }





        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSettings f = new FrmSettings();
            f.Show(this);
        }



        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            //FrmSettings f = new FrmSettings();
            //f.ShowDialog(this);
            // contextMenuStrip.Show(MousePosition);
        }


        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            base.OnPaintBackground(e);

        }
    }
}
