using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;

// NOTA: NO hay "using System.Drawing;" — se califica explícitamente abajo
// para evitar CS0104 ('Font' ambiguo entre System.Drawing.Font e iTextSharp.text.Font)

namespace Aplicacion
{
    public partial class generarReporteDocentes : Form
    {
        private const string ConnStr =
            "Server=LAPTOP-VFH6P020; Database=DB_Asistencia; User Id=sa; " +
            "Password=ACuptxhJT4!; TrustServerCertificate=True;";

        // Guardamos los IDs para usarlos en las consultas
        private readonly Dictionary<string, string> _docenteIds = new();
        private List<string> _columnasDias = new();

        public generarReporteDocentes()
        {
            InitializeComponent();
            CargarDocentes();
            CargarMeses();
            CargarRangosTiempo();
        }

        // ── Carga de ComboBoxes ───────────────────────────────────────────────

        private void CargarDocentes()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("-- Seleccione un docente --");
            // Opción especial para ver todos
            comboBox1.Items.Add("★  TODOS LOS DOCENTES");
            _docenteIds["★  TODOS LOS DOCENTES"] = "TODOS";

            using var conn = new SqlConnection(ConnStr);
            try
            {
                conn.Open();
                using var cmd = new SqlCommand(
                    @"SELECT idDocente,
                             primerNombre + ' ' + primerApellido AS NombreCompleto
                      FROM Docente ORDER BY primerApellido ASC", conn);
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string texto = dr["NombreCompleto"].ToString()!;
                    string id = dr["idDocente"].ToString()!;
                    comboBox1.Items.Add(texto);
                    _docenteIds[texto] = id;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar docentes: " + ex.Message);
            }

            comboBox1.SelectedIndex = 0;
        }

        private void CargarMeses()
        {
            comboBox2.Items.Clear();
            comboBox2.Items.Add("-- Seleccione mes --");
            comboBox2.Items.AddRange(new object[]
            {
                "Enero","Febrero","Marzo","Abril","Mayo","Junio",
                "Julio","Agosto","Septiembre","Octubre","Noviembre","Diciembre"
            });
            comboBox2.SelectedIndex = 0;
        }

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

        private (int inicio, int fin) ObtenerRangoDias() => comboBox3.SelectedIndex switch
        {
            1 => (1, 7),
            2 => (8, 14),
            3 => (15, 21),
            4 => (22, 28),
            5 => (29, 31),
            _ => (-1, -1)
        };

        // ── GENERAR REPORTE ───────────────────────────────────────────────────

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex <= 0) { MessageBox.Show("Seleccione un docente."); return; }
            if (comboBox2.SelectedIndex <= 0) { MessageBox.Show("Seleccione un mes."); return; }
            if (comboBox3.SelectedIndex <= 0) { MessageBox.Show("Seleccione un rango."); return; }

            string seleccion = comboBox1.SelectedItem!.ToString()!;
            bool todos = _docenteIds.TryGetValue(seleccion, out string? idVal)
                               && idVal == "TODOS";
            string idDocente = todos ? "" : (idVal ?? "");

            int mes = comboBox2.SelectedIndex;
            int anio = DateTime.Now.Year;
            var (diaIni, diaFin) = ObtenerRangoDias();

            using var conn = new SqlConnection(ConnStr);

            // LEFT JOIN filtra solo registros confirmados (hora NOT NULL)
            string filtroDocente = todos ? "" : "WHERE D.idDocente = @idDocente";
            string query = $@"
                SELECT
                    D.idDocente,
                    D.primerNombre,
                    ISNULL(D.segundoNombre,'')  AS segundoNombre,
                    D.primerApellido,
                    ISNULL(D.segundoApellido,'') AS segundoApellido,
                    D.area,
                    R.fecha,
                    R.hora
                FROM Docente D
                LEFT JOIN Registro R
                       ON  D.idDocente     = R.idDocente
                       AND YEAR (R.fecha)  = @anio
                       AND MONTH(R.fecha)  = @mes
                       AND DAY  (R.fecha) BETWEEN @diaIni AND @diaFin
                       AND R.hora IS NOT NULL
                {filtroDocente}
                ORDER BY D.primerApellido, D.primerNombre, R.fecha";

            using var cmd = new SqlCommand(query, conn);
            if (!todos) cmd.Parameters.AddWithValue("@idDocente", idDocente);
            cmd.Parameters.AddWithValue("@anio", anio);
            cmd.Parameters.AddWithValue("@mes", mes);
            cmd.Parameters.AddWithValue("@diaIni", diaIni);
            cmd.Parameters.AddWithValue("@diaFin", diaFin);

            var adp = new SqlDataAdapter(cmd);
            var dtRaw = new DataTable();
            adp.Fill(dtRaw);

            if (dtRaw.Rows.Count == 0)
            {
                dataGridView1.DataSource = null;
                MessageBox.Show("No se encontraron registros para los filtros seleccionados.");
                return;
            }

            var dtPivot = ConstruirTablaPivote(dtRaw, anio, mes, diaIni, diaFin);
            ConfigurarDGVAntesDeDatos();   // ← sin parámetros ahora
            dataGridView1.DataSource = dtPivot;
            ConfigurarEstiloDgv();
        }

        // ── TABLA PIVOTE ──────────────────────────────────────────────────────

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
    .GroupBy(r => Convert.ToInt64(r["idDocente"]))       // ← usa idDocente
    .OrderBy(g =>
    {
        var f = g.First();
        return f["primerApellido"].ToString() + " " + f["primerNombre"].ToString();
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

        // ── CONFIGURAR DGV ────────────────────────────────────────────────────

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

        private void ConfigurarEstiloDgv(bool modoTodos)
        {
            // Columna Docente
            if (dataGridView1.Columns.Contains("Docente"))
            {
                var col = dataGridView1.Columns["Docente"];
                col.Width = modoTodos ? 200 : 190;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                col.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
                col.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                col.Frozen = true;
            }

            // Columna Área
            if (dataGridView1.Columns.Contains("Área"))
            {
                var col = dataGridView1.Columns["Área"];
                col.Width = 110;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                col.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            }

            // Columnas de días
            foreach (string colDia in _columnasDias)
            {
                if (dataGridView1.Columns.Contains(colDia))
                    dataGridView1.Columns[colDia].Width = 72;
            }

            // Columna Total
            if (dataGridView1.Columns.Contains("Total"))
            {
                var colT = dataGridView1.Columns["Total"];
                colT.Width = 68;
                colT.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                colT.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
                colT.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
                colT.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // Colorear celdas de días
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

        // ── EXPORTAR PDF ──────────────────────────────────────────────────────

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Primero genere el reporte antes de exportar.");
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Guardar reporte PDF",
                Filter = "Archivos PDF (*.pdf)|*.pdf",
                FileName = $"Reporte_Docentes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try { ExportarPDF(sfd.FileName); MessageBox.Show($"PDF exportado en:\n{sfd.FileName}"); }
            catch (Exception ex) { MessageBox.Show("Error al exportar PDF: " + ex.Message); }
        }

        private void ExportarPDF(string ruta)
        {
            // iTextSharp.text.Font — calificado explícitamente para evitar CS0104
            var fTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.DARK_GRAY);
            var fSub = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.GRAY);
            var fGen = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY);
            var fHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.WHITE);
            var fCelda = FontFactory.GetFont(FontFactory.HELVETICA, 7, BaseColor.BLACK);
            var fBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7, BaseColor.BLACK);

            string seleccion = comboBox1.SelectedItem?.ToString() ?? "";
            bool todos = seleccion.StartsWith("★");
            string mes = comboBox2.SelectedItem?.ToString() ?? "";
            string rango = comboBox3.SelectedItem?.ToString() ?? "";

            var doc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 15, 15, 25, 20);
            PdfWriter.GetInstance(doc, new FileStream(ruta, FileMode.Create));
            doc.Open();

            // Encabezado
            doc.Add(new Paragraph("Reporte de Asistencia – Docentes", fTitulo));
            if (todos)
                doc.Add(new Paragraph("TODOS LOS DOCENTES", fSub));
            else
                doc.Add(new Paragraph($"Docente: {seleccion}", fSub));

            doc.Add(new Paragraph($"Mes: {mes}  |  {rango}", fSub));
            doc.Add(new Paragraph($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}", fGen));
            doc.Add(Chunk.NEWLINE);

            // Tabla
            int totalCols = dataGridView1.Columns.Count;
            var tabla = new PdfPTable(totalCols) { WidthPercentage = 100 };

            // Anchos proporcionales
            float[] anchos = new float[totalCols];
            for (int i = 0; i < totalCols; i++)
            {
                string nombre = dataGridView1.Columns[i].Name;
                anchos[i] = nombre == "Docente" ? 3.5f
                           : nombre == "Área" ? 2f
                           : nombre == "Total" ? 1.2f
                           : 1f; // días
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

            // Filas
            int totalPresentes = 0, totalAusentes = 0;
            bool alterno = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                BaseColor fondo = alterno
                    ? new BaseColor(248, 250, 252)
                    : new BaseColor(255, 255, 255);
                alterno = !alterno;

                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    var cell = row.Cells[col.Index];
                    string valor = cell.Value?.ToString() ?? "";
                    bool esDia = _columnasDias.Contains(col.Name);

                    BaseColor bg = fondo;
                    iTextSharp.text.Font fuente = fCelda;

                    if (esDia)
                    {
                        if (valor == "✗")
                        {
                            bg = new BaseColor(254, 226, 226);
                            fuente = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9,
                                         new BaseColor(153, 27, 27));
                            totalAusentes++;
                        }
                        else if (!string.IsNullOrEmpty(valor))
                        {
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

        // ── Navegación ────────────────────────────────────────────────────────

        private void button1_Click(object sender, EventArgs e)
        {
            new homeAdmin().Show();
            this.Hide();
        }

        private void generarReporteDocentes_Load(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e) { }

        //Boton para generar reporte en excel
        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}