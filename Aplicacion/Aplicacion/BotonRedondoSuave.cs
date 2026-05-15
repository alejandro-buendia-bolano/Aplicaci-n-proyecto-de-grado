using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel; // Para que aparezca bien en el diseñador

namespace Aplicacion
{
    // Heredamos de Button para tener toda su funcionalidad
    public class BotonRedondoSuave : Button
    {
        // Propiedades que aparecerán en el diseñador
        [Category("Bordes Personalizados")]
        [Description("El radio de las esquinas.")]
        public int RadioEsquina { get; set; } = 30; // Valor por defecto

        [Category("Bordes Personalizados")]
        [Description("El color del borde suavizado.")]
        public Color ColorBordeSuavizado { get; set; } = Color.FromArgb(100, 100, 100); // Un gris suave opcional

        [Category("Bordes Personalizados")]
        [Description("Grosor del borde (0 para sin borde).")]
        public int GrosorBorde { get; set; } = 0;

        public BotonRedondoSuave()
        {
            // Configuraciones críticas para que funcione
            this.FlatStyle = FlatStyle.Flat; // Necesario para personalizar el dibujado
            this.FlatAppearance.BorderSize = 0; // Quitamos el borde por defecto de Windows
            this.Size = new Size(150, 40); // Tamaño por defecto
            this.BackColor = Color.FromArgb(100, 100, 255); // Tu color azul de la imagen
            this.ForeColor = Color.White;
        }

        // Esta es la función mágica que dibuja el borde con curvas suaves
        private GraphicsPath ObtenerRutaCurva(Rectangle rect, int radio)
        {
            GraphicsPath path = new GraphicsPath();
            int diametro = radio * 2;

            // Evitar errores si el botón es más pequeño que el radio
            if (diametro > rect.Width) diametro = rect.Width;
            if (diametro > rect.Height) diametro = rect.Height;
            if (diametro <= 0) { path.AddRectangle(rect); return path; }

            path.StartFigure();
            // Esquina superior izquierda
            path.AddArc(rect.X, rect.Y, diametro, diametro, 180, 90);
            // Esquina superior derecha
            path.AddArc(rect.Right - diametro, rect.Y, diametro, diametro, 270, 90);
            // Esquina inferior derecha
            path.AddArc(rect.Right - diametro, rect.Bottom - diametro, diametro, diametro, 0, 90);
            // Esquina inferior izquierda
            path.AddArc(rect.X, rect.Bottom - diametro, diametro, diametro, 90, 90);
            path.CloseFigure();

            return path;
        }

        // Sobrescribimos el evento Paint para dibujar nuestro botón
        protected override void OnPaint(PaintEventArgs pevent)
        {
            // IMPORTANTE: Primero llama a base.OnPaint pero solo para el texto, no el fondo.
            // Esto es complejo en C#, así que lo dibujaremos todo nosotros para control total.
            // base.OnPaint(pevent); // NO llamar a base.OnPaint() para el fondo

            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias; // AQUÍ ESTÁ LA SOLUCIÓN

            Rectangle rectSurface = this.ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -1, -1);

            using (GraphicsPath pathSurface = ObtenerRutaCurva(rectSurface, RadioEsquina))
            using (GraphicsPath pathBorder = ObtenerRutaCurva(rectBorder, RadioEsquina - 1))
            using (SolidBrush brushFondo = new SolidBrush(this.BackColor))
            using (Pen penSurface = new Pen(this.Parent.BackColor, 2)) // Para suavizar el corte de la región
            using (Pen penBorder = new Pen(ColorBordeSuavizado, GrosorBorde))
            {
                // 1. Establecer la región para el "recorte" pero solo para el click del ratón
                // No para el dibujado, porque el dibujado de región es dentado.
                this.Region = new Region(pathSurface);

                // 2. Dibujar el fondo del botón con el GraphicsPath (esto lo hace suave)
                g.FillPath(brushFondo, pathSurface);

                // 3. Opcional: Dibujar un borde suave encima si GrosorBorde > 0
                if (GrosorBorde >= 1)
                {
                    g.DrawPath(penBorder, pathBorder);
                }

                // 4. Dibujar el Texto del botón (esto reemplaza a base.OnPaint)
                TextRenderer.DrawText(g, this.Text, this.Font, rectSurface, this.ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }
    }
}