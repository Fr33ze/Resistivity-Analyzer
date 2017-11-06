using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;

namespace Analyzer
{
    public partial class InversionControl : UserControl
    {
        private float zoomlevel; //the zoomlevel of the inversion
        private int selectedBlock; //the last clicked block
        private Matrix trans; //transformation matrix of the inversion
        private ModelBlock[] _modelblocks; //all modelblocks
        private float _mindifference;
        private float _maxdifference;
        private RectangleF bounds; //bounds of the invasion
        private float xBefore; //for movement
        private float yBefore; //for movement
        private double a; //stretching factor for e^(x*a)
        private double b; //stretching factor for e^(x*b)
        
        public float RMSError { get; set; }
        private float _minaverageres;
        public float MinAvgRes
        {
            get
            {
                return _minaverageres;
            }
            set
            {
                _minaverageres = value;
                b = Math.Log(MaxAvgRes - MinAvgRes + 1, Math.E);
                Invalidate();
            }
        }
        private float _maxaverageres;
        public float MaxAvgRes
        {
            get
            {
                return _maxaverageres;
            }
            set
            {
                _maxaverageres = value;
                b = Math.Log(MaxAvgRes - MinAvgRes + 1, Math.E);
                Invalidate();
            }
        }

        public int ActiveBlockSet { get; set; } //selected blockset
        public float MaxDifference
        {
            get
            {
                return _maxdifference;
            }
            set
            {
                _maxdifference = value;
                a = Math.Log(MaxDifference - MinDifference + 1, Math.E);
                Invalidate();
            }
        }  //max difference of all modelblocks
        public float MinDifference
        {
            get
            {
                return _mindifference;
            }
            set
            {
                _mindifference = value;
                a = Math.Log(MaxDifference - MinDifference + 1, Math.E);
                Invalidate();
            }
        } //min difference of all modelblocks
        public DateTime[] Dates { get; set; } //all dates
        public int Hue { get; set; } //hue of the inversion
        public PointF NormalizationVector { get; set; }
        public ModelBlock[] ModelBlocks
        {
            get { return _modelblocks; }
            set
            {
                if (value == null)
                    return;

                this._modelblocks = value;
                NormalizationVector = Helper.NormalizeOrigin(ModelBlocks);
                initMatrix();
                bounds = Helper.GetBounds(ModelBlocks, false);
                Invalidate();
            }
        } //all modelblocks
        public InversionControl()
        {
            InitializeComponent();

            zoomlevel = 1f;
            selectedBlock = -1;
        }

        public void SaveBitmap(string filename)
        {
            Pen linepen = new Pen(Brushes.Black, 2f / 5f);
            Font axesfont = new Font("Microsoft Sans Serif", 2.3f);
            Font resfont = new Font("Microsoft Sans Serif", 12);
            Bitmap bmp = new Bitmap(1050, 600);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.None;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                g.ScaleTransform(5f, -5f);
                for(int i = 0; i < ModelBlocks.Length; i++)
                {
                    double interpolation = 0.0;
                    if (MaxDifference != -1)
                    {
                        float dif = ModelBlocks[i].Difference - MinDifference;
                        if (dif < 0)
                            dif = 0;
                        interpolation = Math.Log(dif + 1) / a;
                    } else if (MaxAvgRes != -1 && MinAvgRes != -1)
                    {
                        float dif = ModelBlocks[i].AvgResistivity - MinAvgRes;
                        if (dif < 0)
                            dif = 0;
                        interpolation = Math.Log(dif + 1) / b;
                    }
                    if (interpolation < 0)
                        interpolation = 0;
                    else if (interpolation > 1)
                    {
                        interpolation = 1;
                    }
                    interpolation = Math.Round(interpolation * 10) / 10;
                    Color c = Helper.HSVToRGB(new Helper.HSV(0, interpolation, 1 - (interpolation / 4)));
                    g.FillPolygon(new SolidBrush(c), ModelBlocks[i].DrawCorners.ToArray());
                }
                g.ScaleTransform(1f, -1f);
                g.DrawImage(Properties.Resources.curve, new RectangleF(bounds.X - 1, bounds.Y - 1, bounds.Width + 2, bounds.Height + 2), new Rectangle(0, 0, Properties.Resources.curve.Width, Properties.Resources.curve.Height), GraphicsUnit.Pixel);
                g.ScaleTransform(1f / 5f, 1f / 5f);
                g.DrawImage(_palette, -(_palette.Width / 2), -bounds.Top + 150);

                g.ScaleTransform(5f, -5f);
                g.DrawLine(linepen, bounds.Left + 7.2f, bounds.Bottom + 10, bounds.Right, bounds.Bottom + 10);
                g.DrawLine(linepen, bounds.Left - 2, bounds.Bottom + 10, bounds.Left - 2, bounds.Top);

                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                for(int y = (int)bounds.Bottom + 10; y > bounds.Top; y -= 5)
                {
                    g.DrawLine(linepen, bounds.Left - 3, y, bounds.Left - 1, y);
                    g.ScaleTransform(1f, -1f);
                    int num = (int)Math.Round(y + NormalizationVector.Y, 1) + 1;
                    SizeF msstr = g.MeasureString(num.ToString(), axesfont);
                    g.DrawString(num.ToString(), axesfont, Brushes.Black, bounds.Left - 10 - msstr.Width / 5, -y - 1 - msstr.Height / 5);
                    g.ScaleTransform(1f, -1f);
                }
                float x2 = 0f;
                for (float x = bounds.Left + 7.2f; x < bounds.Right; x += 14.4f, x2 += 14.4f)
                {
                    g.DrawLine(linepen, x, bounds.Bottom + 11, x, bounds.Bottom + 9);
                    g.ScaleTransform(1f, -1f);
                    SizeF msstr = g.MeasureString(x2.ToString(), axesfont);
                    g.DrawString(x2.ToString(), axesfont, Brushes.Black, x - msstr.Width / 2f, -bounds.Bottom - 12 - msstr.Height);
                    g.ScaleTransform(1f, -1f);
                }
                SizeF mselev = g.MeasureString("Elevation [m]", resfont);
                g.ResetTransform();
                g.TranslateTransform(bmp.Width / 2 - 515, bmp.Height / 2);
                g.RotateTransform(-90);
                g.DrawString("Elevation [m]", resfont, Brushes.Black, -mselev.Width / 2 + 15, 0);

                g.ResetTransform();
                SizeF mswidth = g.MeasureString("Distance [m]", resfont);
                g.DrawString("Distance [m]", resfont, Brushes.Black, bmp.Width / 2 - mswidth.Width / 2, bmp.Height / 2 - 275);

                SizeF mserror = g.MeasureString("Average RMS error: " + RMSError + "%", resfont);
                g.DrawString("Average RMS error: " + RMSError + "%", resfont, Brushes.Black, g.VisibleClipBounds.Right - mserror.Width - 5, g.VisibleClipBounds.Bottom - mserror.Height - 5);
            }
            bmp.MakeTransparent(Color.FromArgb(33, 33, 33));
            bmp.Save(filename, ImageFormat.Png);
        }

        private Bitmap _palette;
        public Bitmap DrawPalette()
        {
            if (MinDifference == -1 || MaxDifference == -1)
            {
                _palette = null;
                Invalidate();
                return _palette;
            }
            else
            {
                _palette = new Bitmap(11 * 70, 70); //11 cells from 0.0 to 1.0 with a length of 70px each
                using (Graphics g = Graphics.FromImage(_palette))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;

                    Font resfont = new Font("Microsoft Sans Serif", 12);
                    float xcurrent = 0f;
                    for(int i = 0; xcurrent < 1.1f; i++, xcurrent += 0.1f)
                    {
                        float rescurrent = (float)(Math.Pow(Math.E, a * xcurrent) - 1 + MinDifference);
                        rescurrent = rescurrent / Dates.Length;
                        rescurrent = (float)Math.Round(rescurrent, 4);
                        Color c = Helper.HSVToRGB(new Helper.HSV(0, xcurrent, 1 - (xcurrent / 4)));
                        g.FillRectangle(new SolidBrush(c), i * 70, 0, 70, 35);
                        g.DrawLine(Pens.Black, i * 70, 0, i * 70, 35);
                        SizeF mscur = g.MeasureString(rescurrent.ToString(), resfont);
                        g.DrawString(rescurrent.ToString(), resfont, Brushes.Black, i * 70 + 35 - mscur.Width / 2, 17.5f - mscur.Height / 2);
                    }
                    g.DrawLine(Pens.Black, 1, 1, 11 * 70, 1);
                    g.DrawLine(Pens.Black, 0, 35, 11 * 70, 35);
                    g.DrawLine(Pens.Black, 11 * 70, 0, 11 * 70, 35);

                    SizeF mschange = g.MeasureString("Change of resistivity per measurement [\u2126" + "m" + "]", resfont);
                    g.DrawString("Change of resistivity per measurement [\u2126" + "m" + "]", resfont, Brushes.Black, _palette.Width / 2 - mschange.Width / 2, 40);
                }
                Invalidate();
                return _palette;
            }
        }

        public Bitmap DrawAvgPalette()
        {
            if (MinAvgRes == -1 || MaxAvgRes == -1)
            {
                _palette = null;
                Invalidate();
                return _palette;
            }
            else
            {
                _palette = new Bitmap(11 * 70, 70); //11 cells from 0.0 to 1.0 with a length of 70px each
                using (Graphics g = Graphics.FromImage(_palette))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;

                    Font resfont = new Font("Microsoft Sans Serif", 12);
                    float xcurrent = 0f;
                    for (int i = 0; xcurrent < 1.1f; i++, xcurrent += 0.1f)
                    {
                        float avgres = (float)(Math.Pow(Math.E, b * xcurrent) - 1 + MinAvgRes);
                        avgres = (float)Math.Round(avgres, 4);
                        Color c = Helper.HSVToRGB(new Helper.HSV(0, xcurrent, 1 - (xcurrent / 4)));
                        g.FillRectangle(new SolidBrush(c), i * 70, 0, 70, 35);
                        g.DrawLine(Pens.Black, i * 70, 0, i * 70, 35);
                        SizeF mscur = g.MeasureString(avgres.ToString(), resfont);
                        g.DrawString(avgres.ToString(), resfont, Brushes.Black, i * 70 + 35 - mscur.Width / 2, 17.5f - mscur.Height / 2);
                    }
                    g.DrawLine(Pens.Black, 1, 1, 11 * 70, 1);
                    g.DrawLine(Pens.Black, 0, 35, 11 * 70, 35);
                    g.DrawLine(Pens.Black, 11 * 70, 0, 11 * 70, 35);

                    SizeF mschange = g.MeasureString("Average Resistivity over Time [\u2126" + "m" + "]", resfont);
                    g.DrawString("Average Resistivity over Time [\u2126" + "m" + "]", resfont, Brushes.Black, _palette.Width / 2 - mschange.Width / 2, 40);
                }
                Invalidate();
                return _palette;
            }
        }

        public Bitmap FreePalette()
        {
            var ret = _palette;
            _palette = null;
            return ret;
        }

        private void InversionControl_Paint(object sender, PaintEventArgs e)
        {
            if(_palette != null)
            {
                e.Graphics.DrawImage(_palette, Width / 2 - _palette.Width / 2, 70);
            }
            if (ModelBlocks != null)
            {
                e.Graphics.Transform = trans;
                for (int i = 0; i < ModelBlocks.Length; i++)
                {
                    Color c;
                    if (MaxDifference != -1)
                    {
                        float dif = ModelBlocks[i].Difference - MinDifference;
                        if (dif < 0)
                            dif = 0;
                        double interpolation = Math.Log(dif + 1) / a;
                        if (interpolation < 0)
                            interpolation = 0;
                        else if (interpolation > 1)
                            interpolation = 1;
                        interpolation = Math.Round(interpolation * 10) / 10;
                        c = Helper.HSVToRGB(new Helper.HSV(0, interpolation, 1 - (interpolation / 4)));
                    }
                    else if (MaxAvgRes != -1 && MinAvgRes != -1)
                    {
                        float dif = ModelBlocks[i].AvgResistivity - MinAvgRes;
                        if (dif < 0)
                            dif = 0;
                        double interpolation = Math.Log(dif + 1) / b;
                        if (interpolation < 0)
                            interpolation = 0;
                        else if (interpolation > 1)
                            interpolation = 1;
                        interpolation = Math.Round(interpolation * 10) / 10;
                        c = Helper.HSVToRGB(new Helper.HSV(0, interpolation, 1 - (interpolation / 4)));
                    }
                    else
                    {
                        double interpolation = (double)(Math.Log(ModelBlocks[i].Resistivities[ActiveBlockSet]) / 5f);
                        //interpolation = Math.Round(interpolation * 20f) / 20f;
                        if (interpolation < 0)
                            interpolation = 0;
                        float degree = Hue - (float)interpolation * 360;
                        if (degree < 0)
                            degree += 360;
                        c = Helper.HSVToRGB(new Helper.HSV(degree, 1, 1));
                    }
                    e.Graphics.FillPolygon(new SolidBrush(c), ModelBlocks[i].DrawCorners.ToArray());
                    if (ModelBlocks[i].Number == selectedBlock)
                    {
                        e.Graphics.DrawPolygon(new Pen(Color.Black, 2 / (zoomlevel * 5f)), ModelBlocks[i].DrawCorners.ToArray());
                    }
                }
                e.Graphics.ScaleTransform(1f, -1f);
                e.Graphics.DrawImage(Properties.Resources.curve, new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height + 2), new Rectangle(0, 0, Properties.Resources.curve.Width, Properties.Resources.curve.Height), GraphicsUnit.Pixel);
            }
        }

        private void InversionControl_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void InversionControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (zoomlevel < 0.4f && e.Delta < 0 || ModelBlocks == null)
                return;

            if (_blockinfo.Visible)
                _blockinfo.Hide();

            zoomlevel += e.Delta / 120f / 20f;
            float offx = trans.OffsetX;
            float offy = trans.OffsetY;
            trans = new Matrix();
            trans.Translate(offx, offy);
            trans.Scale(zoomlevel * 5f, zoomlevel * -5f);
            Invalidate();
        }

        private void findClickedModelBlock(float x, float y)
        {
            float xx = (x - trans.OffsetX) / (zoomlevel * 5f);
            float yy = (y - trans.OffsetY) / (zoomlevel * -5f);
            if (xx >= bounds.Left && xx <= bounds.Right && yy >= bounds.Top && yy <= bounds.Bottom)
            {
                foreach (ModelBlock mb in ModelBlocks)
                {
                    if (Helper.IsInPolygon(mb.DrawCorners.ToArray(), new PointF(xx, yy)))
                    {
                        if (selectedBlock == mb.Number)
                        {
                            lblId.Text = "ID: " + selectedBlock;
                            lblRes.Text = "Resistivity: " + ModelBlocks[selectedBlock].Resistivities[ActiveBlockSet] + " \u2126" + "m";
                            lblCord.Text = "X: " + ModelBlocks[selectedBlock].RealCorners[0].X + " m\n" + "Y: " + ModelBlocks[selectedBlock].RealCorners[0].Y + " m";
                            if (_linechart != null)
                            {
                                _linechart.Resistivities = ModelBlocks[selectedBlock].Resistivities.ToArray();
                                _linechart.Dates = Dates;
                            }
                            _blockinfo.ArrowLocation = new Point((int)x, (int)y);
                            _blockinfo.Show();
                            return;
                        }
                        else
                        {
                            selectedBlock = mb.Number;
                            _blockinfo.Hide();
                            return;
                        }
                    }
                }
                selectedBlock = -1;
            }
        }

        private void initMatrix()
        {
            trans = new Matrix();
            trans.Translate(Width / 2, Height / 2);
            trans.Scale(zoomlevel * 5f, zoomlevel * -5f);
            Invalidate();
        }

        private void InversionControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (ModelBlocks == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                findClickedModelBlock(e.X, e.Y);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                _blockinfo.Hide();
                xBefore = e.X;
                yBefore = e.Y;
            }
            Invalidate();
        }

        private void InversionControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                if (ModelBlocks == null)
                    return;

                _blockinfo.Hide();
                trans.Translate((e.X - xBefore) / (zoomlevel * 5f), (-e.Y + yBefore) / (zoomlevel * 5f));
                xBefore = e.X;
                yBefore = e.Y;
                Invalidate();
            }
        }

        private OverlayPanel _blockinfo; //overlay panel that shows info about the block
        private Label lblId; //id of block
        private Label lblRes; //resistivity of block
        private Label lblCord; //coordinates of block
        private Button linechartBtn; //button to show/hide linechart
        private Button scalelinechartBtn; //button to normalize y scaling on linechart
        private void InversionControl_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(33, 33, 33);
            _maxdifference = -1;
            _maxaverageres = -1;
            _minaverageres = -1;
            #region init_blockinfo
            _blockinfo = new OverlayPanel();
            _blockinfo.SuspendLayout();
            _blockinfo.Hide();
            lblId = new Label();
            lblId.ForeColor = SystemColors.ControlDark;
            lblId.Location = new Point(0, 0);
            lblId.Height = 15;
            lblRes = new Label();
            lblRes.ForeColor = SystemColors.ControlDark;
            lblRes.Location = new Point(0, 28);
            lblRes.Width = 140;
            lblRes.Height = 15;
            lblCord = new Label();
            lblCord.ForeColor = SystemColors.ControlDark;
            lblCord.Location = new Point(0, 47);
            lblCord.Width = 150;
            lblCord.Height = 30;
            linechartBtn = new Button();
            linechartBtn.BackColor = SystemColors.ControlDark;
            linechartBtn.BackgroundImage = Properties.Resources.line2;
            linechartBtn.BackgroundImageLayout = ImageLayout.Stretch;
            linechartBtn.FlatAppearance.BorderColor = SystemColors.ControlDarkDark;
            linechartBtn.FlatStyle = FlatStyle.Flat;
            linechartBtn.Location = new Point(150, 0);
            linechartBtn.Size = new Size(24, 24);
            linechartBtn.UseVisualStyleBackColor = false;
            linechartBtn.Click += new EventHandler(this.linechartBtn_Click);
            scalelinechartBtn = new Button();
            scalelinechartBtn.BackColor = SystemColors.ControlDark;
            scalelinechartBtn.BackgroundImage = Properties.Resources.scaley2;
            scalelinechartBtn.BackgroundImageLayout = ImageLayout.Stretch;
            scalelinechartBtn.FlatAppearance.BorderColor = SystemColors.ControlDarkDark;
            scalelinechartBtn.FlatStyle = FlatStyle.Flat;
            scalelinechartBtn.Size = new Size(24, 24);
            scalelinechartBtn.UseVisualStyleBackColor = false;
            scalelinechartBtn.Click += new EventHandler(this.scalelinechartBtn_Click);
            scalelinechartBtn.Hide();
            _blockinfo.Controls.Add(lblId);
            _blockinfo.Controls.Add(lblRes);
            _blockinfo.Controls.Add(lblCord);
            _blockinfo.Controls.Add(linechartBtn);
            _blockinfo.ResumeLayout();
            Controls.Add(_blockinfo);
            #endregion
            _defaultSize = new Size(_blockinfo.Width, _blockinfo.Height);
        }

        private void scalelinechartBtn_Click(object sender, EventArgs e)
        {
            _linechart.NormalizeY = !_linechart.NormalizeY;
        }

        private LineChart _linechart; //shows the change of resistivity over time per modelblock
        private void linechartBtn_Click(object sender, EventArgs e)
        {
            if (_linechart == null)
            {
                _linechart = new LineChart();
                _linechart.Size = new Size(250, 150);
                _linechart.Resistivities = ModelBlocks[selectedBlock].Resistivities.ToArray();
                _linechart.Dates = Dates;
                _linechart.Location = new Point(0, lblCord.Location.Y + lblCord.Height);
                _blockinfo.Controls.Add(_linechart);
                linechartBtn.Location = new Point(_blockinfo.Width - 34, 25);
                _blockinfo.Controls.Add(scalelinechartBtn);
                scalelinechartBtn.Location = new Point(linechartBtn.Location.X - 34, 25);
                scalelinechartBtn.Show();
            }
            else
            {
                if (_linechart.Visible)
                {
                    _linechart.Hide();
                    ResetBlockInfo();
                }
                else
                {
                    _linechart.Location = new Point(0, _linechart.Location.Y - 15);
                    _blockinfo.Controls.Add(_linechart);
                    linechartBtn.Location = new Point(_blockinfo.Width - 34, 25);
                    _blockinfo.Controls.Add(scalelinechartBtn);
                    scalelinechartBtn.Location = new Point(linechartBtn.Location.X - 34, 25);
                    scalelinechartBtn.Show();
                    _linechart.Show();
                }
            }
        }

        private Size _defaultSize; //default size of the overlay (linechart hidden)
        private void ResetBlockInfo()
        {
            _blockinfo.Controls.Remove(_linechart);
            _blockinfo.Controls.Remove(scalelinechartBtn);
            scalelinechartBtn.Hide();
            _blockinfo.Size = new Size(_defaultSize.Width, _defaultSize.Height);
            linechartBtn.Location = new Point(160, 25);
        }
    }
}
