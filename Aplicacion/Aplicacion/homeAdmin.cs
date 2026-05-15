using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aplicacion
{
    public partial class homeAdmin : Form
    {
        public homeAdmin()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            this.Hide();
            login.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            administrarEstudiantes estudiantes = new administrarEstudiantes();
            this.Hide();
            estudiantes.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            generarReporteEstudiantes reportesEstudiantes = new generarReporteEstudiantes();
            this.Hide();
            reportesEstudiantes.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            administrarDocentes docentes = new administrarDocentes();
            this.Hide();
            docentes.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            generarReporteDocentes reporteDocentes = new generarReporteDocentes();
            this.Hide();
            reporteDocentes.Show();
        }
    }




}
