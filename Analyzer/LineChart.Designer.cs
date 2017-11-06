namespace Analyzer
{
    partial class LineChart
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
            this.saveCsvDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // LineChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.DoubleBuffered = true;
            this.Name = "LineChart";
            this.Load += new System.EventHandler(this.LineChart_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.LineChart_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LineChart_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LineChart_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog saveCsvDialog;
    }
}
