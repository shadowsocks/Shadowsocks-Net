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
            this.propertyGridServer = new System.Windows.Forms.PropertyGrid();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabGlobal = new System.Windows.Forms.TabPage();
            this.groupBoxHTTP = new System.Windows.Forms.GroupBox();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.groupBoxSOCKS5 = new System.Windows.Forms.GroupBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.tabServers = new System.Windows.Forms.TabPage();
            this.tabAppearance = new System.Windows.Forms.TabPage();
            this.tableAppearance = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxSystrayIcon = new System.Windows.Forms.GroupBox();
            this.flowLayoutSystrayIcon = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSystrayIconGood = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.tableAbout = new System.Windows.Forms.TableLayoutPanel();
            this.lblAbout = new System.Windows.Forms.Label();
            this.linkAbout = new System.Windows.Forms.LinkLabel();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableMain = new System.Windows.Forms.TableLayoutPanel();
            this.panelBottomBar = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.userControl12 = new Shadowsocks_Windows_Local.Ui.UCServerList();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerServerTab)).BeginInit();
            this.splitContainerServerTab.Panel1.SuspendLayout();
            this.splitContainerServerTab.Panel2.SuspendLayout();
            this.splitContainerServerTab.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabGlobal.SuspendLayout();
            this.groupBoxHTTP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.groupBoxSOCKS5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.tabServers.SuspendLayout();
            this.tabAppearance.SuspendLayout();
            this.tableAppearance.SuspendLayout();
            this.groupBoxSystrayIcon.SuspendLayout();
            this.flowLayoutSystrayIcon.SuspendLayout();
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
            this.splitContainerServerTab.Panel1.Controls.Add(this.userControl12);
            // 
            // splitContainerServerTab.Panel2
            // 
            this.splitContainerServerTab.Panel2.Controls.Add(this.propertyGridServer);
            // 
            // propertyGridServer
            // 
            resources.ApplyResources(this.propertyGridServer, "propertyGridServer");
            this.propertyGridServer.Name = "propertyGridServer";
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabGlobal);
            this.tabControlMain.Controls.Add(this.tabServers);
            this.tabControlMain.Controls.Add(this.tabAppearance);
            this.tabControlMain.Controls.Add(this.tabAbout);
            resources.ApplyResources(this.tabControlMain, "tabControlMain");
            this.tabControlMain.Multiline = true;
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Tag = "Global";
            // 
            // tabGlobal
            // 
            this.tabGlobal.Controls.Add(this.groupBoxHTTP);
            this.tabGlobal.Controls.Add(this.groupBoxSOCKS5);
            resources.ApplyResources(this.tabGlobal, "tabGlobal");
            this.tabGlobal.Name = "tabGlobal";
            this.tabGlobal.UseVisualStyleBackColor = true;
            // 
            // groupBoxHTTP
            // 
            this.groupBoxHTTP.Controls.Add(this.numericUpDown2);
            resources.ApplyResources(this.groupBoxHTTP, "groupBoxHTTP");
            this.groupBoxHTTP.Name = "groupBoxHTTP";
            this.groupBoxHTTP.TabStop = false;
            // 
            // numericUpDown2
            // 
            resources.ApplyResources(this.numericUpDown2, "numericUpDown2");
            this.numericUpDown2.Name = "numericUpDown2";
            // 
            // groupBoxSOCKS5
            // 
            this.groupBoxSOCKS5.Controls.Add(this.numericUpDown1);
            resources.ApplyResources(this.groupBoxSOCKS5, "groupBoxSOCKS5");
            this.groupBoxSOCKS5.Name = "groupBoxSOCKS5";
            this.groupBoxSOCKS5.TabStop = false;
            // 
            // numericUpDown1
            // 
            resources.ApplyResources(this.numericUpDown1, "numericUpDown1");
            this.numericUpDown1.Name = "numericUpDown1";
            // 
            // tabServers
            // 
            this.tabServers.Controls.Add(this.splitContainerServerTab);
            resources.ApplyResources(this.tabServers, "tabServers");
            this.tabServers.Name = "tabServers";
            this.tabServers.UseVisualStyleBackColor = true;
            // 
            // tabAppearance
            // 
            this.tabAppearance.Controls.Add(this.tableAppearance);
            resources.ApplyResources(this.tabAppearance, "tabAppearance");
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
            this.groupBoxSystrayIcon.Controls.Add(this.flowLayoutSystrayIcon);
            resources.ApplyResources(this.groupBoxSystrayIcon, "groupBoxSystrayIcon");
            this.groupBoxSystrayIcon.Name = "groupBoxSystrayIcon";
            this.groupBoxSystrayIcon.TabStop = false;
            // 
            // flowLayoutSystrayIcon
            // 
            this.flowLayoutSystrayIcon.Controls.Add(this.btnSystrayIconGood);
            this.flowLayoutSystrayIcon.Controls.Add(this.button1);
            this.flowLayoutSystrayIcon.Controls.Add(this.button2);
            resources.ApplyResources(this.flowLayoutSystrayIcon, "flowLayoutSystrayIcon");
            this.flowLayoutSystrayIcon.Name = "flowLayoutSystrayIcon";
            // 
            // btnSystrayIconGood
            // 
            this.btnSystrayIconGood.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSystrayIconGood.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnSystrayIconGood, "btnSystrayIconGood");
            this.btnSystrayIconGood.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_speed_50;
            this.btnSystrayIconGood.Name = "btnSystrayIconGood";
            this.btnSystrayIconGood.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.button1, "button1");
            this.button1.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_speed_50;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.button2, "button2");
            this.button2.Image = global::Shadowsocks_Windows_Local.Properties.Resources.icon_speed_50;
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.tableAbout);
            resources.ApplyResources(this.tabAbout, "tabAbout");
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
            this.panelBottomBar.Controls.Add(this.btnApply);
            this.panelBottomBar.Controls.Add(this.btnCancel);
            this.panelBottomBar.Controls.Add(this.btnOK);
            resources.ApplyResources(this.panelBottomBar, "panelBottomBar");
            this.panelBottomBar.Name = "panelBottomBar";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // userControl12
            // 
            resources.ApplyResources(this.userControl12, "userControl12");
            this.userControl12.Name = "userControl12";
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
            this.groupBoxHTTP.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.groupBoxSOCKS5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.tabServers.ResumeLayout(false);
            this.tabAppearance.ResumeLayout(false);
            this.tableAppearance.ResumeLayout(false);
            this.groupBoxSystrayIcon.ResumeLayout(false);
            this.flowLayoutSystrayIcon.ResumeLayout(false);
            this.tabAbout.ResumeLayout(false);
            this.tableAbout.ResumeLayout(false);
            this.tableMain.ResumeLayout(false);
            this.panelBottomBar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabGlobal;
        private System.Windows.Forms.TabPage tabServers;
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
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button btnSystrayIconGood;
        private System.Windows.Forms.TableLayoutPanel tableAppearance;
        private System.Windows.Forms.GroupBox groupBoxSystrayIcon;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutSystrayIcon;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private UCServerList userControl12;
        private System.Windows.Forms.Label label1;
    }
}