using System.Drawing.Drawing2D;
using System.Data;
using Microsoft.Data.SqlClient;
namespace Aplicacion
{
    public partial class Login : Form
    {
        public string NombreDocente { get; set; }
        
        public Login()
        {
            InitializeComponent();
            // Metodos para mejorar la interfaz 
            bordes_redondos.RedondearBordes(button5, 30);
            bordes_redondos.RedondearBordes(button4, 30);
            bordes_redondos.RedondearBordes(textBox1, 30);
            bordes_redondos.RedondearBordes(textBox2, 30);
            bordes_redondos.RedondearBordes(button3, 30);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            string user = textBox1.Text;
            string pass = textBox2.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection("Server=LAPTOP-VFH6P020; Database=DB_Asistencia; User Id=sa; Password=ACuptxhJT4!; TrustServerCertificate=True;"))
                {
                    conn.Open();

                    // Usamos LEFT JOIN para que los administradores (con idDocente NULL) puedan entrar
                    string query = @"SELECT U.tipoUsuario, D.primerNombre, U.idDocente 
                             FROM Usuario U 
                             LEFT JOIN Docente D ON U.idDocente = D.idDocente 
                             WHERE U.Usuario = @user AND U.Contraseña = @pass";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", user);
                        cmd.Parameters.AddWithValue("@pass", pass);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string tipo = reader["tipoUsuario"].ToString().ToLower();

                                // Si es administrador, el nombre será "Administrador"
                                // Si es docente, tomará el primerNombre de la tabla
                                string nombreParaMostrar = reader["primerNombre"] != DBNull.Value
                                    ? reader["primerNombre"].ToString()
                                    : "Administrador";

                                // El ID puede ser NULL para admins, manejamos eso con cuidado
                                int idParaPasar = reader["idDocente"] != DBNull.Value
                                    ? Convert.ToInt32(reader["idDocente"])
                                    : 0;

                                this.Hide();

                                if (tipo == "administrador")
                                {
                                    homeAdmin formAdmin = new homeAdmin();
                                    formAdmin.Show();
                                }
                                else if (tipo == "docente")
                                {
                                    // Enviamos los datos reales capturados
                                    Home formDocente = new Home(nombreParaMostrar, idParaPasar);
                                    formDocente.Show();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Usuario o contraseña incorrectos.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Crítico: " + ex.Message);
            }


        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        public void RedondearBordes(Control control, int radio)
        {
            GraphicsPath ruta = new GraphicsPath();
            int d = radio * 2;

            // Crear la forma con arcos en las esquinas
            ruta.AddArc(0, 0, d, d, 180, 90); // Superior izquierda
            ruta.AddArc(control.Width - d, 0, d, d, 270, 90); // Superior derecha
            ruta.AddArc(control.Width - d, control.Height - d, d, d, 0, 90); // Inferior derecha
            ruta.AddArc(0, control.Height - d, d, d, 90, 90); // Inferior izquierda
            ruta.CloseFigure();

            control.Region = new Region(ruta);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }


}
