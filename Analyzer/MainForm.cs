using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Analyzer
{
    public partial class MainForm : Form
    {
        //overlay that shows options to...
        private OverlayPanel _viewpanel; //change the hue of the blockset and more view related options
        private OverlayPanel _loadingpanel; //load .xyz files from a folder
        private OverlayPanel _datepanel; //change the viewed date

        private OverlayPanel _overpanel; //currently active OverlayPanel (only one can be active at a time)
        private Font _boldFont; //font for writing the date in _datepanel
        public OverlayPanel OverPanel
        {
            get { return _overpanel; }
            set
            {
                if (OverPanel != null)
                    OverPanel.Hide();
                this._overpanel = value;
                OverPanel.Show();
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private CheckBox cbxenableavg;

        private MyTrackBar timechanger; //trackbar that changes the viewed date
        private Label lbldate; //label that shows the selected date
        private Label lblpath; //label that shows the selected path to open from
        private CheckBox cbxenable; //checkbox that enables change rate viewing
        private TextBox txbmin; //textbox that sets the white value
        private TextBox txbmax; //textbox that sets the red value
        private void MainForm_Load(object sender, EventArgs e)
        {
            settingsPanel.BackColor = Color.FromArgb(66, 66, 66);
            _boldFont = new Font("Microsoft Sans Serif", 9, FontStyle.Bold);

            #region init_viewpanel
            _viewpanel = new OverlayPanel();
            _viewpanel.Hide();
            _viewpanel.SuspendLayout();
            inversionControl.Controls.Add(_viewpanel);

            MyTrackBar bar = new MyTrackBar();
            bar.Minimum = 0;
            bar.Maximum = 60;
            bar.Size = new Size(300, 40);
            bar.BackColor = Color.FromArgb(66, 66, 66);
            bar.Scroll += huebar_ValueChanged;

            cbxenable = new CheckBox();
            cbxenable.BackColor = Color.FromArgb(66, 66, 66);
            cbxenable.FlatStyle = FlatStyle.Flat;
            cbxenable.ForeColor = SystemColors.ControlDark;
            cbxenable.Location = new Point(10, bar.Location.Y + bar.Height + 20);
            cbxenable.Size = new Size(100, 29);
            cbxenable.TabIndex = 2;
            cbxenable.Text = "View Change Rate";
            cbxenable.CheckedChanged += cbxenable_CheckedChanged;

            txbmin = new TextBox();
            txbmin.Enabled = false;
            txbmin.BackColor = Color.FromArgb(66, 66, 66);
            txbmin.ForeColor = SystemColors.ControlDark;
            txbmin.Location = new Point(cbxenable.Location.X + cbxenable.Width + 10, cbxenable.Location.Y);
            txbmin.Size = new Size(150, 30);
            txbmin.Text = "-1";
            txbmin.TextChanged += txbmin_TextChanged;

            txbmax = new TextBox();
            txbmax.Enabled = false;
            txbmax.BackColor = Color.FromArgb(66, 66, 66);
            txbmax.ForeColor = SystemColors.ControlDark;
            txbmax.Location = new Point(txbmin.Location.X, txbmin.Location.Y + txbmin.Height + 10);
            txbmax.Size = new Size(150, 30);
            txbmax.Text = "-1";
            txbmax.TextChanged += txbmax_TextChanged;

            cbxenableavg = new CheckBox();
            cbxenableavg.BackColor = Color.FromArgb(66, 66, 66);
            cbxenableavg.FlatStyle = FlatStyle.Flat;
            cbxenableavg.ForeColor = SystemColors.ControlDark;
            cbxenableavg.Location = new Point(10, txbmax.Location.Y + txbmax.Height + 20);
            cbxenableavg.Size = new Size(100, 29);
            cbxenableavg.TabIndex = 2;
            cbxenableavg.Text = "View Average Resistivity";
            cbxenableavg.CheckedChanged += cbxenableavg_CheckedChanged;

            _viewpanel.Controls.Add(bar);
            _viewpanel.Controls.Add(cbxenable);
            _viewpanel.Controls.Add(txbmin);
            _viewpanel.Controls.Add(txbmax);
            _viewpanel.Controls.Add(cbxenableavg);
            _viewpanel.ResumeLayout();
            #endregion

            #region init_loadingpanel
            _loadingpanel = new OverlayPanel();
            _loadingpanel.Hide();
            _loadingpanel.SuspendLayout();
            inversionControl.Controls.Add(_loadingpanel);

            Button loadFolderBtn = new Button();
            loadFolderBtn.BackColor = System.Drawing.SystemColors.ControlDark;
            loadFolderBtn.BackgroundImage = Properties.Resources.folderopen2;
            loadFolderBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            loadFolderBtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            loadFolderBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            loadFolderBtn.Location = new System.Drawing.Point(0, 0);
            loadFolderBtn.Name = "loadBtn";
            loadFolderBtn.Size = new System.Drawing.Size(24, 24);
            loadFolderBtn.TabIndex = 0;
            loadFolderBtn.UseVisualStyleBackColor = false;
            loadFolderBtn.Click += new System.EventHandler(this.loadFolderBtn_Click);

            lblpath = new Label();
            lblpath.AutoSize = true;
            lblpath.Text = "Loading Path...";
            lblpath.ForeColor = SystemColors.ControlDark;
            lblpath.Location = new Point(loadFolderBtn.Location.X + loadBtn.Width - 10, 15);

            _loadingpanel.Controls.Add(loadFolderBtn);
            _loadingpanel.Controls.Add(lblpath);
            _loadingpanel.ResumeLayout();
            #endregion

            #region init_datepanel
            _datepanel = new OverlayPanel();
            _datepanel.Hide();
            _datepanel.SuspendLayout();
            inversionControl.Controls.Add(_datepanel);

            lbldate = new Label();
            lbldate.AutoSize = true;
            lbldate.Text = "Date: ...";
            lbldate.Font = _boldFont;
            lbldate.Location = new Point(0, 0);
            lbldate.ForeColor = SystemColors.ControlDark;

            timechanger = new MyTrackBar();
            timechanger.Scroll += timechanger_ValueChanged;
            timechanger.Size = new Size(600, 40);
            timechanger.BackColor = Color.FromArgb(66, 66, 66);
            timechanger.Location = new Point(0, lbldate.Location.Y + lbldate.Height + 20);

            _datepanel.Controls.Add(lbldate);
            _datepanel.Controls.Add(timechanger);
            _datepanel.ResumeLayout();
            #endregion
        }

        private void cbxenableavg_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxenableavg.Checked)
            {
                inversionControl.MinDifference = -1;
                inversionControl.MaxDifference = -1;

                MinMaxResistivity mmr = Helper.GetAvgMinMaxResistivity(ModelBlocks);
                inversionControl.MinAvgRes = mmr.Minimum;
                inversionControl.MaxAvgRes = mmr.Maximum;

                cbxenable.Checked = false;
                cbxenable.Enabled = false;

                inversionControl.DrawAvgPalette();
                exportbtn.Show();
            } else
            {
                cbxenable.Enabled = true;
                inversionControl.MinAvgRes = -1;
                inversionControl.MaxAvgRes = -1;

                inversionControl.FreePalette();
                exportbtn.Hide();
            }
        }

        private void cbxenable_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxenable.Checked)
            {
                inversionControl.MinAvgRes = -1;
                inversionControl.MaxAvgRes = -1;

                exportbtn.Show();
                txbmin.Enabled = true;
                txbmax.Enabled = true;
                txbmin.Text = ModelBlocks.Min(x => x.Difference).ToString();
                txbmax.Text = ModelBlocks.Max(x => x.Difference).ToString();

                cbxenableavg.Checked = false;
                cbxenableavg.Enabled = false;
            }
            else
            {
                exportbtn.Hide();
                txbmin.Enabled = false;
                txbmax.Enabled = false;
                txbmin.Text = "-1";
                txbmax.Text = "-1";

                cbxenableavg.Enabled = true;
            }

        }

        private void txbmin_TextChanged(object sender, EventArgs e)
        {
            try
            {
                inversionControl.MinDifference = float.Parse(txbmin.Text);
                inversionControl.DrawPalette();
            }
            catch (Exception exc)
            {
            }
        }

        private void txbmax_TextChanged(object sender, EventArgs e)
        {
            try
            {
                inversionControl.MaxDifference = float.Parse(txbmax.Text);
                inversionControl.DrawPalette();
            }
            catch (Exception exc)
            {
            }
        }

        private void timechanger_ValueChanged(object sender, EventArgs e)
        {
            inversionControl.ActiveBlockSet = timechanger.Value;
            lbldate.Text = _datetimes[timechanger.Value].ToString("dd.MM.yyyy HH:mm:ss");
            lbldate.Location = new Point(timechanger.Width / 2 - lbldate.Width / 2, lbldate.Location.Y);
            inversionControl.Invalidate();
        }

        public ModelBlock[] ModelBlocks { get; set; } //all model blocks
        private List<DateTime> _datetimes; //all datetimes
        private int _start; //starting index in array _allxyz[]
        private int _end; //ending index ...
        private void loadIntervalBtn_Click(object sender, EventArgs e)
        {
            OverPanel.Hide();

            _allxyz = Directory.GetFiles(openxyzfolderDialog.SelectedPath, "*.xyz").ToArray();

            string startdate;
            _start = -1;
            for (int i = 0; _start == -1; i++)
            {
                startdate = fromdtp.Value.AddDays(i).ToString("yyyy-MM-dd");
                _start = Array.FindIndex(_allxyz, x => x.Contains(startdate));
            }

            string enddate;
            _end = -1;
            for (int i = 0; _end == -1; i++)
            {
                enddate = todtp.Value.AddDays(-i).ToString("yyyy-MM-dd");
                _end = Array.FindLastIndex(_allxyz, x => x.Contains(enddate));
            }

            if (_end < _start)
                MessageBox.Show("End date can't be smaller than start date");

            _allxyz = _allxyz.SubArray(_start, _end - _start);
            string[] _dates = _allxyz.Select(Path.GetFileNameWithoutExtension).ToArray();
            _datetimes = new List<DateTime>();

            for (int i = 0; i < _dates.Length; i++)
            {
                _datetimes.Add(DateTime.ParseExact(_dates[i], "yyyy-MM-dd HH-mm-ss", null));
            }

            ModelBlocks = Helper.GetModelBlocksFromFile(_allxyz[0]);
            int errorfiles = Helper.GetResistivitiesFromFile(ModelBlocks, _allxyz, _datetimes);
            timechanger.Minimum = 0;
            timechanger.Maximum = _end - _start - 1 - errorfiles;
            inversionControl.Dates = _datetimes.ToArray();
            inversionControl.ModelBlocks = ModelBlocks;
            inversionControl.RMSError = Helper.GetRMSError(_allxyz);
            settingsBtn.Show();
            huepicker.Show();
            showchart.Show();
            cbxenable.Checked = false;
        }

        private string[] _allxyz; //all files
        private DateTimePicker fromdtp; //datetimepicker "from"
        private DateTimePicker todtp; //datetimepicker "to"
        private void loadFolderBtn_Click(object sender, EventArgs e)
        {
            if (openxyzfolderDialog.ShowDialog() == DialogResult.OK)
            {
                _allxyz = Directory.GetFiles(openxyzfolderDialog.SelectedPath, "*.xyz").ToArray();
                DateTime start = DateTime.ParseExact(Path.GetFileNameWithoutExtension(_allxyz[0]), "yyyy-MM-dd HH-mm-ss", null);
                DateTime end = DateTime.ParseExact(Path.GetFileNameWithoutExtension(_allxyz[_allxyz.Length - 1]), "yyyy-MM-dd HH-mm-ss", null);

                #region init_panel_stuff
                OverPanel.Controls.Remove(lblpath);
                lblpath.Text = openxyzfolderDialog.SelectedPath;
                lblpath.Location = new Point(lblpath.Location.X - 10, 15);
                OverPanel.SuspendLayout();
                Button loadIntervalBtn = new Button();
                loadIntervalBtn.BackColor = SystemColors.ControlDark;
                loadIntervalBtn.BackgroundImage = Properties.Resources.checkbox2;
                loadIntervalBtn.BackgroundImageLayout = ImageLayout.Stretch;
                loadIntervalBtn.FlatAppearance.BorderColor = SystemColors.ControlDarkDark;
                loadIntervalBtn.FlatStyle = FlatStyle.Flat;
                loadIntervalBtn.Location = new Point(0, 44);
                loadIntervalBtn.Name = "loadIntervalBtn";
                loadIntervalBtn.Size = new Size(24, 24);
                loadIntervalBtn.TabIndex = 0;
                loadIntervalBtn.UseVisualStyleBackColor = false;
                loadIntervalBtn.Click += new EventHandler(this.loadIntervalBtn_Click);

                Label fromlbl = new Label();
                fromlbl.Width = 27;
                fromlbl.Text = "from";
                fromlbl.Location = new Point(lblpath.Location.X, 50);
                fromlbl.ForeColor = SystemColors.ControlDark;

                fromdtp = new DateTimePicker();
                fromdtp.Format = DateTimePickerFormat.Custom;
                fromdtp.CustomFormat = "yyyy-MM-dd";
                fromdtp.Value = start;
                fromdtp.MinDate = start;
                fromdtp.MaxDate = end;
                fromdtp.Size = new Size(75, 30);
                fromdtp.Location = new Point(fromlbl.Location.X + fromlbl.Width, 47);

                Label tolbl = new Label();
                tolbl.Width = 15;
                tolbl.AutoSize = true;
                tolbl.Text = "to";
                tolbl.Location = new Point(fromdtp.Location.X + fromdtp.Width, 50);
                tolbl.ForeColor = SystemColors.ControlDark;

                todtp = new DateTimePicker();
                todtp.Format = DateTimePickerFormat.Custom;
                todtp.CustomFormat = "yyyy-MM-dd";
                todtp.Value = end;
                todtp.MinDate = start;
                todtp.MaxDate = end;
                todtp.Size = new Size(75, 30);
                todtp.Location = new Point(tolbl.Location.X + tolbl.Width, 47);

                OverPanel.Controls.Add(loadIntervalBtn);
                OverPanel.Controls.Add(fromlbl);
                OverPanel.Controls.Add(fromdtp);
                OverPanel.Controls.Add(tolbl);
                OverPanel.Controls.Add(todtp);
                OverPanel.ResumeLayout();
                OverPanel.Controls.Add(lblpath);
                #endregion
            }
        }

        private void huebar_ValueChanged(object sender, EventArgs e)
        {
            inversionControl.Hue = ((TrackBar)sender).Value * 5;
            inversionControl.Invalidate();
        }

        private void huepicker_Click(object sender, EventArgs e)
        {
            if (_viewpanel.Visible)
            {
                OverPanel.Hide();
            }
            else
            {
                _viewpanel.ArrowLocation = new Point(((Button)sender).Location.X + ((Button)sender).Width / 2, settingsPanel.Height);
                OverPanel = _viewpanel;
            }
        }

        private void inversionControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (OverPanel == null)
                return;
            OverPanel.Hide();
        }

        private void settingsPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (OverPanel == null)
                return;
            OverPanel.Hide();
        }

        private void loadBtn_Click(object sender, EventArgs e)
        {
            if (_loadingpanel.Visible)
            {
                OverPanel.Hide();
            }
            else
            {
                _loadingpanel.ArrowLocation = new Point(((Button)sender).Location.X + ((Button)sender).Width / 2, settingsPanel.Height);
                OverPanel = _loadingpanel;
            }
        }

        private void settingsBtn_Click(object sender, EventArgs e)
        {
            if (_datepanel.Visible)
            {
                OverPanel.Hide();
            }
            else
            {
                lbldate.Text = _datetimes[timechanger.Value].ToString("dd.MM.yyyy HH:mm:ss");
                lbldate.Location = new Point(timechanger.Width / 2 - lbldate.Width / 2, lbldate.Location.Y);
                _datepanel.ArrowLocation = new Point(((Button)sender).Location.X + ((Button)sender).Width / 2, settingsPanel.Height);
                OverPanel = _datepanel;
            }
        }

        private void showchart_Click(object sender, EventArgs e)
        {
            if(OverPanel != null)
            {
                OverPanel.Hide();
            }
            InversionsPerDay ipd = new InversionsPerDay();
            ipd.Dates = _datetimes;
            ipd.ShowDialog();
        }

        private void exportbtn_Click(object sender, EventArgs e)
        {
            if(OverPanel != null)
            {
                OverPanel.Hide();
            }
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                inversionControl.SaveBitmap(saveFileDialog1.FileName);
            }
        }
    }
}
