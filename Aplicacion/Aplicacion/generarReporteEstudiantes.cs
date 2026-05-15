using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;

namespace Aplicacion
{
    public partial class generarReporteEstudiantes : Form
    {
        string connectionString = "Server=LAPTOP-VFH6P020; Database=DB_Asistencia; User Id=sa; Password=ACuptxhJT4!; TrustServerCertificate=True;";

        // Guardamos el rol para saber a qué home regresar
        private string _rolUsuario;
        private string _nombreDocente;
        private int _idDocente;

        // Constructor para Admin (sin nombre de docente)
        public generarReporteEstudiantes(string rolUsuario = "admin")
        {
            InitializeComponent();
            _rolUsuario = rolUsuario;
            CargarGrados();
            CargarMeses();
            CargarRangosTiempo();
        }

        // Constructor para Docente (con nombre para restaurar bienvenida)
        public generarReporteEstudiantes(string nombreDocente, int idDocente)
        {
            InitializeComponent();
            _rolUsuario = "docente";
            _nombreDocente = nombreDocente;
            _idDocente = idDocente;
            CargarGrados();
            CargarMeses();
            CargarRangosTiempo();
        }

        // ── Cargar ComboBox Grados ──────────────────────────────────────────────
        private void CargarGrados()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("-- Seleccione un grado --");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT nombre FROM Aula ORDER BY nombre ASC";
                SqlCommand cmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                        comboBox1.Items.Add(dr["nombre"].ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar grados: " + ex.Message);
                }
            }
            comboBox1.SelectedIndex = 0;
        }

        // ── Cargar ComboBox Meses ───────────────────────────────────────────────
        private void CargarMeses()
        {
            comboBox2.Items.Clear();
            comboBox2.Items.Add("-- Seleccione mes --");
            string[] meses = { "Enero","Febrero","Marzo","Abril","Mayo","Junio",
                                "Julio","Agosto","Septiembre","Octubre","Noviembre","Diciembre" };
            comboBox2.Items.AddRange(meses);
            comboBox2.SelectedIndex = 0;
        }

        // ── Cargar ComboBox Rango de tiempo (5 semanas) ─────────────────────────
        private void CargarRangosTiempo()
        {
            comboBox3.Items.Clear();
            comboBox3.Items.Add("-- Seleccione un rango de tiempo --");
            comboBox3.Items.Add("Semana 1: Día 1 – Día 7");
            comboBox3.Items.Add("Semana 2: Día 8 – Día 14");
            comboBox3.Items.Add("Semana 3: Día 15 – Día 21");
            comboBox3.Items.Add("Semana 4: Día 22 – Día 28");
            comboBox3.Items.Add("Semana 5: Día 29 – Día 31");
            comboBox3.SelectedIndex = 0;
        }

        // ── Obtener rango de días según semana seleccionada ─────────────────────
        private (int diaInicio, int diaFin) ObtenerRangoDias()
        {
            switch (comboBox3.SelectedIndex)
            {
                case 1: return (1, 7);
                case 2: return (8, 14);
                case 3: return (15, 21);
                case 4: return (22, 28);
                case 5: return (29, 31);
                default: return (-1, -1);
            }
        }

        // ── GENERAR REPORTE ─────────────────────────────────────────────────────
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex <= 0) { MessageBox.Show("Seleccione un grado."); return; }
            if (comboBox2.SelectedIndex <= 0) { MessageBox.Show("Seleccione un mes."); return; }
            if (comboBox3.SelectedIndex <= 0) { MessageBox.Show("Seleccione un rango."); return; }

            string nombreGrado = comboBox1.SelectedItem.ToString();
            int mes = comboBox2.SelectedIndex;
            var (diaInicio, diaFin) = ObtenerRangoDias();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Usamos LEFT JOIN para incluir a los estudiantes que NO tienen registros
                string query = $@"
            SELECT E.primerNombre AS [Primer Nombre], 
                   ISNULL(E.segundoNombre, '') AS [Segundo Nombre], 
                   E.primerApellido AS [Primer Apellido], 
                   ISNULL(E.segundoApellido, '') AS [Segundo Apellido],
                   R.fecha AS [Fecha]
            FROM Estudiante E
            INNER JOIN AulaEstudiante AE ON E.idEstudiante = AE.idEstudiante
            INNER JOIN Aula A ON AE.idAula = A.idAula
            LEFT JOIN Registro R ON E.idEstudiante = R.idEstudiante 
                AND MONTH(R.fecha) = @mes 
                AND DAY(R.fecha) BETWEEN @diaInicio AND @diaFin
            WHERE A.nombre = @grado
            ORDER BY E.primerApellido, E.primerNombre";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@grado", nombreGrado);
                cmd.Parameters.AddWithValue("@mes", mes);
                cmd.Parameters.AddWithValue("@diaInicio", diaInicio);
                cmd.Parameters.AddWithValue("@diaFin", diaFin);

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                dataGridView1.DataSource = dt;
                ConfigurarEstiloDgv();

                if (dt.Rows.Count == 0)
                    MessageBox.Show("No hay estudiantes matriculados en este grado.");
            }
        }



        private void ConfigurarEstiloDgv()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Verificamos si la celda de Fecha es NULL o está vacía
                if (row.Cells["Fecha"].Value != null && row.Cells["Fecha"].Value != DBNull.Value)
                {
                    // ASISTENCIA: Verde claro
                    row.DefaultCellStyle.BackColor = Color.FromArgb(200, 255, 200);
                }
                else
                {
                    // INASISTENCIA: Rojo claro
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);

    
                }
            }

            // Estética del DataGridView
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToAddRows = false; // Evita la fila vacía al final
        }

        // ── EXPORTAR PDF ────────────────────────────────────────────────────────
        // Requiere: instalar NuGet → iTextSharp  (Install-Package iTextSharp)
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Primero genere el reporte antes de exportar.");
                return;
            }

            // Cuadro de diálogo para elegir ruta de guardado
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Guardar reporte PDF";
                sfd.Filter = "Archivos PDF (*.pdf)|*.pdf";
                sfd.FileName = $"Reporte_Estudiantes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    ExportarPDF(sfd.FileName);
                    MessageBox.Show($"PDF exportado correctamente en:\n{sfd.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar PDF: " + ex.Message);
                }
            }
        }

        private void ExportarPDF(string rutaArchivo)
        {
            iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 20, 20, 30, 20);
            PdfWriter.GetInstance(doc, new FileStream(rutaArchivo, FileMode.Create));
            doc.Open();

            // Título
            var fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.DARK_GRAY);
            var fuenteHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);
            var fuenteCelda = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK);

            string grado = comboBox1.SelectedItem?.ToString() ?? "";
            string mes = comboBox2.SelectedItem?.ToString() ?? "";
            string rango = comboBox3.SelectedItem?.ToString() ?? "";

            doc.Add(new Paragraph($"Reporte de Asistencia – Estudiantes", fuenteTitulo));
            doc.Add(new Paragraph($"Grado: {grado}  |  Mes: {mes}  |  {rango}",
                FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.GRAY)));
            doc.Add(new Paragraph($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}",
                FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY)));
            doc.Add(Chunk.NEWLINE);

            // Tabla
            int cols = dataGridView1.Columns.Count;
            PdfPTable tabla = new PdfPTable(cols) { WidthPercentage = 100 };

            // Encabezados
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(col.HeaderText, fuenteHeader))
                {
                    BackgroundColor = new BaseColor(44, 62, 80),
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                tabla.AddCell(cell);
            }

            // Filas
            bool alterno = false;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                BaseColor fondo = alterno ? new BaseColor(236, 240, 241) : BaseColor.WHITE;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    PdfPCell pCell = new PdfPCell(new Phrase(cell.Value?.ToString() ?? "", fuenteCelda))
                    {
                        BackgroundColor = fondo,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 4
                    };
                    tabla.AddCell(pCell);
                }
                alterno = !alterno;
            }

            doc.Add(tabla);
            doc.Close();
        }

        // ── Navegación (CORRECCIÓN 4: regresar según rol) ───────────────────────
        private void button1_Click(object sender, EventArgs e)
        {
            if (_rolUsuario == "admin")
            {
                homeAdmin admin = new homeAdmin();
                this.Hide();
                admin.Show();
            }
            else
            {
                // Docente: restaurar el nombre de bienvenida
                Home home = new Home(_nombreDocente, _idDocente);
                this.Hide();
                home.Show();
            }
        }

        private void generarReporteEstudiantes_Load(object sender, EventArgs e) { }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Deja esto vacío por ahora o llama a tus otros métodos
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Deja esto vacío por ahora o llama a tus otros métodos
        }
    }
}