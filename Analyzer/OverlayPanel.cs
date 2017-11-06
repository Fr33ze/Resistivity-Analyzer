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
    public partial class OverlayPanel : UserControl
    {
        private PointF[] _arrow;
        private SolidBrush _backbrush;
        private Pen _borderpen;
        private PointF _arrowlocation;
        public PointF ArrowLocation
        {
            get
            {
                return this._arrowlocation;
            }
            set
            {
                this._arrowlocation = value;
                AdjustLocation();
                _arrow = new PointF[3];
                _arrow[0] = new PointF(_arrowlocation.X, 0);
                _arrow[1] = new PointF(_arrowlocation.X - 10, 15);
                _arrow[2] = new PointF(_arrowlocation.X + 10, 15);
            }
        }

        public OverlayPanel()
        {
            InitializeComponent();
            _backbrush = new SolidBrush(Color.FromArgb(66, 66, 66));
            _borderpen = new Pen(SystemColors.ControlDark, 1);
        }

        private void OverlayPanel_ControlAdded(object sender, ControlEventArgs e)
        {
            e.Control.Location = new Point(e.Control.Location.X < 0 ? 10 : e.Control.Location.X + 10, e.Control.Location.Y < 10 ? 25 : e.Control.Location.Y + 15);
            if (e.Control.Location.X + e.Control.Width > this.Width)
                this.Width = e.Control.Location.X + e.Control.Width + 10;
            if (e.Control.Location.Y + e.Control.Height > this.Height)
                this.Height = e.Control.Location.Y + e.Control.Height + 10;

            Invalidate();
        }

        private void AdjustLocation()
        {
            if (Parent == null || ArrowLocation == null)
                return;

            Location = new Point((int)ArrowLocation.X - Width / 2, (int)ArrowLocation.Y);
            if (this.Location.X + this.Width > Parent.Width)
            {
                this.Location = new Point(Parent.Width - this.Width, this.Location.Y);
            }
            else if (this.Location.X < 0)
            {
                this.Location = new Point(0, this.Location.Y);
            }
            _arrowlocation = new PointF(ArrowLocation.X - Location.X, 0);
        }

        private void OverlayPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_arrow == null)
                return;

            e.Graphics.FillPolygon(_backbrush, _arrow);
            e.Graphics.FillRectangle(_backbrush, 0, 15, Width, Height - 15);
            using (GraphicsPath gp = new GraphicsPath())
            {
                PointF upperLeft = new PointF(0, 15);
                PointF upperRight = new PointF(Width - 1, 15);
                PointF lowerRight = new PointF(Width - 1, Height - 1);
                PointF lowerLeft = new PointF(0, Height - 1);
                gp.AddLine(upperLeft, _arrow[1]);
                gp.AddLine(_arrow[1], _arrow[0]);
                gp.AddLine(_arrow[0], _arrow[2]);
                gp.AddLine(_arrow[2], upperRight);
                gp.AddLine(upperRight, lowerRight);
                gp.AddLine(lowerRight, lowerLeft);
                gp.AddLine(lowerLeft, upperLeft);
                gp.CloseFigure();
                e.Graphics.DrawPath(_borderpen, gp);
            }
        }
    }
}
