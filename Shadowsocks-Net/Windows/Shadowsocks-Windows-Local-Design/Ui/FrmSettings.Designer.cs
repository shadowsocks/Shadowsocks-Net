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
            this.userControl12 = new Shadowsocks_Windows_Local.Ui.UCServerList();
            this.propertyGridServer = new System.Windows.Forms.PropertyGrid();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabGlobal = new System.Windows.Forms.TabPage();
            this.tableGlobal = new System.Windows.Forms.TableLayoutPanel();
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
            this.tabServer = new System.Windows.Forms.TabPage();
            this.tabAppearance = new System.Windows.Forms.TabPage();
            this.tableAppearance = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxSystrayIcon = new System.Windows.Forms.GroupBox();
            this.ucSystrayIconEditor1 = new Shadowsocks_Windows_Local.Ui.UCSystrayIconEditor();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.tableAbout = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAbout = new System.Windows.Forms.Label();
            this.linkAbout = new System.Windows.Forms.LinkLabel();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableMain = new System.Windows.Forms.TableLayoutPanel();
            this.panelBottomBar = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerServerTab)).BeginInit();
            this.splitContainerServerTab.Panel1.SuspendLayout();
            this.splitContainerServerTab.Panel2.SuspendLayout();
            this.splitContainerServerTab.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabGlobal.SuspendLayout();
            this.tableGlobal.SuspendLayout();
            this.groupBoxSOCKS5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_SOCKS5_Port)).BeginInit();
            this.groupBoxHTTP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_HTTP_Port)).BeginInit();
            this.tabServer.SuspendLayout();
            this.tabAppearance.SuspendLayout();
            this.tableAppearance.SuspendLayout();
            this.groupBoxSystrayIcon.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.tableAbout.SuspendLayout();
            this.tableMain.SuspendLayout();
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
            resources.ApplyResources(this.splitContainerServerTab.Panel1, "splitContainerServerTab.Panel1");
            this.splitContainerServerTab.Panel1.Controls.Add(this.userControl12);
            // 
            // splitContainerServerTab.Panel2
            // 
            resources.ApplyResources(this.splitContainerServerTab.Panel2, "splitContainerServerTab.Panel2");
            this.splitContainerServerTab.Panel2.Controls.Add(this.propertyGridServer);
            // 
            // userControl12
            // 
            resources.ApplyResources(this.userControl12, "userControl12");
            this.userControl12.Name = "userControl12";
            // 
            // propertyGridServer
            // 
            resources.ApplyResources(this.propertyGridServer, "propertyGridServer");
            this.propertyGridServer.Name = "propertyGridServer";
            // 
            // tabControlMain
            // 
            resources.ApplyResources(this.tabControlMain, "tabControlMain");
            this.tabControlMain.Controls.Add(this.tabGlobal);
            this.tabControlMain.Controls.Add(this.tabServer);
            this.tabControlMain.Controls.Add(this.tabAppearance);
            this.tabControlMain.Controls.Add(this.tabAbout);
            this.tabControlMain.Multiline = true;
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Tag = "";
            // 
            // tabGlobal
            // 
            resources.ApplyResources(this.tabGlobal, "tabGlobal");
            this.tabGlobal.Controls.Add(this.tableGlobal);
            this.tabGlobal.Name = "tabGlobal";
            this.tabGlobal.Tag = "Global";
            this.tabGlobal.UseVisualStyleBackColor = true;
            // 
            // tableGlobal
            // 
            resources.ApplyResources(this.tableGlobal, "tableGlobal");
            this.tableGlobal.Controls.Add(this.groupBoxSOCKS5, 0, 0);
            this.tableGlobal.Controls.Add(this.groupBoxHTTP, 0, 1);
            this.tableGlobal.Name = "tableGlobal";
            // 
            // groupBoxSOCKS5
            // 
            resources.ApplyResources(this.groupBoxSOCKS5, "groupBoxSOCKS5");
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
            resources.ApplyResources(this.txt_SOCKS5_Port, "txt_SOCKS5_Port");
            this.txt_SOCKS5_Port.Name = "txt_SOCKS5_Port";
            // 
            // groupBoxHTTP
            // 
            resources.ApplyResources(this.groupBoxHTTP, "groupBoxHTTP");
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
            resources.ApplyResources(this.txt_HTTP_Port, "txt_HTTP_Port");
            this.txt_HTTP_Port.Name = "txt_HTTP_Port";
            // 
            // tabServer
            // 
            resources.ApplyResources(this.tabServer, "tabServer");
            this.tabServer.Controls.Add(this.splitContainerServerTab);
            this.tabServer.Name = "tabServer";
            this.tabServer.UseVisualStyleBackColor = true;
            // 
            // tabAppearance
            // 
            resources.ApplyResources(this.tabAppearance, "tabAppearance");
            this.tabAppearance.Controls.Add(this.tableAppearance);
            this.tabAppearance.Name = "tabAppearance";
            this.tabAppearance.UseVisualStyleBackColor = true;
            // 
            // tableAppearance
            // 
            resources.ApplyResources(this.tableAppearance, "tableAppearance");
            this.tableAppearance.Controls.Add(this.groupBoxSystrayIcon, 0, 0);
            this.tableAppearance.Name = "tableAppearance";
            // 
            // groupBoxSystrayIcon
            // 
            resources.ApplyResources(this.groupBoxSystrayIcon, "groupBoxSystrayIcon");
            this.groupBoxSystrayIcon.Controls.Add(this.ucSystrayIconEditor1);
            this.groupBoxSystrayIcon.Name = "groupBoxSystrayIcon";
            this.groupBoxSystrayIcon.TabStop = false;
            // 
            // ucSystrayIconEditor1
            // 
            resources.ApplyResources(this.ucSystrayIconEditor1, "ucSystrayIconEditor1");
            this.ucSystrayIconEditor1.BackColor = System.Drawing.Color.White;
            this.ucSystrayIconEditor1.Name = "ucSystrayIconEditor1";
            // 
            // tabAbout
            // 
            resources.ApplyResources(this.tabAbout, "tabAbout");
            this.tabAbout.Controls.Add(this.tableAbout);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // tableAbout
            // 
            resources.ApplyResources(this.tableAbout, "tableAbout");
            this.tableAbout.Controls.Add(this.label1, 0, 2);
            this.tableAbout.Controls.Add(this.lblAbout, 0, 0);
            this.tableAbout.Controls.Add(this.linkAbout, 0, 1);
            this.tableAbout.Name = "tableAbout";
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
            // btnApply
            // 
            resources.ApplyResources(this.btnApply, "btnApply");
            this.btnApply.Name = "btnApply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tableMain
            // 
            resources.ApplyResources(this.tableMain, "tableMain");
            this.tableMain.Controls.Add(this.panelBottomBar, 0, 1);
            this.tableMain.Controls.Add(this.tabControlMain, 0, 0);
            this.tableMain.Name = "tableMain";
            // 
            // panelBottomBar
            // 
            resources.ApplyResources(this.panelBottomBar, "panelBottomBar");
            this.panelBottomBar.Controls.Add(this.btnApply);
            this.panelBottomBar.Controls.Add(this.btnCancel);
            this.panelBottomBar.Controls.Add(this.btnOK);
            this.panelBottomBar.Name = "panelBottomBar";
            // 
            // FrmSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableMain);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSettings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSettings_FormClosing);
            this.splitContainerServerTab.Panel1.ResumeLayout(false);
            this.splitContainerServerTab.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerServerTab)).EndInit();
            this.splitContainerServerTab.ResumeLayout(false);
            this.tabControlMain.ResumeLayout(false);
            this.tabGlobal.ResumeLayout(false);
            this.tableGlobal.ResumeLayout(false);
            this.groupBoxSOCKS5.ResumeLayout(false);
            this.groupBoxSOCKS5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_SOCKS5_Port)).EndInit();
            this.groupBoxHTTP.ResumeLayout(false);
            this.groupBoxHTTP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_HTTP_Port)).EndInit();
            this.tabServer.ResumeLayout(false);
            this.tabAppearance.ResumeLayout(false);
            this.tableAppearance.ResumeLayout(false);
            this.groupBoxSystrayIcon.ResumeLayout(false);
            this.tabAbout.ResumeLayout(false);
            this.tableAbout.ResumeLayout(false);
            this.tableMain.ResumeLayout(false);
            this.panelBottomBar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabGlobal;
        private System.Windows.Forms.TabPage tabServer;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PropertyGrid propertyGridServer;
        private System.Windows.Forms.Label lblAbout;
        private System.Windows.Forms.LinkLabel linkAbout;
        private System.Windows.Forms.GroupBox groupBoxHTTP;
        private System.Windows.Forms.GroupBox groupBoxSOCKS5;
        private System.Windows.Forms.TableLayoutPanel tableAbout;
        private System.Windows.Forms.SplitContainer splitContainerServerTab;
        private System.Windows.Forms.TableLayoutPanel tableMain;
        private System.Windows.Forms.Panel panelBottomBar;
        private System.Windows.Forms.TabPage tabAppearance;
        private System.Windows.Forms.NumericUpDown txt_HTTP_Port;
        private System.Windows.Forms.NumericUpDown txt_SOCKS5_Port;
        private System.Windows.Forms.TableLayoutPanel tableAppearance;
        private System.Windows.Forms.GroupBox groupBoxSystrayIcon;
        private UCServerList userControl12;
        private System.Windows.Forms.Label label1;
        private UCSystrayIconEditor ucSystrayIconEditor1;
        private System.Windows.Forms.TableLayoutPanel tableGlobal;
        private System.Windows.Forms.CheckBox chk_SOCKS5_Enable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chk_HTTP_Enable;
        private System.Windows.Forms.CheckBox chk_SOCKS5_Share;
        private System.Windows.Forms.CheckBox chk_HTTP_Share;
    }
}