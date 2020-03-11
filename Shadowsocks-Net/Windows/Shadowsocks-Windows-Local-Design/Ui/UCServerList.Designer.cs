/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

namespace Shadowsocks_Windows_Local.Ui
{
    partial class UCServerList
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCServerList));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnAdd_URL = new System.Windows.Forms.ToolStripButton();
            this.btnAdd_QRCode = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSortUp = new System.Windows.Forms.ToolStripButton();
            this.btnSortDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnShare = new System.Windows.Forms.ToolStripSplitButton();
            this.btnShareQrCodeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnShareSsURIMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRemove = new System.Windows.Forms.ToolStripButton();
            this.listServer = new System.Windows.Forms.ListBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmSortUp = new System.Windows.Forms.ToolStripMenuItem();
            this.cmSortDown = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmShare = new System.Windows.Forms.ToolStripMenuItem();
            this.cmShareSsURI = new System.Windows.Forms.ToolStripMenuItem();
            this.cmShareQrCode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.BackColor = System.Drawing.SystemColors.MenuBar;
            this.toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAdd,
            this.btnAdd_URL,
            this.btnAdd_QRCode,
            this.toolStripSeparator1,
            this.btnSortUp,
            this.btnSortDown,
            this.toolStripSeparator2,
            this.btnShare,
            this.btnRemove});
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAdd.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_add_file_48;
            this.btnAdd.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnAdd_URL
            // 
            resources.ApplyResources(this.btnAdd_URL, "btnAdd_URL");
            this.btnAdd_URL.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAdd_URL.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_add_link_50;
            this.btnAdd_URL.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.btnAdd_URL.Name = "btnAdd_URL";
            this.btnAdd_URL.Click += new System.EventHandler(this.btnAdd_URL_Click);
            // 
            // btnAdd_QRCode
            // 
            resources.ApplyResources(this.btnAdd_QRCode, "btnAdd_QRCode");
            this.btnAdd_QRCode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAdd_QRCode.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_qr_code_50;
            this.btnAdd_QRCode.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.btnAdd_QRCode.Name = "btnAdd_QRCode";
            this.btnAdd_QRCode.Click += new System.EventHandler(this.btnAdd_QRCode_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // btnSortUp
            // 
            resources.ApplyResources(this.btnSortUp, "btnSortUp");
            this.btnSortUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSortUp.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_sort_up_50;
            this.btnSortUp.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.btnSortUp.Name = "btnSortUp";
            this.btnSortUp.Click += new System.EventHandler(this.btnSortUp_Click);
            // 
            // btnSortDown
            // 
            resources.ApplyResources(this.btnSortDown, "btnSortDown");
            this.btnSortDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSortDown.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_sort_down_50;
            this.btnSortDown.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.btnSortDown.Name = "btnSortDown";
            this.btnSortDown.Click += new System.EventHandler(this.btnSortDown_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // btnShare
            // 
            resources.ApplyResources(this.btnShare, "btnShare");
            this.btnShare.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnShare.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnShareQrCodeMenuItem,
            this.btnShareSsURIMenuItem});
            this.btnShare.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_share_50;
            this.btnShare.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.btnShare.Name = "btnShare";
            // 
            // btnShareQrCodeMenuItem
            // 
            resources.ApplyResources(this.btnShareQrCodeMenuItem, "btnShareQrCodeMenuItem");
            this.btnShareQrCodeMenuItem.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_qr_code_48;
            this.btnShareQrCodeMenuItem.Name = "btnShareQrCodeMenuItem";
            this.btnShareQrCodeMenuItem.Click += new System.EventHandler(this.btnShareQrCodeMenuItem_Click);
            // 
            // btnShareSsURIMenuItem
            // 
            resources.ApplyResources(this.btnShareSsURIMenuItem, "btnShareSsURIMenuItem");
            this.btnShareSsURIMenuItem.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_link_50;
            this.btnShareSsURIMenuItem.Name = "btnShareSsURIMenuItem";
            this.btnShareSsURIMenuItem.Click += new System.EventHandler(this.btnShareSsURIMenuItem_Click);
            // 
            // btnRemove
            // 
            resources.ApplyResources(this.btnRemove, "btnRemove");
            this.btnRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRemove.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_delete_file_50;
            this.btnRemove.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // listServer
            // 
            resources.ApplyResources(this.listServer, "listServer");
            this.listServer.FormattingEnabled = true;
            this.listServer.Name = "listServer";
            this.listServer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listBoxServer_MouseClick);
            this.listServer.SelectedIndexChanged += new System.EventHandler(this.listBoxServer_SelectedIndexChanged);
            // 
            // contextMenuStrip
            // 
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmRemove,
            this.toolStripMenuItem2,
            this.cmSortUp,
            this.cmSortDown,
            this.toolStripMenuItem1,
            this.cmShare});
            this.contextMenuStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.contextMenuStrip.Name = "contextMenuStrip1";
            // 
            // cmRemove
            // 
            resources.ApplyResources(this.cmRemove, "cmRemove");
            this.cmRemove.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_delete_file_50;
            this.cmRemove.Name = "cmRemove";
            this.cmRemove.Click += new System.EventHandler(this.cmRemove_Click);
            // 
            // toolStripMenuItem2
            // 
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            // 
            // cmSortUp
            // 
            resources.ApplyResources(this.cmSortUp, "cmSortUp");
            this.cmSortUp.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_sort_up_50;
            this.cmSortUp.Name = "cmSortUp";
            this.cmSortUp.Click += new System.EventHandler(this.cmSortUp_Click);
            // 
            // cmSortDown
            // 
            resources.ApplyResources(this.cmSortDown, "cmSortDown");
            this.cmSortDown.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_sort_down_50;
            this.cmSortDown.Name = "cmSortDown";
            this.cmSortDown.Click += new System.EventHandler(this.cmSortDown_Click);
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // cmShare
            // 
            resources.ApplyResources(this.cmShare, "cmShare");
            this.cmShare.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmShareSsURI,
            this.cmShareQrCode});
            this.cmShare.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_share_50;
            this.cmShare.Name = "cmShare";
            // 
            // cmShareSsURI
            // 
            resources.ApplyResources(this.cmShareSsURI, "cmShareSsURI");
            this.cmShareSsURI.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_link_50;
            this.cmShareSsURI.Name = "cmShareSsURI";
            this.cmShareSsURI.Click += new System.EventHandler(this.cmShareSsURI_Click);
            // 
            // cmShareQrCode
            // 
            resources.ApplyResources(this.cmShareQrCode, "cmShareQrCode");
            this.cmShareQrCode.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_qr_code_48;
            this.cmShareQrCode.Name = "cmShareQrCode";
            this.cmShareQrCode.Click += new System.EventHandler(this.cmShareQrCode_Click);
            // 
            // UCServerList
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.listServer);
            this.Name = "UCServerList";
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.UCServerList_Layout);
            this.Resize += new System.EventHandler(this.UCServerList_Resize);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnAdd_URL;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnAdd_QRCode;
        private System.Windows.Forms.ToolStripButton btnSortUp;
        private System.Windows.Forms.ToolStripButton btnSortDown;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnRemove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSplitButton btnShare;
        private System.Windows.Forms.ToolStripMenuItem btnShareQrCodeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnShareSsURIMenuItem;
        private System.Windows.Forms.ListBox listServer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem cmRemove;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem cmSortUp;
        private System.Windows.Forms.ToolStripMenuItem cmSortDown;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cmShare;
        private System.Windows.Forms.ToolStripMenuItem cmShareSsURI;
        private System.Windows.Forms.ToolStripMenuItem cmShareQrCode;
    }
}
