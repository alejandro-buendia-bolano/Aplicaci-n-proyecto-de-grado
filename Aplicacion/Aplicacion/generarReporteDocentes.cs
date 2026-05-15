using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;

namespace Aplicacion
{
    public partial class generarReporteDocentes : Form
    {
        string connectionString = "Server=LAPTOP-VFH6P020; Database=DB_Asistencia; User Id=sa; Password=ACuptxhJT4!; TrustServerCertificate=True;";

        public generarReporteDocentes()
        {
            InitializeComponent();
            CargarDocentes();
            CargarMeses();
            CargarRangosTiempo();
        }

        // ── Clase auxiliar para el ComboBox con ID ──────────────────────────────
        private class ComboItem
        {
            public string Id { get; set; }
            public string Texto { get; set; }
            public override string ToString() => Texto;
        }

        // ── Cargar ComboBox Docentes ────────────────────────────────────────────
        private void CargarDocentes()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("-- Seleccione un docente --");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT idDocente, 
                                        primerNombre + ' ' + primerApellido AS NombreCompleto 
                                 FROM Docente 
                                 ORDER BY primerApellido ASC";
                SqlCommand cmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        comboBox1.Items.Add(new ComboItem
                        {
                            Id = dr["idDocente"].ToString(),
                            Texto = dr["NombreCompleto"].ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar docentes: " + ex.Message);
                }
            }

            comboBox1.DisplayMember = "Texto";
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
            if (comboBox1.SelectedIndex <= 0) { MessageBox.Show("Seleccione un docente."); return; }
            if (comboBox2.SelectedIndex <= 0) { MessageBox.Show("Seleccione un mes."); return; }
            if (comboBox3.SelectedIndex <= 0) { MessageBox.Show("Seleccione un rango."); return; }

            string idDocente = ((ComboItem)comboBox1.SelectedItem).Id;
            int mes = comboBox2.SelectedIndex; // 1=Enero … 12=Diciembre
            var (diaInicio, diaFin) = ObtenerRangoDias();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // LEFT JOIN para incluir días sin asistencia (aparecen en rojo)
                // Generamos una fila por cada día del rango para mostrar presencia/ausencia
                string query = @"
                    SELECT  D.primerNombre        AS [Primer Nombre],
                            ISNULL(D.segundoNombre, '') AS [Segundo Nombre],
                            D.primerApellido      AS [Primer Apellido],
                            ISNULL(D.segundoApellido,'') AS [Segundo Apellido],
                            D.area                AS [Área],
                            R.fecha               AS [Fecha],
                            R.hora                AS [Hora]
                    FROM Docente D
                    LEFT JOIN Registro R 
                           ON D.idDocente = R.idDocente
                          AND MONTH(R.fecha) = @mes
                          AND DAY(R.fecha) BETWEEN @diaInicio AND @diaFin
                    WHERE D.idDocente = @idDocente
                    ORDER BY R.fecha";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idDocente", idDocente);
                cmd.Parameters.AddWithValue("@mes", mes);
                cmd.Parameters.AddWithValue("@diaInicio", diaInicio);
                cmd.Parameters.AddWithValue("@diaFin", diaFin);

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                dataGridView1.DataSource = dt;
                ConfigurarEstiloDgv();

                if (dt.Rows.Count == 0)
                    MessageBox.Show("No se encontraron registros para los filtros seleccionados.");
            }
        }

        // ── Estilo del DataGridView con colores asistencia/inasistencia ──────────
        private void ConfigurarEstiloDgv()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.None;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                bool tieneAsistencia = row.Cells["Fecha"].Value != null
                                    && row.Cells["Fecha"].Value != DBNull.Value;

                if (tieneAsistencia)
                {
                    // ASISTENCIA: verde claro
                    row.DefaultCellStyle.BackColor = Color.FromArgb(200, 255, 200);
                    row.DefaultCellStyle.ForeColor = Color.DarkGreen;
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(150, 220, 150);
                }
                else
                {
                    // INASISTENCIA: rojo claro
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 150, 150);
                }
            }
        }

        // ── EXPORTAR PDF con colores ────────────────────────────────────────────
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Primero genere el reporte antes de exportar.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Guardar reporte PDF";
                sfd.Filter = "Archivos PDF (*.pdf)|*.pdf";
                sfd.FileName = $"Reporte_Docentes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
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
            iTextSharp.text.Document doc = new iTextSharp.text.Document(
                PageSize.A4.Rotate(), 20, 20, 30, 20);
            PdfWriter.GetInstance(doc, new FileStream(rutaArchivo, FileMode.Create));
            doc.Open();

            var fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.DARK_GRAY);
            var fuenteSub = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.GRAY);
            var fuenteGen = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY);
            var fuenteHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);
            var fuenteVerde = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK);
            var fuenteRojo = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK);

            string docente = comboBox1.SelectedItem?.ToString() ?? "";
            string mes = comboBox2.SelectedItem?.ToString() ?? "";
            string rango = comboBox3.SelectedItem?.ToString() ?? "";

            // Encabezado del documento
            doc.Add(new Paragraph("Reporte de Asistencia – Docentes", fuenteTitulo));
            doc.Add(new Paragraph($"Docente: {docente}  |  Mes: {mes}  |  {rango}", fuenteSub));
            doc.Add(new Paragraph($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}", fuenteGen));
            doc.Add(Chunk.NEWLINE);

            // Leyenda de colores
            doc.Add(new Paragraph("■ Verde: Asistencia registrada     ■ Rojo: Sin asistencia",
                FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.DARK_GRAY)));
            doc.Add(Chunk.NEWLINE);

            // Tabla PDF
            int cols = dataGridView1.Columns.Count;
            PdfPTable tabla = new PdfPTable(cols) { WidthPercentage = 100 };

            // Encabezados de columna
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

            // Filas con color según asistencia
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Verificar si la fila tiene fecha (asistencia) o no (inasistencia)
                bool tieneAsistencia = row.Cells["Fecha"].Value != null
                                    && row.Cells["Fecha"].Value != DBNull.Value;

                // Verde claro para asistencia, rojo claro para inasistencia
                BaseColor fondoFila = tieneAsistencia
                    ? new BaseColor(200, 255, 200)   // verde claro
                    : new BaseColor(255, 200, 200);  // rojo claro

                foreach (DataGridViewCell cell in row.Cells)
                {
                    string texto = cell.Value?.ToString() ?? "";

                    // Formatear fecha legible si aplica
                    if (cell.OwningColumn.HeaderText == "Fecha" && tieneAsistencia)
                    {
                        if (DateTime.TryParse(texto, out DateTime fecha))
                            texto = fecha.ToString("dd/MM/yyyy");
                    }

                    // Formatear hora legible si aplica
                    if (cell.OwningColumn.HeaderText == "Hora" && tieneAsistencia
                        && TimeSpan.TryParse(texto, out TimeSpan hora))
                    {
                        texto = DateTime.Today.Add(hora).ToString("hh:mm tt");
                    }

                    PdfPCell pCell = new PdfPCell(new Phrase(texto, tieneAsistencia ? fuenteVerde : fuenteRojo))
                    {
                        BackgroundColor = fondoFila,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 4
                    };
                    tabla.AddCell(pCell);
                }
            }

            doc.Add(tabla);

            // Pie de página con totales
            int totalAsistencias = 0;
            int totalInasistencias = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Fecha"].Value != null && row.Cells["Fecha"].Value != DBNull.Value)
                    totalAsistencias++;
                else
                    totalInasistencias++;
            }

            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph(
                $"Total asistencias: {totalAsistencias}     Total inasistencias: {totalInasistencias}",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.DARK_GRAY)));

            doc.Close();
        }

        // ── Navegación ──────────────────────────────────────────────────────────
        private void button1_Click(object sender, EventArgs e)
        {
            homeAdmin admin = new homeAdmin();
            this.Hide();
            admin.Show();
        }

        private void generarReporteDocentes_Load(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}