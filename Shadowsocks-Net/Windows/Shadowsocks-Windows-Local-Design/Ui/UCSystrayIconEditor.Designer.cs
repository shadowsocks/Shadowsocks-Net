namespace Shadowsocks_Windows_Local.Ui
{
    partial class UCSystrayIconEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCSystrayIconEditor));
            this.flowLayoutSystrayIcon = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSystrayIconFast = new System.Windows.Forms.Button();
            this.btnSystrayIconGood = new System.Windows.Forms.Button();
            this.btnSystrayIconSlow = new System.Windows.Forms.Button();
            this.btnSystrayIconBad = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.flowLayoutSystrayIcon.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutSystrayIcon
            // 
            resources.ApplyResources(this.flowLayoutSystrayIcon, "flowLayoutSystrayIcon");
            this.flowLayoutSystrayIcon.Controls.Add(this.btnSystrayIconFast);
            this.flowLayoutSystrayIcon.Controls.Add(this.btnSystrayIconGood);
            this.flowLayoutSystrayIcon.Controls.Add(this.btnSystrayIconSlow);
            this.flowLayoutSystrayIcon.Controls.Add(this.btnSystrayIconBad);
            this.flowLayoutSystrayIcon.Name = "flowLayoutSystrayIcon";
            // 
            // btnSystrayIconFast
            // 
            resources.ApplyResources(this.btnSystrayIconFast, "btnSystrayIconFast");
            this.btnSystrayIconFast.BackColor = System.Drawing.Color.Transparent;
            this.btnSystrayIconFast.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSystrayIconFast.Name = "btnSystrayIconFast";
            this.btnSystrayIconFast.Tag = "SystrayIconFast";
            this.btnSystrayIconFast.UseVisualStyleBackColor = false;
            this.btnSystrayIconFast.Click += new System.EventHandler(this.btnSystrayIcon_Click);
            // 
            // btnSystrayIconGood
            // 
            resources.ApplyResources(this.btnSystrayIconGood, "btnSystrayIconGood");
            this.btnSystrayIconGood.BackColor = System.Drawing.Color.Transparent;
            this.btnSystrayIconGood.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSystrayIconGood.Name = "btnSystrayIconGood";
            this.btnSystrayIconGood.Tag = "SystrayIconGood";
            this.btnSystrayIconGood.UseVisualStyleBackColor = false;
            this.btnSystrayIconGood.Click += new System.EventHandler(this.btnSystrayIcon_Click);
            // 
            // btnSystrayIconSlow
            // 
            resources.ApplyResources(this.btnSystrayIconSlow, "btnSystrayIconSlow");
            this.btnSystrayIconSlow.BackColor = System.Drawing.Color.Transparent;
            this.btnSystrayIconSlow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSystrayIconSlow.Name = "btnSystrayIconSlow";
            this.btnSystrayIconSlow.Tag = "SystrayIconSlow";
            this.btnSystrayIconSlow.UseVisualStyleBackColor = false;
            this.btnSystrayIconSlow.Click += new System.EventHandler(this.btnSystrayIcon_Click);
            // 
            // btnSystrayIconBad
            // 
            resources.ApplyResources(this.btnSystrayIconBad, "btnSystrayIconBad");
            this.btnSystrayIconBad.BackColor = System.Drawing.Color.Transparent;
            this.btnSystrayIconBad.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSystrayIconBad.Name = "btnSystrayIconBad";
            this.btnSystrayIconBad.Tag = "SystrayIconBad";
            this.btnSystrayIconBad.UseVisualStyleBackColor = false;
            this.btnSystrayIconBad.Click += new System.EventHandler(this.btnSystrayIcon_Click);
            // 
            // colorDialog1
            // 
            this.colorDialog1.AnyColor = true;
            this.colorDialog1.Color = System.Drawing.Color.Green;
            this.colorDialog1.FullOpen = true;
            // 
            // UCSystrayIconEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.flowLayoutSystrayIcon);
            this.Name = "UCSystrayIconEditor";
            this.Load += new System.EventHandler(this.UCSystrayIconEditor_Load);
            this.flowLayoutSystrayIcon.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutSystrayIcon;
        private System.Windows.Forms.Button btnSystrayIconFast;
        private System.Windows.Forms.Button btnSystrayIconGood;
        private System.Windows.Forms.Button btnSystrayIconSlow;
        private System.Windows.Forms.Button btnSystrayIconBad;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}
