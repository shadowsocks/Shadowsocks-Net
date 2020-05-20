/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Shadowsocks_Windows_Local.Ui
{
    using Properties;

    public partial class FrmSettings : Form
    {
        public FrmSettings()
        {
            

            InitializeComponent();

            LoadSettings();


            this.chk_SOCKS5_Share.CheckedChanged += new System.EventHandler(this.chk_SOCKS5_Share_CheckedChanged);
            this.chk_SOCKS5_Enable.CheckedChanged += new System.EventHandler(this.chk_SOCKS5_Enable_CheckedChanged);
            this.txt_SOCKS5_Port.ValueChanged += new System.EventHandler(this.txt_SOCKS5_Port_ValueChanged);
            this.chk_HTTP_Share.CheckedChanged += new System.EventHandler(this.chk_HTTP_Share_CheckedChanged);
            this.chk_HTTP_Enable.CheckedChanged += new System.EventHandler(this.chk_HTTP_Enable_CheckedChanged);
            this.txt_HTTP_Port.ValueChanged += new System.EventHandler(this.txt_HTTP_Port_ValueChanged);
            this.chkEnableDynamicSystrayIcon.CheckedChanged += new System.EventHandler(this.chkEnableDynamicSystrayIcon_CheckedChanged);

            this.Size = Settings.Default.SettingWindowSize;
            this.Icon = IconCache.Default;
        }
        void SaveSettings()
        {
            Settings.Default.Save();
        }


        void LoadSettings()
        {
            //global
            chk_HTTP_Enable.Checked = Settings.Default.HTTP_Enabled;
            chk_HTTP_Share.Checked = Settings.Default.HTTP_Share;
            txt_HTTP_Port.Value = Settings.Default.HTTP_Port;

            chk_SOCKS5_Enable.Checked = Settings.Default.SOCKS5_Enabled;
            chk_SOCKS5_Share.Checked = Settings.Default.SOCKS5_Share;
            txt_SOCKS5_Port.Value = Settings.Default.SOCKS5_Port;

            //appearance
            chkEnableDynamicSystrayIcon.Checked = Settings.Default.EnableDynamicSystrayIcon;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //SaveSettings();
            this.Close();
        }



        private void chkEnableDynamicSystrayIcon_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.EnableDynamicSystrayIcon = (sender as CheckBox).Checked;
        }


        private void FrmSettings_Load(object sender, EventArgs e)
        {
            ////////////////LoadSettings();
        }
        private void FrmSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.SettingWindowSize = this.Size;
            SaveSettings();
        }



        #region Setting
        private void linkAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo(linkAbout.Text);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        private void chk_SOCKS5_Enable_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.SOCKS5_Enabled = (sender as CheckBox).Checked;
        }

        private void chk_SOCKS5_Share_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.SOCKS5_Share = (sender as CheckBox).Checked;
        }

        private void chk_HTTP_Enable_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.HTTP_Enabled = (sender as CheckBox).Checked;
        }

        private void chk_HTTP_Share_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.HTTP_Share = (sender as CheckBox).Checked;
        }

        private void txt_SOCKS5_Port_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown txt = sender as NumericUpDown;
            ushort p = (ushort)txt.Value;
            if (p == Settings.Default.HTTP_Port)
            {
                MessageBox.Show(Messages.invalid_value, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt.Focus();
                txt.Select(0, p.ToString().Length);
                return;
            }
            Settings.Default.SOCKS5_Port = p;
        }

        private void txt_HTTP_Port_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown txt = sender as NumericUpDown;
            ushort p = (ushort)txt.Value;
            if (p == Settings.Default.SOCKS5_Port)
            {
                MessageBox.Show(Messages.invalid_value, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt.Focus();
                txt.Select(0, p.ToString().Length);
                return;
            }
            Settings.Default.HTTP_Port = p;
        }
        #endregion
    }
}
