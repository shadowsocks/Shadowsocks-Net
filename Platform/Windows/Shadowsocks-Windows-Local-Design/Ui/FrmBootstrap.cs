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
    using Properties;
    public partial class FrmBootstrap : Form
    {
        public FrmBootstrap()
        {
            this.BackColor = Color.LightGray;
            this.TransparencyKey = Color.LightGray;

            InitializeComponent();

            this.BackgroundImageLayout = ImageLayout.Zoom;
            this.BackgroundImage = IconGenerator.GetImage(Settings.Default.SystrayIconDefault);

            this.notifyIcon.Icon = IconCache.Default;

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
            //TODO switch server
        }


        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            base.OnPaintBackground(e);
        }

        
        private void timer1_Tick(object sender, EventArgs e)
        {
            notifyIcon.Text = DateTime.Now.ToLongTimeString();

            int sec = DateTime.Now.Second;
            if (0 == sec % 2)
            {
                notifyIcon.Icon = IconCache.Fast;
            }
            else if (0 == sec % 3)
            {
                notifyIcon.Icon = IconCache.Good;
            }
            else if (0 == sec % 5)
            {
                notifyIcon.Icon = IconCache.Slow;
            }
            else if (0 == sec % 7)
            {
                notifyIcon.Icon = IconCache.Bad;
            }
            else
            {
                notifyIcon.Icon = IconCache.Default;
            }
        }
    }
}
