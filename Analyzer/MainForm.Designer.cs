namespace Analyzer
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.settingsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.loadBtn = new System.Windows.Forms.Button();
            this.settingsBtn = new System.Windows.Forms.Button();
            this.huepicker = new System.Windows.Forms.Button();
            this.showchart = new System.Windows.Forms.Button();
            this.exportbtn = new System.Windows.Forms.Button();
            this.openxyzfolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.inversionControl = new Analyzer.InversionControl();
            this.settingsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsPanel
            // 
            this.settingsPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.settingsPanel.Controls.Add(this.loadBtn);
            this.settingsPanel.Controls.Add(this.settingsBtn);
            this.settingsPanel.Controls.Add(this.huepicker);
            this.settingsPanel.Controls.Add(this.showchart);
            this.settingsPanel.Controls.Add(this.exportbtn);
            this.settingsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.settingsPanel.Location = new System.Drawing.Point(0, 0);
            this.settingsPanel.Name = "settingsPanel";
            this.settingsPanel.Size = new System.Drawing.Size(2014, 95);
            this.settingsPanel.TabIndex = 1;
            this.settingsPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.settingsPanel_MouseDown);
            // 
            // loadBtn
            // 
            this.loadBtn.BackColor = System.Drawing.SystemColors.ControlDark;
            this.loadBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("loadBtn.BackgroundImage")));
            this.loadBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.loadBtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.loadBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadBtn.Location = new System.Drawing.Point(3, 3);
            this.loadBtn.Name = "loadBtn";
            this.loadBtn.Size = new System.Drawing.Size(86, 86);
            this.loadBtn.TabIndex = 1;
            this.loadBtn.UseVisualStyleBackColor = false;
            this.loadBtn.Click += new System.EventHandler(this.loadBtn_Click);
            // 
            // settingsBtn
            // 
            this.settingsBtn.BackColor = System.Drawing.SystemColors.ControlDark;
            this.settingsBtn.BackgroundImage = global::Analyzer.Properties.Resources.datepicker2;
            this.settingsBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.settingsBtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.settingsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsBtn.Location = new System.Drawing.Point(95, 3);
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(86, 86);
            this.settingsBtn.TabIndex = 2;
            this.settingsBtn.UseVisualStyleBackColor = false;
            this.settingsBtn.Visible = false;
            this.settingsBtn.Click += new System.EventHandler(this.settingsBtn_Click);
            // 
            // huepicker
            // 
            this.huepicker.BackColor = System.Drawing.SystemColors.ControlDark;
            this.huepicker.BackgroundImage = global::Analyzer.Properties.Resources.view2;
            this.huepicker.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.huepicker.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.huepicker.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.huepicker.Location = new System.Drawing.Point(187, 3);
            this.huepicker.Name = "huepicker";
            this.huepicker.Size = new System.Drawing.Size(86, 86);
            this.huepicker.TabIndex = 0;
            this.huepicker.UseVisualStyleBackColor = false;
            this.huepicker.Visible = false;
            this.huepicker.Click += new System.EventHandler(this.huepicker_Click);
            // 
            // showchart
            // 
            this.showchart.BackColor = System.Drawing.SystemColors.ControlDark;
            this.showchart.BackgroundImage = global::Analyzer.Properties.Resources.barchart2;
            this.showchart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.showchart.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.showchart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showchart.Location = new System.Drawing.Point(279, 3);
            this.showchart.Name = "showchart";
            this.showchart.Size = new System.Drawing.Size(86, 86);
            this.showchart.TabIndex = 3;
            this.showchart.UseVisualStyleBackColor = false;
            this.showchart.Visible = false;
            this.showchart.Click += new System.EventHandler(this.showchart_Click);
            // 
            // exportbtn
            // 
            this.exportbtn.BackColor = System.Drawing.SystemColors.ControlDark;
            this.exportbtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("exportbtn.BackgroundImage")));
            this.exportbtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.exportbtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.exportbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exportbtn.Location = new System.Drawing.Point(371, 3);
            this.exportbtn.Name = "exportbtn";
            this.exportbtn.Size = new System.Drawing.Size(86, 86);
            this.exportbtn.TabIndex = 4;
            this.exportbtn.UseVisualStyleBackColor = false;
            this.exportbtn.Visible = false;
            this.exportbtn.Click += new System.EventHandler(this.exportbtn_Click);
            // 
            // openxyzfolderDialog
            // 
            this.openxyzfolderDialog.SelectedPath = "C:\\Diplomarbeit_Geoelektrik\\daten_xyz";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Image File (*.png)|*.png";
            // 
            // inversionControl
            // 
            this.inversionControl.ActiveBlockSet = 0;
            this.inversionControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.inversionControl.Dates = null;
            this.inversionControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inversionControl.Hue = 0;
            this.inversionControl.Location = new System.Drawing.Point(0, 0);
            this.inversionControl.Margin = new System.Windows.Forms.Padding(12);
            this.inversionControl.MaxDifference = -1F;
            this.inversionControl.MinDifference = 0F;
            this.inversionControl.ModelBlocks = null;
            this.inversionControl.Name = "inversionControl";
            this.inversionControl.NormalizationVector = ((System.Drawing.PointF)(resources.GetObject("inversionControl.NormalizationVector")));
            this.inversionControl.Size = new System.Drawing.Size(2014, 1037);
            this.inversionControl.TabIndex = 0;
            this.inversionControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.inversionControl_MouseDown);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2014, 1037);
            this.Controls.Add(this.settingsPanel);
            this.Controls.Add(this.inversionControl);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "MainForm";
            this.Text = "Alalyzer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.settingsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private InversionControl inversionControl;
        private System.Windows.Forms.FlowLayoutPanel settingsPanel;
        private System.Windows.Forms.Button huepicker;
        private System.Windows.Forms.Button loadBtn;
        private System.Windows.Forms.FolderBrowserDialog openxyzfolderDialog;
        private System.Windows.Forms.Button settingsBtn;
        private System.Windows.Forms.Button showchart;
        private System.Windows.Forms.Button exportbtn;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

