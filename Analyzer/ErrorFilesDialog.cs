using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Analyzer
{
    public partial class ErrorFilesDialog : Form
    {
        private int errorfiles;

        public ErrorFilesDialog()
        {
            InitializeComponent();
            errorfiles = 0;
        }

        public void AddErrorFile(string errorfile)
        {
            listBox1.Items.Add(errorfile);
            errorfiles++;
        }

        private void ErrorFilesDialog_Load(object sender, EventArgs e)
        {
            //errorfiles = 0;
        }

        private void ErrorFilesDialog_Shown(object sender, EventArgs e)
        {
            Text = errorfiles + " invalid .xyz files";
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBox1.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                string path = (string)listBox1.Items[index];
                ProcessStartInfo psi = new ProcessStartInfo("explorer.exe", "/n /e,/select," + path);
                Process.Start(psi);
            }
        }
    }
}
