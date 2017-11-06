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

namespace Analyzer
{
    public partial class LineChart : UserControl
    {
        private Bitmap _bmp;
        private float _minres;
        private float _maxres;
        private Pen _linepen;
        private Pen _shadowpen;
        private Brush _backbrush;
        private DateTime[] _dates;
        private bool _normalizeY;
        private float[] _resistivities;
        public float[] Resistivities
        {
            get { return _resistivities; }
            set
            {
                _resistivities = value;
                _minres = value.Min();
                _maxres = value.Max();
                DrawBitmap();
            }
        }
        public bool NormalizeY
        {
            get
            {
                return _normalizeY;
            }
            set
            {
                _normalizeY = value;
                DrawBitmap();
            }
        }
        public DateTime[] Dates
        {
            get { return _dates; }
            set
            {
                _dates = value;
                DrawBitmap();
            }
        }

        public LineChart()
        {
            _axes = new Font("Microsoft Sans Serif", 7);
            _linepen = new Pen(Color.FromArgb(0, 120, 215));
            _shadowpen = new Pen(Color.FromArgb(33, 93, 141));
            _backbrush = new SolidBrush(Color.FromArgb(66, 66, 66));
            _xoffs = 0;
            _selected = -1;
            _normalizeY = false;
            InitializeComponent();
        }

        private void LineChart_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(66, 66, 66);
            saveCsvDialog.Filter = "Comma-separated values (*.csv)|*.csv";
            DrawBitmap();
        }

        private float xscale;
        public void DrawBitmap()
        {
            if (Resistivities == null || Dates == null)
            {
                _bmp = new Bitmap(Width, Height);
                Graphics g = Graphics.FromImage(_bmp);
                SizeF measure = g.MeasureString("Loading...", _axes);
                g.DrawString("Loading...", _axes, SystemBrushes.ControlDark, Width / 2 - measure.Width / 2, Height / 2 - measure.Height / 2);
            }
            else
            {
                xscale = Resistivities.Length < Width - 25 ? (Width - 25) / Resistivities.Length : 2f;
                _bmp = new Bitmap((int)(25 + Resistivities.Length * xscale + 20), Height);
                Graphics g = Graphics.FromImage(_bmp);
                g.TranslateTransform(25, Height - 15);
                g.ScaleTransform(1, -1);

                if (NormalizeY)
                {
                    PointF last = new PointF(0, Resistivities[0] / 200f * (Height - 25));
                    for (int i = 1; i < Resistivities.Length; i++)
                    {
                        g.DrawLine(_shadowpen, last.X, last.Y - 1, i * xscale, Resistivities[i] / 200f * (Height - 25) - 1);
                        g.DrawLine(_linepen, last, last = new PointF(i * xscale, Resistivities[i] / 200f * (Height - 25)));
                    }
                }
                else
                {
                    PointF last = new PointF(0, Resistivities[0] / _maxres * (Height * 0.75f));
                    for (int i = 1; i < Resistivities.Length; i++)
                    {
                        g.DrawLine(_shadowpen, last.X, last.Y - 1, i * xscale, Resistivities[i] / _maxres * (Height * 0.75f) - 1);
                        g.DrawLine(_linepen, last, last = new PointF(i * xscale, Resistivities[i] / _maxres * (Height * 0.75f)));
                    }
                }
                g.ResetTransform();
            }
            Invalidate();
        }

        private Font _axes;
        private float _xoffs;
        private void LineChart_Paint(object sender, PaintEventArgs e)
        {
            if (Resistivities != null && Dates != null)
            {
                e.Graphics.DrawImage(_bmp, new Rectangle(0, 0, Width, Height), new Rectangle((int)_xoffs, 0, Width, Height), GraphicsUnit.Pixel);
                e.Graphics.FillRectangle(_backbrush, 0, 0, 25, Height - 15);
                e.Graphics.DrawLine(SystemPens.ControlDark, 25, 10, 25, Height - 10);
                e.Graphics.DrawLine(SystemPens.ControlDark, 20, Height - 15, Width - 20, Height - 15);

                if (_selected != -1)
                    e.Graphics.DrawString(Dates[_selected].ToString("dd.MM.yyyy HH:mm:ss"), _axes, SystemBrushes.ControlDark, 30, Height - 13);
                else
                    e.Graphics.DrawString("Click a value to show its date", _axes, SystemBrushes.ControlDark, 30, Height - 13);

                e.Graphics.TranslateTransform(25, Height - 15);
                e.Graphics.ScaleTransform(1f, -1f);
                float y = 0;
                for(int i = 1; y < this.Height; i++)
                {
                    if (NormalizeY)
                        y = i * 50f / 200f * (Height - 25);
                    else
                        y = i * 50f / _maxres * (Height * 0.75f);

                    if (y > Height)
                        break;

                    if (i % 2 == 0)
                    {
                        e.Graphics.DrawLine(SystemPens.ControlDark, -2, y, 2, y);
                        e.Graphics.ScaleTransform(1f, -1f);
                        e.Graphics.DrawString(i * 50 + "", _axes, SystemBrushes.ControlDark, -22, -y - 6);
                        e.Graphics.ScaleTransform(1f, -1f);

                    }
                    else
                    {
                        e.Graphics.DrawLine(SystemPens.ControlDark, -1, y, 1, y);
                    }
                }
                e.Graphics.ResetTransform();

                e.Graphics.TranslateTransform(12, Height - 18);
                e.Graphics.RotateTransform(270);
                e.Graphics.DrawString("\u2126" + "m", _axes, SystemBrushes.ControlDark, 0, 0);
                e.Graphics.ResetTransform();
            }
            else
            {
                e.Graphics.DrawImageUnscaled(_bmp, 0, 0);
            }
        }

        private float xBefore;
        private int _selected;
        private void LineChart_MouseDown(object sender, MouseEventArgs e)
        {
            if (Resistivities == null || Dates == null)
                return;

            if(e.Button == MouseButtons.Middle)
            {
                xBefore = e.X;
            }
            else if (e.Button == MouseButtons.Left)
            {
                float temp = (_xoffs + e.X - 25) / xscale;
                _selected = temp < 0 ? -1 : (temp > Resistivities.Length - 1 ? -1 : (int)Math.Round(temp));
                Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if(saveCsvDialog.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();
                    for(int i = 0; i < Resistivities.Length; i++)
                    {
                        sb.Append(Dates[i].ToString("yyyy.MM.dd HH:mm:ss") + ";" + Resistivities[i] + Environment.NewLine);
                    }
                    System.IO.File.WriteAllText(saveCsvDialog.FileName, sb.ToString());
                }
            }
        }

        private void LineChart_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                if (Resistivities == null || Dates == null)
                    return;

                _xoffs += xBefore - e.X;
                if(_xoffs < 0)
                {
                    _xoffs = 0;
                } else if(_xoffs > _bmp.Width - this.Width)
                {
                    _xoffs = _bmp.Width - this.Width;
                }
                xBefore = e.X;

                Invalidate();
            }
        }
    }
}
