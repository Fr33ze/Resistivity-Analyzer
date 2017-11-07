using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Analyzer
{
    public struct ModelBlock
    {
        public int Number { get; set; }
        public List<PointF> DrawCorners { get; set; }
        public List<PointF> RealCorners { get; set; }
        public List<float> Resistivities { get; set; }
        public float Difference { get; set; }
        public float AvgResistivity { get; set; }

        public void SetDifference()
        {
            for (int i = 1; i < Resistivities.Count; i++)
            {
                Difference += Math.Abs(Resistivities[i - 1] - Resistivities[i]);
            }
        }
    }

    public struct MinMaxResistivity
    {
        public MinMaxResistivity(float min, float max)
        {
            Minimum = min;
            Maximum = max;
        }
        public float Minimum { get; set; }
        public float Maximum { get; set; }
    }

    public static class Helper
    {
        public struct HSV
        {
            private double _h;
            private double _s;
            private double _v;

            public HSV(double h, double s, double v)
            {
                this._h = h;
                this._s = s;
                this._v = v;
            }

            public double H
            {
                get { return this._h; }
                set { this._h = value; }
            }

            public double S
            {
                get { return this._s; }
                set { this._s = value; }
            }

            public double V
            {
                get { return this._v; }
                set { this._v = value; }
            }

            public bool Equals(HSV hsv)
            {
                return (this.H == hsv.H) && (this.S == hsv.S) && (this.V == hsv.V);
            }
        }

        public static Color HSVToRGB(HSV hsv)
        {
            double r = 0, g = 0, b = 0;

            if (hsv.S == 0)
            {
                r = hsv.V;
                g = hsv.V;
                b = hsv.V;
            }
            else
            {
                int i;
                double f, p, q, t;

                if (hsv.H == 360)
                    hsv.H = 0;
                else
                    hsv.H = hsv.H / 60;

                i = (int)Math.Truncate(hsv.H);
                f = hsv.H - i;

                p = hsv.V * (1.0 - hsv.S);
                q = hsv.V * (1.0 - (hsv.S * f));
                t = hsv.V * (1.0 - (hsv.S * (1.0 - f)));

                switch (i)
                {
                    case 0:
                        r = hsv.V;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = hsv.V;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = hsv.V;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = hsv.V;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = hsv.V;
                        break;

                    default:
                        r = hsv.V;
                        g = p;
                        b = q;
                        break;
                }

            }
            return Color.FromArgb(Clamp(r * 255), Clamp(g * 255), Clamp(b * 255));
        }

        public static int Clamp(double input)
        {
            if (input < 0)
                return 0;
            else if (input > 255)
                return 255;

            return (int)input;
        }

        public static int Clamp(int input)
        {
            if (input < 0)
                return 0;
            else if (input > 255)
                return 255;

            return input;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static MinMaxResistivity GetMinMaxResistivityOfBlockset(ModelBlock[] input, int blockset)
        {
            float min = 1000000;
            float max = -1000000;
            foreach (ModelBlock mb in input)
            {
                if (mb.Resistivities[blockset] < min)
                {
                    min = mb.Resistivities[blockset];
                }
                if (mb.Resistivities[blockset] > max)
                {
                    max = mb.Resistivities[blockset];
                }
            }

            return new MinMaxResistivity(min, max);
        }

        public static MinMaxResistivity GetMinMaxResistivityOfBlockOverTime(ModelBlock input)
        {
            return new MinMaxResistivity(input.Resistivities.Min(), input.Resistivities.Max());
        }

        public static MinMaxResistivity GetMinMaxResistivityOfBlocksetOverTime(ModelBlock[] input)
        {
            float min = 1000000;
            float max = -1000000;

            foreach (ModelBlock mb in input)
            {
                float currentmin = mb.Resistivities.Min();
                float currentmax = mb.Resistivities.Max();
                if (currentmin < min)
                {
                    min = currentmin;
                }
                if (currentmax > max)
                {
                    max = currentmax;
                }
            }

            return new MinMaxResistivity(min, max);
        }

        public static MinMaxResistivity GetAvgMinMaxResistivity(ModelBlock[] input)
        {
            float min = 1000000;
            float max = -1000000;

            foreach (ModelBlock mb in input)
            {
                float cur = mb.AvgResistivity;
                if (cur < min)
                    min = cur;
                if (cur > max)
                    max = cur;
            }

            return new MinMaxResistivity(min, max);
        }

        public static ModelBlock[] GetModelBlocksFromFile(string filename)
        {
            List<ModelBlock> mlist = new List<ModelBlock>();

            StreamReader sr = new StreamReader(filename);
            //Skip to modelblock part
            while (!sr.ReadLine().Contains("/Coordinates of model blocks (with topography).")) { }
            for (int i = 0; i < 4; i++) { sr.ReadLine(); }

            Regex regex = new Regex(" +");
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line == "")
                {
                    break;
                }
                string[] result = regex.Split(line.Trim());
                ModelBlock mb = new ModelBlock();
                mb.RealCorners = new List<PointF>();
                mb.RealCorners.Add(new PointF(float.Parse(result[1], CultureInfo.InvariantCulture), float.Parse(result[2], CultureInfo.InvariantCulture)));
                mb.RealCorners.Add(new PointF(float.Parse(result[3], CultureInfo.InvariantCulture), float.Parse(result[4], CultureInfo.InvariantCulture)));
                mb.RealCorners.Add(new PointF(float.Parse(result[5], CultureInfo.InvariantCulture), float.Parse(result[6], CultureInfo.InvariantCulture)));
                mb.RealCorners.Add(new PointF(float.Parse(result[7], CultureInfo.InvariantCulture), float.Parse(result[8], CultureInfo.InvariantCulture)));

                mb.Resistivities = new List<float>();
                /*
                mb.Resistivities.Add(float.Parse(result[9], CultureInfo.InvariantCulture));
                */
                mb.Number = int.Parse(result[0], CultureInfo.InvariantCulture);
                mlist.Add(mb);
            }

            return mlist.ToArray();
        }

        public static float GetRMSError(string[] allxyz)
        {
            float error = 0f;
            for (int i = 0; i < allxyz.Length; i++)
            {
                StreamReader sr = new StreamReader(allxyz[i]);
                string rms = sr.ReadLine();
                //Skip to rms error part
                for (; ; rms = sr.ReadLine())
                {
                    if (rms == null)
                        break;

                    if (rms.Contains("/Percent RMS error for this model is   "))
                        break;
                }
                if(rms != null)
                    error += float.Parse(rms.Split(' ').Last(), CultureInfo.InvariantCulture);
            }

            return error / allxyz.Length;
        }

        public static int GetResistivitiesFromFile(ModelBlock[] input, string[] allxyz, List<DateTime> dates)
        {
            List<int> errorfiles = new List<int>();
            for (int time = 0; time < allxyz.Length; time++)
            {
                StreamReader sr = new StreamReader(allxyz[time]);
                //Skip to modelblock part
                while (!sr.ReadLine().Contains("/Coordinates of model blocks (with topography).")) { }
                for (int n = 0; n < 4; n++) { sr.ReadLine(); }

                for (int m = 0; m < input.Length; m++)
                {
                    string line = sr.ReadLine();
                    if (line == "")
                    {
                        errorfiles.Add(time);
                        for (int m2 = 0; m2 < m; m2++)
                        {
                            input[m2].Resistivities.RemoveAt(input[m2].Resistivities.Count - 1);
                        }
                        break;
                    }
                    else
                    {
                        input[m].Resistivities.Add(float.Parse(line.Substring(114), CultureInfo.InvariantCulture));
                    }
                }
            }
            for (int i = 0; i < input.Length; i++)
            {
                input[i].SetDifference();
                input[i].AvgResistivity = input[i].Resistivities.Average();
            }
            if (errorfiles.Count > 0)
            {
                ErrorFilesDialog efd = new ErrorFilesDialog();
                for (int i = errorfiles.Count - 1; i >= 0; i--)
                {
                    efd.AddErrorFile(allxyz[errorfiles[i]]);
                    dates.RemoveAt(errorfiles[i]);
                }
                efd.ShowDialog();
            }
            return errorfiles.Count;
        }

        public static RectangleF GetBounds(ModelBlock[] input, bool real)
        {
            float minx = 9999;
            float miny = 9999;
            float maxx = -9999;
            float maxy = -9999;

            foreach (ModelBlock mb in input)
            {
                foreach (PointF p in real ? mb.RealCorners : mb.DrawCorners)
                {
                    if (p.X < minx)
                        minx = p.X;
                    if (p.X > maxx)
                        maxx = p.X;
                    if (p.Y < miny)
                        miny = p.Y;
                    if (p.Y > maxy)
                        maxy = p.Y;
                }
            }

            return new RectangleF(minx, miny, maxx - minx, maxy - miny);
        }

        public static PointF NormalizeOrigin(ModelBlock[] input)
        {
            RectangleF bounds = GetBounds(input, true);
            PointF center = new PointF(bounds.Left + (bounds.Right - bounds.Left) / 2, bounds.Top + (bounds.Bottom - bounds.Top) / 2);
            for (int m = 0; m < input.Length; m++)
            {
                input[m].DrawCorners = new List<PointF>();
                foreach (PointF p in input[m].RealCorners)
                {
                    input[m].DrawCorners.Add(new PointF(p.X - center.X, p.Y - center.Y));
                }
            }

            return center; //normalization vector
        }

        public static bool IsInPolygon(PointF[] poly, PointF point)
        {
            var coef = poly.Skip(1).Select((p, i) =>
                                            (point.Y - poly[i].Y) * (p.X - poly[i].X)
                                          - (point.X - poly[i].X) * (p.Y - poly[i].Y))
                                    .ToList();

            if (coef.Any(p => p == 0))
                return true;

            for (int i = 1; i < coef.Count(); i++)
            {
                if (coef[i] * coef[i - 1] < 0)
                    return false;
            }
            return true;
        }

        public static Color InterpolateColor(Color[] colors, double x)
        {
            double r = 0.0, g = 0.0, b = 0.0;
            double total = 0.0;
            double step = 1.0 / (double)(colors.Length - 1);
            double mu = 0.0;
            double sigma_2 = 0.035;

            foreach (Color color in colors)
            {
                total += Math.Exp(-(x - mu) * (x - mu) / (2.0 * sigma_2)) / Math.Sqrt(2.0 * Math.PI * sigma_2);
                mu += step;
            }

            mu = 0.0;
            foreach (Color color in colors)
            {
                double percent = Math.Exp(-(x - mu) * (x - mu) / (2.0 * sigma_2)) / Math.Sqrt(2.0 * Math.PI * sigma_2);
                mu += step;

                r += color.R * percent / total;
                g += color.G * percent / total;
                b += color.B * percent / total;
            }

            return Color.FromArgb(255, (int)r, (int)g, (int)b);
        }
    }
}
