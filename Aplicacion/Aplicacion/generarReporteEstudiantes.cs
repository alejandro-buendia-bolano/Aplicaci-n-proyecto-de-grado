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
            comboBox3.Items.Add("Mes completo");                   // ← nuevo
            comboBox3.SelectedIndex = 0;
        }

        // ── Obtener rango de días según semana seleccionada ─────────────────────
        private (int diaInicio, int diaFin) ObtenerRangoDias()
        {
            int mes = comboBox2.SelectedIndex;
            int anio = DateTime.Now.Year;

            switch (comboBox3.SelectedIndex)
            {
                case 1: return (1, 7);
                case 2: return (8, 14);
                case 3: return (15, 21);
                case 4: return (22, 28);
                case 5: return (29, 31);
                case 6: return (1, DateTime.DaysInMonth(anio, mes)); // mes completo
                default: return (-1, -1);
            }
        }


        // ── GENERAR REPORTE ─────────────────────────────────────────────────────
        // ── GENERAR REPORTE ─────────────────────────────────────────────────────
        // ── GENERAR REPORTE ─────────────────────────────────────────────────────
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex <= 0) { MessageBox.Show("Seleccione un grado."); return; }
            if (comboBox2.SelectedIndex <= 0) { MessageBox.Show("Seleccione un mes."); return; }
            if (comboBox3.SelectedIndex <= 0) { MessageBox.Show("Seleccione un rango."); return; }

            string nombreGrado = comboBox1.SelectedItem.ToString();
            int mes = comboBox2.SelectedIndex;
            int anio = DateTime.Now.Year;
            var (diaInicio, diaFin) = ObtenerRangoDias();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT
                E.idEstudiante,
                E.primerNombre,
                ISNULL(E.segundoNombre,  '') AS segundoNombre,
                E.primerApellido,
                ISNULL(E.segundoApellido,'') AS segundoApellido,
                R.fecha,
                R.hora
            FROM  Estudiante      E
            INNER JOIN AulaEstudiante AE ON E.idEstudiante = AE.idEstudiante
            INNER JOIN Aula           A  ON AE.idAula      = A.idAula
            LEFT  JOIN Registro       R
                   ON  E.idEstudiante      = R.idEstudiante
                   AND YEAR (R.fecha)      = @anio
                   AND MONTH(R.fecha)      = @mes
                   AND DAY  (R.fecha) BETWEEN @diaInicio AND @diaFin
                   AND R.hora IS NOT NULL
            WHERE A.nombre = @grado
            ORDER BY E.primerApellido, E.primerNombre";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@grado", nombreGrado);
                cmd.Parameters.AddWithValue("@anio", anio);
                cmd.Parameters.AddWithValue("@mes", mes);
                cmd.Parameters.AddWithValue("@diaInicio", diaInicio);
                cmd.Parameters.AddWithValue("@diaFin", diaFin);

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dtRaw = new DataTable();
                adp.Fill(dtRaw);

                if (dtRaw.Rows.Count == 0)
                {
                    dataGridView1.DataSource = null;
                    MessageBox.Show("No hay estudiantes matriculados en este grado " +
                                    "para el período seleccionado.");
                    return;
                }

                // Construir tabla pivote y mostrarla
                DataTable dtPivot = ConstruirTablaPivote(dtRaw, anio, mes, diaInicio, diaFin);
                ConfigurarDGVAntesDeDatos();   // ← sin parámetros ahora
                dataGridView1.DataSource = dtPivot;
                ConfigurarEstiloDgv();
            }
        }

        private List<string> _columnasDias = new List<string>();

        private DataTable ConstruirTablaPivote(DataTable raw, int anio, int mes,
                                        int diaIni, int diaFin)
        {
            _columnasDias.Clear();
            var pivot = new DataTable();

            pivot.Columns.Add("Estudiante", typeof(string));

            int diasEnMes = DateTime.DaysInMonth(anio, mes);
            int diaFinReal = Math.Min(diaFin, diasEnMes);

            for (int d = diaIni; d <= diaFinReal; d++)
            {
                var fecha = new DateTime(anio, mes, d);

                // ── FIX 1: saltar sábado y domingo ──────────────────────────────
                if (fecha.DayOfWeek == DayOfWeek.Saturday ||
                    fecha.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                string wd = fecha.DayOfWeek switch
                {
                    DayOfWeek.Monday => "lun",
                    DayOfWeek.Tuesday => "mar",
                    DayOfWeek.Wednesday => "mié",
                    DayOfWeek.Thursday => "jue",
                    DayOfWeek.Friday => "vie",
                    _ => ""
                };
                string colKey = $"{wd} {d:D2}";
                pivot.Columns.Add(colKey, typeof(string));
                _columnasDias.Add(colKey);
            }

            pivot.Columns.Add("Total", typeof(string));

            var grupos = raw.AsEnumerable()
    .GroupBy(r => Convert.ToInt64(r["idEstudiante"]))
    .OrderBy(g =>
    {
        var f = g.First();
        return f["primerApellido"] + " " + f["primerNombre"];
    });

            foreach (var grupo in grupos)
            {
                var pf = grupo.First();
                string apell = $"{pf["primerApellido"]} {pf["segundoApellido"]}".Trim();
                string nom = $"{pf["primerNombre"]} {pf["segundoNombre"]}".Trim();

                var row = pivot.NewRow();
                row["Estudiante"] = $"{apell}, {nom}";

                int presentes = 0;

                foreach (string colKey in _columnasDias)
                {
                    // Reconstruir la fecha desde el nombre de columna ("lun 10" → día 10)
                    int dia = int.Parse(colKey.Split(' ')[1]);
                    var fecha = new DateTime(anio, mes, dia);

                    var reg = grupo.FirstOrDefault(r =>
                        r["fecha"] != DBNull.Value &&
                        ((DateTime)r["fecha"]).Date == fecha.Date &&
                        r["hora"] != DBNull.Value);

                    if (reg != null)
                    {
                        var hora = (TimeSpan)reg["hora"];
                        row[colKey] = hora.ToString(@"hh\:mm");
                        presentes++;
                    }
                    else
                    {
                        row[colKey] = "✗";
                    }
                }

                row["Total"] = $"{presentes}/{_columnasDias.Count}";
                pivot.Rows.Add(row);
            }

            return pivot;
        }

        private void ConfigurarDGVAntesDeDatos()
        {
            dataGridView1.AllowUserToAddRows = false;
            // ── FIX 2: Fill en lugar de None → elimina el espacio muerto ─────────
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.GridColor = System.Drawing.Color.FromArgb(220, 220, 220);
            dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.RowTemplate.Height = 36;
            dataGridView1.ColumnHeadersHeight = 42;

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(226, 232, 240);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor =
                System.Drawing.Color.FromArgb(248, 250, 252);
        }

        // ── COLOREAR FILAS ───────────────────────────────────────────────────────
        private void ConfigurarEstiloDgv()
        {
            // Número de días hábiles del período (para calcular peso proporcional)
            int numDias = _columnasDias.Count;

            // ── Columna Estudiante: ancla fija, peso mayor ────────────────────────
            if (dataGridView1.Columns.Contains("Estudiante"))
            {
                var col = dataGridView1.Columns["Estudiante"];
                col.MinimumWidth = 150;
                col.FillWeight = 220;   // más ancha que los demás
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                col.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
                col.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                col.Frozen = true;
            }

            // ── Columnas de días: peso igual entre ellas ──────────────────────────
            foreach (string colDia in _columnasDias)
            {
                if (!dataGridView1.Columns.Contains(colDia)) continue;
                var col = dataGridView1.Columns[colDia];
                col.MinimumWidth = 55;
                col.FillWeight = 80;
            }

            // ── Columna Total ─────────────────────────────────────────────────────
            if (dataGridView1.Columns.Contains("Total"))
            {
                var colT = dataGridView1.Columns["Total"];
                colT.MinimumWidth = 55;
                colT.FillWeight = 75;
                colT.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                colT.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
                colT.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
                colT.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // ── Colorear celdas de días ───────────────────────────────────────────
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                foreach (string colDia in _columnasDias)
                {
                    if (!dataGridView1.Columns.Contains(colDia)) continue;
                    var cell = row.Cells[colDia];
                    string val = cell.Value?.ToString() ?? "";

                    if (val == "✗")
                    {
                        cell.Style.BackColor = System.Drawing.Color.FromArgb(254, 226, 226);
                        cell.Style.ForeColor = System.Drawing.Color.FromArgb(153, 27, 27);
                        cell.Style.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
                    }
                    else if (!string.IsNullOrEmpty(val))
                    {
                        cell.Style.BackColor = System.Drawing.Color.FromArgb(220, 252, 231);
                        cell.Style.ForeColor = System.Drawing.Color.FromArgb(22, 101, 52);
                        cell.Style.Font = new System.Drawing.Font("Segoe UI", 8.5F);
                    }
                }
            }

            dataGridView1.Refresh();
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
            var fTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.DARK_GRAY);
            var fSub = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.GRAY);
            var fGen = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY);
            var fHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.WHITE);
            var fCelda = FontFactory.GetFont(FontFactory.HELVETICA, 7, BaseColor.BLACK);
            var fBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7, BaseColor.BLACK);

            string grado = comboBox1.SelectedItem?.ToString() ?? "";
            string mes = comboBox2.SelectedItem?.ToString() ?? "";
            string rango = comboBox3.SelectedItem?.ToString() ?? "";

            var doc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 15, 15, 25, 20);
            PdfWriter.GetInstance(doc, new FileStream(rutaArchivo, FileMode.Create));
            doc.Open();

            // Encabezado
            doc.Add(new Paragraph("Reporte de Asistencia – Estudiantes", fTitulo));
            doc.Add(new Paragraph($"Grado: {grado}  |  Mes: {mes}  |  {rango}", fSub));
            doc.Add(new Paragraph($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}", fGen));
            doc.Add(Chunk.NEWLINE);

            // Tabla
            int totalCols = dataGridView1.Columns.Count;
            var tabla = new PdfPTable(totalCols) { WidthPercentage = 100 };

            // Anchos proporcionales (igual que en docentes)
            float[] anchos = new float[totalCols];
            for (int i = 0; i < totalCols; i++)
            {
                string nombre = dataGridView1.Columns[i].Name;
                anchos[i] = nombre == "Estudiante" ? 3.5f
                          : nombre == "Total" ? 1.2f
                          : 1f;
            }
            tabla.SetWidths(anchos);

            // Encabezados
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                tabla.AddCell(new PdfPCell(new Phrase(col.HeaderText, fHeader))
                {
                    BackgroundColor = new BaseColor(30, 41, 59),
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 4
                });
            }

            // Filas con colores verde/rojo según asistencia
            int totalPresentes = 0, totalAusentes = 0;
            bool alterno = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                BaseColor fondoBase = alterno
                    ? new BaseColor(248, 250, 252)
                    : new BaseColor(255, 255, 255);
                alterno = !alterno;

                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    var cell = row.Cells[col.Index];
                    string valor = cell.Value?.ToString() ?? "";
                    bool esDia = _columnasDias.Contains(col.Name);

                    BaseColor bg = fondoBase;
                    iTextSharp.text.Font fuente = fCelda;

                    if (esDia)
                    {
                        if (valor == "✗")
                        {
                            // Rojo — ausente
                            bg = new BaseColor(254, 226, 226);
                            fuente = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9,
                                         new BaseColor(153, 27, 27));
                            totalAusentes++;
                        }
                        else if (!string.IsNullOrEmpty(valor))
                        {
                            // Verde — presente
                            bg = new BaseColor(220, 252, 231);
                            fuente = FontFactory.GetFont(FontFactory.HELVETICA, 7,
                                         new BaseColor(22, 101, 52));
                            totalPresentes++;
                        }
                    }
                    else if (col.Name == "Total")
                    {
                        fuente = fBold;
                        bg = new BaseColor(241, 245, 249);
                    }

                    tabla.AddCell(new PdfPCell(new Phrase(valor, fuente))
                    {
                        BackgroundColor = bg,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 3
                    });
                }
            }

            doc.Add(tabla);
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph(
                $"Total asistencias: {totalPresentes}     Total ausencias: {totalAusentes}",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.DARK_GRAY)));
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

        // Boton para exportar xlsx
        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Primero genere el reporte antes de exportar.");
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Exportar a Excel",
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = $"Reporte_Docentes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try { ExportarXLSX(sfd.FileName); MessageBox.Show($"Excel exportado en:\n{sfd.FileName}"); }
            catch (Exception ex) { MessageBox.Show("Error al exportar Excel: " + ex.Message); }
        }

        private void ExportarXLSX(string ruta)
        {
            using var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.Worksheets.Add("Asistencia Docentes");

            string seleccion = comboBox1.SelectedItem?.ToString() ?? "";
            bool todos = seleccion.StartsWith("★");
            string mes = comboBox2.SelectedItem?.ToString() ?? "";
            string rango = comboBox3.SelectedItem?.ToString() ?? "";

            ws.Cell(1, 1).Value = "Reporte de Asistencia – Docentes";
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Range(1, 1, 1, dataGridView1.Columns.Count).Merge();
            ws.Cell(2, 1).Value = todos ? "TODOS LOS DOCENTES" : $"Docente: {seleccion}";
            ws.Cell(3, 1).Value = $"Mes: {mes}  |  {rango}";
            ws.Cell(4, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Range(2, 1, 2, dataGridView1.Columns.Count).Merge();
            ws.Range(3, 1, 3, dataGridView1.Columns.Count).Merge();
            ws.Range(4, 1, 4, dataGridView1.Columns.Count).Merge();

            int filaEnc = 6;

            for (int c = 0; c < dataGridView1.Columns.Count; c++)
            {
                var cell = ws.Cell(filaEnc, c + 1);
                cell.Value = dataGridView1.Columns[c].HeaderText;
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
                cell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromArgb(30, 41, 59);
                cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
            }

            int fila = filaEnc + 1;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                for (int c = 0; c < dataGridView1.Columns.Count; c++)
                {
                    string colNombre = dataGridView1.Columns[c].Name;
                    string valor = row.Cells[c].Value?.ToString() ?? "";
                    var cell = ws.Cell(fila, c + 1);

                    cell.Value = valor;
                    cell.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                    if (_columnasDias.Contains(colNombre))
                    {
                        if (valor == "✗")
                        {
                            cell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromArgb(254, 226, 226);
                            cell.Style.Font.FontColor = ClosedXML.Excel.XLColor.FromArgb(153, 27, 27);
                            cell.Style.Font.Bold = true;
                        }
                        else if (!string.IsNullOrEmpty(valor))
                        {
                            cell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromArgb(220, 252, 231);
                            cell.Style.Font.FontColor = ClosedXML.Excel.XLColor.FromArgb(22, 101, 52);
                        }
                    }
                    else if (colNombre == "Total")
                    {
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromArgb(241, 245, 249);
                    }
                    else if (colNombre == "Docente")
                    {
                        cell.Style.Font.Bold = true;
                        cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Left;
                    }
                }
                fila++;
            }

            ws.Columns().AdjustToContents();
            ws.Column(1).Width = Math.Max(ws.Column(1).Width, 32);
            wb.SaveAs(ruta);
        }


    }
    
}