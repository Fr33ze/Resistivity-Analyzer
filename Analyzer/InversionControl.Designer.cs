namespace Analyzer
{
    partial class InversionControl
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

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // InversionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "InversionControl";
            this.Size = new System.Drawing.Size(640, 579);
            this.Load += new System.EventHandler(this.InversionControl_Load);
            this.SizeChanged += new System.EventHandler(this.InversionControl_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.InversionControl_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.InversionControl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.InversionControl_MouseMove);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.InversionControl_MouseWheel);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}
