/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

namespace Shadowsocks_Windows_Local.Ui
{
    partial class FrmSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSettings));
            this.splitContainerServerTab = new System.Windows.Forms.SplitContainer();
            this.ucServerList = new Shadowsocks_Windows_Local.Ui.UCServerList();
            this.propertyGridServer = new System.Windows.Forms.PropertyGrid();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabGlobal = new System.Windows.Forms.TabPage();
            this.layoutGlobal = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxSOCKS5 = new System.Windows.Forms.GroupBox();
            this.chk_SOCKS5_Share = new System.Windows.Forms.CheckBox();
            this.chk_SOCKS5_Enable = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_SOCKS5_Port = new System.Windows.Forms.NumericUpDown();
            this.groupBoxHTTP = new System.Windows.Forms.GroupBox();
            this.chk_HTTP_Share = new System.Windows.Forms.CheckBox();
            this.chk_HTTP_Enable = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_HTTP_Port = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.tabAppearance = new System.Windows.Forms.TabPage();
            this.layoutAppearance = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxSystrayIcon = new System.Windows.Forms.GroupBox();
            this.ucSystrayIconEditor1 = new Shadowsocks_Windows_Local.Ui.UCSystrayIconEditor();
            this.chkEnableDynamicSystrayIcon = new System.Windows.Forms.CheckBox();
            this.tabServer = new System.Windows.Forms.TabPage();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.layoutAbout = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAbout = new System.Windows.Forms.Label();
            this.linkAbout = new System.Windows.Forms.LinkLabel();
            this.btnOK = new System.Windows.Forms.Button();
            this.layoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.panelBottomBar = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerServerTab)).BeginInit();
            this.splitContainerServerTab.Panel1.SuspendLayout();
            this.splitContainerServerTab.Panel2.SuspendLayout();
            this.splitContainerServerTab.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabGlobal.SuspendLayout();
            this.layoutGlobal.SuspendLayout();
            this.groupBoxSOCKS5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_SOCKS5_Port)).BeginInit();
            this.groupBoxHTTP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_HTTP_Port)).BeginInit();
            this.tabAppearance.SuspendLayout();
            this.layoutAppearance.SuspendLayout();
            this.groupBoxSystrayIcon.SuspendLayout();
            this.tabServer.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.layoutAbout.SuspendLayout();
            this.layoutMain.SuspendLayout();
            this.panelBottomBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerServerTab
            // 
            resources.ApplyResources(this.splitContainerServerTab, "splitContainerServerTab");
            this.splitContainerServerTab.Name = "splitContainerServerTab";
            // 
            // splitContainerServerTab.Panel1
            // 
            this.splitContainerServerTab.Panel1.Controls.Add(this.ucServerList);
            // 
            // splitContainerServerTab.Panel2
            // 
            this.splitContainerServerTab.Panel2.Controls.Add(this.propertyGridServer);
            // 
            // ucServerList
            // 
            resources.ApplyResources(this.ucServerList, "ucServerList");
            this.ucServerList.Name = "ucServerList";
            // 
            // propertyGridServer
            // 
            resources.ApplyResources(this.propertyGridServer, "propertyGridServer");
            this.propertyGridServer.Name = "propertyGridServer";
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabGlobal);
            this.tabControlMain.Controls.Add(this.tabAppearance);
            this.tabControlMain.Controls.Add(this.tabServer);
            this.tabControlMain.Controls.Add(this.tabAbout);
            resources.ApplyResources(this.tabControlMain, "tabControlMain");
            this.tabControlMain.Multiline = true;
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Tag = "";
            // 
            // tabGlobal
            // 
            this.tabGlobal.Controls.Add(this.layoutGlobal);
            resources.ApplyResources(this.tabGlobal, "tabGlobal");
            this.tabGlobal.Name = "tabGlobal";
            this.tabGlobal.Tag = "Global";
            this.tabGlobal.UseVisualStyleBackColor = true;
            // 
            // layoutGlobal
            // 
            resources.ApplyResources(this.layoutGlobal, "layoutGlobal");
            this.layoutGlobal.Controls.Add(this.groupBoxSOCKS5, 0, 0);
            this.layoutGlobal.Controls.Add(this.groupBoxHTTP, 0, 1);
            this.layoutGlobal.Controls.Add(this.label4, 0, 2);
            this.layoutGlobal.Name = "layoutGlobal";
            // 
            // groupBoxSOCKS5
            // 
            resources.ApplyResources(this.groupBoxSOCKS5, "groupBoxSOCKS5");
            this.groupBoxSOCKS5.Controls.Add(this.label6);
            this.groupBoxSOCKS5.Controls.Add(this.chk_SOCKS5_Share);
            this.groupBoxSOCKS5.Controls.Add(this.chk_SOCKS5_Enable);
            this.groupBoxSOCKS5.Controls.Add(this.label2);
            this.groupBoxSOCKS5.Controls.Add(this.txt_SOCKS5_Port);
            this.groupBoxSOCKS5.Name = "groupBoxSOCKS5";
            this.groupBoxSOCKS5.TabStop = false;
            // 
            // chk_SOCKS5_Share
            // 
            resources.ApplyResources(this.chk_SOCKS5_Share, "chk_SOCKS5_Share");
            this.chk_SOCKS5_Share.Name = "chk_SOCKS5_Share";
            this.chk_SOCKS5_Share.UseVisualStyleBackColor = true;
            // 
            // chk_SOCKS5_Enable
            // 
            resources.ApplyResources(this.chk_SOCKS5_Enable, "chk_SOCKS5_Enable");
            this.chk_SOCKS5_Enable.Name = "chk_SOCKS5_Enable";
            this.chk_SOCKS5_Enable.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txt_SOCKS5_Port
            // 
            this.txt_SOCKS5_Port.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_SOCKS5_Port, "txt_SOCKS5_Port");
            this.txt_SOCKS5_Port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.txt_SOCKS5_Port.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.txt_SOCKS5_Port.Name = "txt_SOCKS5_Port";
            this.txt_SOCKS5_Port.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            // 
            // groupBoxHTTP
            // 
            resources.ApplyResources(this.groupBoxHTTP, "groupBoxHTTP");
            this.groupBoxHTTP.Controls.Add(this.label5);
            this.groupBoxHTTP.Controls.Add(this.chk_HTTP_Share);
            this.groupBoxHTTP.Controls.Add(this.chk_HTTP_Enable);
            this.groupBoxHTTP.Controls.Add(this.label3);
            this.groupBoxHTTP.Controls.Add(this.txt_HTTP_Port);
            this.groupBoxHTTP.Name = "groupBoxHTTP";
            this.groupBoxHTTP.TabStop = false;
            // 
            // chk_HTTP_Share
            // 
            resources.ApplyResources(this.chk_HTTP_Share, "chk_HTTP_Share");
            this.chk_HTTP_Share.Name = "chk_HTTP_Share";
            this.chk_HTTP_Share.UseVisualStyleBackColor = true;
            // 
            // chk_HTTP_Enable
            // 
            resources.ApplyResources(this.chk_HTTP_Enable, "chk_HTTP_Enable");
            this.chk_HTTP_Enable.Name = "chk_HTTP_Enable";
            this.chk_HTTP_Enable.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txt_HTTP_Port
            // 
            this.txt_HTTP_Port.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_HTTP_Port, "txt_HTTP_Port");
            this.txt_HTTP_Port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.txt_HTTP_Port.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.txt_HTTP_Port.Name = "txt_HTTP_Port";
            this.txt_HTTP_Port.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.Gray;
            this.label4.Name = "label4";
            // 
            // tabAppearance
            // 
            this.tabAppearance.Controls.Add(this.layoutAppearance);
            resources.ApplyResources(this.tabAppearance, "tabAppearance");
            this.tabAppearance.Name = "tabAppearance";
            this.tabAppearance.UseVisualStyleBackColor = true;
            // 
            // layoutAppearance
            // 
            resources.ApplyResources(this.layoutAppearance, "layoutAppearance");
            this.layoutAppearance.Controls.Add(this.groupBoxSystrayIcon, 0, 0);
            this.layoutAppearance.Name = "layoutAppearance";
            // 
            // groupBoxSystrayIcon
            // 
            resources.ApplyResources(this.groupBoxSystrayIcon, "groupBoxSystrayIcon");
            this.groupBoxSystrayIcon.Controls.Add(this.ucSystrayIconEditor1);
            this.groupBoxSystrayIcon.Controls.Add(this.chkEnableDynamicSystrayIcon);
            this.groupBoxSystrayIcon.Name = "groupBoxSystrayIcon";
            this.groupBoxSystrayIcon.TabStop = false;
            // 
            // ucSystrayIconEditor1
            // 
            resources.ApplyResources(this.ucSystrayIconEditor1, "ucSystrayIconEditor1");
            this.ucSystrayIconEditor1.BackColor = System.Drawing.Color.White;
            this.ucSystrayIconEditor1.Name = "ucSystrayIconEditor1";
            // 
            // chkEnableDynamicSystrayIcon
            // 
            resources.ApplyResources(this.chkEnableDynamicSystrayIcon, "chkEnableDynamicSystrayIcon");
            this.chkEnableDynamicSystrayIcon.Name = "chkEnableDynamicSystrayIcon";
            this.chkEnableDynamicSystrayIcon.UseVisualStyleBackColor = true;
            // 
            // tabServer
            // 
            this.tabServer.Controls.Add(this.splitContainerServerTab);
            resources.ApplyResources(this.tabServer, "tabServer");
            this.tabServer.Name = "tabServer";
            this.tabServer.UseVisualStyleBackColor = true;
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.layoutAbout);
            resources.ApplyResources(this.tabAbout, "tabAbout");
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // layoutAbout
            // 
            resources.ApplyResources(this.layoutAbout, "layoutAbout");
            this.layoutAbout.Controls.Add(this.label1, 0, 2);
            this.layoutAbout.Controls.Add(this.lblAbout, 0, 0);
            this.layoutAbout.Controls.Add(this.linkAbout, 0, 1);
            this.layoutAbout.Name = "layoutAbout";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lblAbout
            // 
            resources.ApplyResources(this.lblAbout, "lblAbout");
            this.lblAbout.Name = "lblAbout";
            // 
            // linkAbout
            // 
            resources.ApplyResources(this.linkAbout, "linkAbout");
            this.linkAbout.Name = "linkAbout";
            this.linkAbout.TabStop = true;
            this.linkAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAbout_LinkClicked);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // layoutMain
            // 
            resources.ApplyResources(this.layoutMain, "layoutMain");
            this.layoutMain.Controls.Add(this.panelBottomBar, 0, 1);
            this.layoutMain.Controls.Add(this.tabControlMain, 0, 0);
            this.layoutMain.Name = "layoutMain";
            // 
            // panelBottomBar
            // 
            this.panelBottomBar.Controls.Add(this.btnOK);
            resources.ApplyResources(this.panelBottomBar, "panelBottomBar");
            this.panelBottomBar.Name = "panelBottomBar";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.Color.Gray;
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.ForeColor = System.Drawing.Color.Gray;
            this.label6.Name = "label6";
            // 
            // FrmSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.layoutMain);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSettings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSettings_FormClosing);
            this.Load += new System.EventHandler(this.FrmSettings_Load);
            this.splitContainerServerTab.Panel1.ResumeLayout(false);
            this.splitContainerServerTab.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerServerTab)).EndInit();
            this.splitContainerServerTab.ResumeLayout(false);
            this.tabControlMain.ResumeLayout(false);
            this.tabGlobal.ResumeLayout(false);
            this.layoutGlobal.ResumeLayout(false);
            this.layoutGlobal.PerformLayout();
            this.groupBoxSOCKS5.ResumeLayout(false);
            this.groupBoxSOCKS5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_SOCKS5_Port)).EndInit();
            this.groupBoxHTTP.ResumeLayout(false);
            this.groupBoxHTTP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_HTTP_Port)).EndInit();
            this.tabAppearance.ResumeLayout(false);
            this.layoutAppearance.ResumeLayout(false);
            this.layoutAppearance.PerformLayout();
            this.groupBoxSystrayIcon.ResumeLayout(false);
            this.groupBoxSystrayIcon.PerformLayout();
            this.tabServer.ResumeLayout(false);
            this.tabAbout.ResumeLayout(false);
            this.layoutAbout.ResumeLayout(false);
            this.layoutMain.ResumeLayout(false);
            this.panelBottomBar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabGlobal;
        private System.Windows.Forms.TabPage tabServer;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.PropertyGrid propertyGridServer;
        private System.Windows.Forms.Label lblAbout;
        private System.Windows.Forms.LinkLabel linkAbout;
        private System.Windows.Forms.GroupBox groupBoxHTTP;
        private System.Windows.Forms.GroupBox groupBoxSOCKS5;
        private System.Windows.Forms.TableLayoutPanel layoutAbout;
        private System.Windows.Forms.SplitContainer splitContainerServerTab;
        private System.Windows.Forms.TableLayoutPanel layoutMain;
        private System.Windows.Forms.Panel panelBottomBar;
        private System.Windows.Forms.TabPage tabAppearance;
        private System.Windows.Forms.NumericUpDown txt_HTTP_Port;
        private System.Windows.Forms.NumericUpDown txt_SOCKS5_Port;
        private System.Windows.Forms.TableLayoutPanel layoutAppearance;
        private System.Windows.Forms.GroupBox groupBoxSystrayIcon;
        private UCServerList ucServerList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel layoutGlobal;
        private System.Windows.Forms.CheckBox chk_SOCKS5_Enable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chk_HTTP_Enable;
        private System.Windows.Forms.CheckBox chk_SOCKS5_Share;
        private System.Windows.Forms.CheckBox chk_HTTP_Share;
        private System.Windows.Forms.CheckBox chkEnableDynamicSystrayIcon;
        private UCSystrayIconEditor ucSystrayIconEditor1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
    }
}