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
            this.tabControlSettings = new System.Windows.Forms.TabControl();
            this.tabGlobal = new System.Windows.Forms.TabPage();
            this.tabServers = new System.Windows.Forms.TabPage();
            this.listBoxServers = new System.Windows.Forms.ListBox();
            this.propertyGridServer = new System.Windows.Forms.PropertyGrid();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.linkAbout = new System.Windows.Forms.LinkLabel();
            this.lblAbout = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBoxSOCKS5 = new System.Windows.Forms.GroupBox();
            this.groupBoxHTTP = new System.Windows.Forms.GroupBox();
            this.tabControlSettings.SuspendLayout();
            this.tabGlobal.SuspendLayout();
            this.tabServers.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlSettings
            // 
            this.tabControlSettings.Controls.Add(this.tabGlobal);
            this.tabControlSettings.Controls.Add(this.tabServers);
            this.tabControlSettings.Controls.Add(this.tabAbout);
            resources.ApplyResources(this.tabControlSettings, "tabControlSettings");
            this.tabControlSettings.Multiline = true;
            this.tabControlSettings.Name = "tabControlSettings";
            this.tabControlSettings.SelectedIndex = 0;
            this.tabControlSettings.Tag = "Global";
            // 
            // tabGlobal
            // 
            this.tabGlobal.Controls.Add(this.groupBoxHTTP);
            this.tabGlobal.Controls.Add(this.groupBoxSOCKS5);
            this.tabGlobal.Controls.Add(this.comboBox1);
            this.tabGlobal.Controls.Add(this.checkBox1);
            resources.ApplyResources(this.tabGlobal, "tabGlobal");
            this.tabGlobal.Name = "tabGlobal";
            this.tabGlobal.UseVisualStyleBackColor = true;
            // 
            // tabServers
            // 
            this.tabServers.Controls.Add(this.listBoxServers);
            this.tabServers.Controls.Add(this.propertyGridServer);
            resources.ApplyResources(this.tabServers, "tabServers");
            this.tabServers.Name = "tabServers";
            this.tabServers.UseVisualStyleBackColor = true;
            // 
            // listBoxServers
            // 
            resources.ApplyResources(this.listBoxServers, "listBoxServers");
            this.listBoxServers.FormattingEnabled = true;
            this.listBoxServers.Name = "listBoxServers";
            // 
            // propertyGridServer
            // 
            resources.ApplyResources(this.propertyGridServer, "propertyGridServer");
            this.propertyGridServer.Name = "propertyGridServer";
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.linkAbout);
            this.tabAbout.Controls.Add(this.lblAbout);
            resources.ApplyResources(this.tabAbout, "tabAbout");
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // linkAbout
            // 
            resources.ApplyResources(this.linkAbout, "linkAbout");
            this.linkAbout.Name = "linkAbout";
            this.linkAbout.TabStop = true;
            this.linkAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAbout_LinkClicked);
            // 
            // lblAbout
            // 
            resources.ApplyResources(this.lblAbout, "lblAbout");
            this.lblAbout.Name = "lblAbout";
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
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.Name = "comboBox1";
            // 
            // groupBoxSOCKS5
            // 
            resources.ApplyResources(this.groupBoxSOCKS5, "groupBoxSOCKS5");
            this.groupBoxSOCKS5.Name = "groupBoxSOCKS5";
            this.groupBoxSOCKS5.TabStop = false;
            // 
            // groupBoxHTTP
            // 
            resources.ApplyResources(this.groupBoxHTTP, "groupBoxHTTP");
            this.groupBoxHTTP.Name = "groupBoxHTTP";
            this.groupBoxHTTP.TabStop = false;
            // 
            // FrmSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.tabControlSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSettings";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.tabControlSettings.ResumeLayout(false);
            this.tabGlobal.ResumeLayout(false);
            this.tabGlobal.PerformLayout();
            this.tabServers.ResumeLayout(false);
            this.tabAbout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlSettings;
        private System.Windows.Forms.TabPage tabGlobal;
        private System.Windows.Forms.TabPage tabServers;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PropertyGrid propertyGridServer;
        private System.Windows.Forms.ListBox listBoxServers;
        private System.Windows.Forms.Label lblAbout;
        private System.Windows.Forms.LinkLabel linkAbout;
        private System.Windows.Forms.GroupBox groupBoxHTTP;
        private System.Windows.Forms.GroupBox groupBoxSOCKS5;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}