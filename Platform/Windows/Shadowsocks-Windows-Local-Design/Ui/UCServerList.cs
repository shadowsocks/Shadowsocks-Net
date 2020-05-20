/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shadowsocks_Windows_Local.Ui
{
    public partial class UCServerList : UserControl
    {
        public UCServerList()
        {
            InitializeComponent();

            toolStrip.Dock = DockStyle.None;
        }

        void RemoveServer()
        {
            if (null != listServer.SelectedItem)
            {
                listServer.Items.Remove(listServer.SelectedItem);
            }
        }


        void ShareServer()
        {
            MessageBox.Show("ShareServer(), stay tuned.");
        }

        void ToggleButton()
        {
            btnSortUp.Enabled = btnSortDown.Enabled = (null != listServer.SelectedItem);
            btnShare.Enabled = btnRemove.Enabled = (null != listServer.SelectedItem);
        }


        private void UCServerList_Layout(object sender, LayoutEventArgs e)
        {
            ToggleButton();
            FloatToolbar();
        }

        private void UCServerList_Resize(object sender, EventArgs e)
        {
            FloatToolbar();
        }

        void FloatToolbar()
        {
            listServer.Height = this.Height - toolStrip.Height;
            toolStrip.Width = this.Width;
            toolStrip.Left = 0;
            toolStrip.Top = listServer.Height + 1;
            toolStrip.BringToFront();
        }

        private void listBoxServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToggleButton();
        }

        private void listBoxServer_MouseClick(object sender, MouseEventArgs e)
        {
            ToggleButton();
            if (null != listServer.SelectedItem && e.Button == MouseButtons.Right)
            {
                contextMenuStrip.Show(MousePosition);
            }
        }




        #region Commands
        private void btnAdd_Click(object sender, EventArgs e)
        {
            listServer.Items.Add($"{this.Height},{listServer.Height},{toolStrip.Top}");
        }

        private void btnAdd_URL_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_QRCode_Click(object sender, EventArgs e)
        {

        }

        private void btnSortUp_Click(object sender, EventArgs e)
        {

        }

        private void btnSortDown_Click(object sender, EventArgs e)
        {

        }

        private void btnShareSsURIMenuItem_Click(object sender, EventArgs e)
        {
            ShareServer();
        }

        private void btnShareQrCodeMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {

        }

        private void cmRemove_Click(object sender, EventArgs e)
        {

        }

        private void cmSortUp_Click(object sender, EventArgs e)
        {

        }

        private void cmSortDown_Click(object sender, EventArgs e)
        {

        }

        private void cmShareSsURI_Click(object sender, EventArgs e)
        {
            ShareServer();
        }

        private void cmShareQrCode_Click(object sender, EventArgs e)
        {
            ShareServer();
        }
        #endregion
    }
}
