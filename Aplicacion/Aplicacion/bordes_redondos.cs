using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
namespace Aplicacion
{
    internal class bordes_redondos
    {
        public static void RedondearBordes(Control control, int radio)
        {
            GraphicsPath ruta = new GraphicsPath();
            int d = radio * 2;

            // Evitar errores si el control es más pequeño que el radio
            if (d > control.Width || d > control.Height) d = Math.Min(control.Width, control.Height);

            ruta.AddArc(0, 0, d, d, 180, 90);
            ruta.AddArc(control.Width - d, 0, d, d, 270, 90);
            ruta.AddArc(control.Width - d, control.Height - d, d, d, 0, 90);
            ruta.AddArc(0, control.Height - d, d, d, 90, 90);
            ruta.CloseFigure();

            control.Region = new Region(ruta);
        }
    }
}
