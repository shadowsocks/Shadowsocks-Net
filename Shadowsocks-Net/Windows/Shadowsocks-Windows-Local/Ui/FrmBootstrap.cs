using System;
/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            InitializeComponent();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSettings f = new FrmSettings();
            f.Show();
        }

        private void FrmBootstrap_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Task.Delay(2000);
                this.UiThread(() => 
                { 
                    //this.WindowState = FormWindowState.Minimized;
                    this.Hide();
                });
            });
        }
    }
}
