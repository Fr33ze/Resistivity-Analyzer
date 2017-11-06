using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Analyzer
{
    public partial class InversionsPerDay : Form
    {
        private Bitmap _bmp;
        private Brush chartbrush;
        private Pen chartpen;
        private Font _valuefont;
        private Font _datefont;
        private Dictionary<string, int> valuesPerDay;
        private List<DateTime> _dates;

        private DateTime start;
        private DateTime end;
        private int alldays;
        public List<DateTime> Dates {
            get
            {
                return _dates;
            }
            set
            {
                _dates = value;
                SetValuesPerDay();
                DrawBitmap();
            }
        }

        public InversionsPerDay()
        {
            InitializeComponent();
            chartbrush = new SolidBrush(Color.FromArgb(0, 120, 215));
            chartpen = new Pen(chartbrush, 1f);
            _valuefont = new Font("Microsoft Sans Serif", 10);
            _datefont = new Font("Microsoft Sans Serif", 7);
            BackColor = Color.FromArgb(66, 66, 66);
        }

        private void SetValuesPerDay()
        {
            valuesPerDay = new Dictionary<string, int>();
            start = new DateTime(Dates[0].Year, Dates[0].Month, Dates[0].Day);
            DateTime current = new DateTime(Dates[0].Year, Dates[0].Month, Dates[0].Day);
            end = new DateTime(Dates.Last().Year, Dates.Last().Month, Dates.Last().Day);
            alldays = (int)(end - current).TotalDays;
            while(!current.Equals(end))
            {
                int values = Dates.Where(x => x.Year == current.Year && x.Month == current.Month && x.Day == current.Day).Count();
                valuesPerDay.Add(current.ToString("yyyy.MM.dd"), values);
                current = current.AddDays(1);
            }
        }

        private void DrawBitmap()
        {
            _bmp = new Bitmap(alldays * 30, 300);
            using (Graphics g = Graphics.FromImage(_bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.TranslateTransform(0f, 150f);
                g.ScaleTransform(1f, -1f);
                DateTime current = start;
                for(int i = 0; i < alldays; i++)
                {
                    int invnum = valuesPerDay[current.ToString("yyyy.MM.dd")];
                    if (invnum != 0)
                    {
                        g.FillRectangle(chartbrush, i * 30, 0, 15, invnum * 25);
                        g.ScaleTransform(1f, -1f);
                        SizeF ms = g.MeasureString(invnum.ToString(), _valuefont);
                        g.DrawString(invnum.ToString(), _valuefont, SystemBrushes.ControlDark, i * 30 + (15 - ms.Width) / 2 + 1, -(invnum * 25 / 2 + ms.Height / 2));
                        g.RotateTransform(-90f);
                        string todrawdate = current.ToString("yyyy.MM.dd");
                        SizeF msdate = g.MeasureString(todrawdate, _datefont);
                        g.DrawString(todrawdate, _datefont, SystemBrushes.ControlDark, -70, i * 30 + (15 - msdate.Height) / 2 + 1);
                        g.DrawLine(chartpen, -(70 - msdate.Width), i * 30 + msdate.Height / 2, 0, i * 30 + msdate.Height / 2 + 1);
                        g.RotateTransform(90f);
                        g.ScaleTransform(1f, -1f);
                    } else
                    {
                        g.DrawLine(chartpen, i * 30, 1, i * 30 + 15, 1);
                        g.ScaleTransform(1f, -1f);
                        SizeF ms = g.MeasureString(invnum.ToString(), _valuefont);
                        g.DrawString(invnum.ToString(), _valuefont, SystemBrushes.ControlDark, i * 30 + (15 - ms.Width) / 2 + 1, -ms.Height - 1);
                        g.RotateTransform(-90f);
                        string todrawdate = current.ToString("yyyy.MM.dd");
                        SizeF msdate = g.MeasureString(todrawdate, _datefont);
                        g.DrawString(todrawdate, _datefont, SystemBrushes.ControlDark, -70, i * 30 + (15 - msdate.Height) / 2 + 1);
                        g.DrawLine(chartpen, -(70 - msdate.Width), i * 30 + msdate.Height / 2, 0, i * 30 + msdate.Height / 2 + 1);
                        g.RotateTransform(90f);
                        g.ScaleTransform(1f, -1f);
                    }
                    current = current.AddDays(1);
                }
            }
            Invalidate();
        }

        private void InversionsPerDay_Paint(object sender, PaintEventArgs e)
        {
            if (_bmp != null)
            {
                float trans = _bmp.Width / 100 * hScrollBar1.Value;
                e.Graphics.TranslateTransform(-trans, 0);
                e.Graphics.DrawImage(_bmp, 0, 0);
            }
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
