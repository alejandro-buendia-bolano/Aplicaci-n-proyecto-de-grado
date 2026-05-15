using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Aplicacion
{
    public class BotonRedondo : Button
    {
        // Propiedades para ajustar desde el diseñador
        public int RadioBorde { get; set; } = 30;
        public float TamañoBorde { get; set; } = 0;
        public Color ColorBorde { get; set; } = Color.PaleVioletRed;

        public BotonRedondo()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Size = new Size(150, 40);
            this.BackColor = Color.MediumSlateBlue;
            this.ForeColor = Color.White;
        }

        private GraphicsPath GetFigurePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias; // Esto quita lo borroso

            Rectangle rectSurface = this.ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -1, -1);
            int smoothSize = 2;
            if (RadioBorde > 2)
            {
                using (GraphicsPath pathSurface = GetFigurePath(rectSurface, RadioBorde))
                using (GraphicsPath pathBorder = GetFigurePath(rectBorder, RadioBorde - 1))
                using (Pen penSurface = new Pen(this.Parent.BackColor, smoothSize))
                using (Pen penBorder = new Pen(ColorBorde, TamañoBorde))
                {
                    pevent.Graphics.SetClip(pathSurface);
                    // Dibujar el fondo
                    pevent.Graphics.FillPath(new SolidBrush(this.BackColor), pathSurface);

                    // Dibujar el borde suavizado
                    this.Region = new Region(pathSurface);
                    if (TamañoBorde >= 1)
                        pevent.Graphics.DrawPath(penBorder, pathBorder);
                }
            }
        }
    }
}