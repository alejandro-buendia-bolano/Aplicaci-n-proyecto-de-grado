using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Aplicacion
{
    public partial class Home : Form
    {
        public string NombreDocente { get; set; }
        private int idDocente;
        public Home(string nombre, int idDoc)
        {
            InitializeComponent();

            InitializeComponent();

            // Guardamos en las variables de la clase
            this.NombreDocente = nombre;
            this.idDocente = idDoc;

            ConfigurarDiseno();
            timer1.Start();
            ActualizarFechaHora();

            // Actualizamos el label inmediatamente
            label3.Text = "Bienvenido, " + this.NombreDocente;
        }

        // Creacion de metodo independiente para no repetir codigo
        private void ActualizarFechaHora()
        {
            label7.Text = DateTime.Now.ToString("hh:mm tt");
            label8.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ActualizarFechaHora();
        }

        private void ConfigurarDiseno()
        {
            // Metodo para redondear botones

            bordes_redondos.RedondearBordes(button3, 10);
            bordes_redondos.RedondearBordes(button4, 10);
            bordes_redondos.RedondearBordes(button5, 10);

            bordes_redondos.RedondearBordes(button6, 10);
            bordes_redondos.RedondearBordes(button9, 10);


            // Configuración de labels sobre botones
            bordes_redondos.RedondearBordes(button7, 10);

            label9.Parent = button3;
            label9.BackColor = Color.Transparent;
            label9.Location = new Point(15, 15);

            label10.Parent = button3;
            label10.BackColor = Color.Transparent;
            label10.Location = new Point(20, 50);

            var fondoDocente = button4;
            label14.Parent = fondoDocente;
            label13.Parent = fondoDocente;
            button9.Parent = fondoDocente;

            label14.BackColor = Color.Transparent;
            label13.BackColor = Color.Transparent;
            label14.ForeColor = Color.FromArgb(44, 53, 155);
            label13.ForeColor = Color.Black;
            label14.Location = new Point(20, 20);
            label13.Location = new Point(20, 50);
            button9.Location = new Point(20, 110);
        }

        private void log_out_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            this.Hide();
            login.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            AsistenciaEstudiantes asistenciaEstudiante = new AsistenciaEstudiantes(NombreDocente, idDocente);
            this.Hide();
            asistenciaEstudiante.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            asistenciaDocentes asistencia = new asistenciaDocentes(NombreDocente, idDocente);
            this.Hide();
            asistencia.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {
        }

        private void button7_Click(object sender, EventArgs e)
        {
            generarReporteEstudiantes reporte = new generarReporteEstudiantes(NombreDocente, idDocente);
            this.Hide();
            reporte.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void Home_load(object sender, EventArgs e)
        {
            label3.Text = $"Bienvenido, {NombreDocente}";
        }
    }
}