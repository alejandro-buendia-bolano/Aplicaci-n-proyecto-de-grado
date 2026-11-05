using DPFP;
using DPFP.Capture;
using DPFP.Verification;
using iTextSharp.text.pdf.codec.wmf;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Aplicacion
{
    public partial class AsistenciaEstudiantes : Form, DPFP.Capture.EventHandler
    {
        string connectionString = "Server=LAPTOP-VFH6P020; Database=DB_Asistencia; User Id=sa; Password=ACuptxhJT4!; TrustServerCertificate=True;";

        private DPFP.Capture.Capture Capturador;
        private DPFP.Processing.Enrollment Enrolador;
        private bool _asistenciaActiva = false;

        // Lista de asistencias confirmadas en la sesión actual
        private List<(int idEstudiante, DateTime fecha, TimeSpan hora)> _asistenciasRegistradas
            = new List<(int, DateTime, TimeSpan)>();

        private string _nombreDocente;
        private int _idDocente;
        public AsistenciaEstudiantes(string nombreDocente, int idDocente)
        {
            _nombreDocente = nombreDocente;
            _idDocente = idDocente;
            InitializeComponent();
            InicializarReloj();
            CargarGradosDocente();
            InitHardware();
            btnDetener.Enabled = false;
        }
        private void InicializarReloj()
        {
            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer1.Start();
            ActualizarFechaHora();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            ActualizarFechaHora();
        }
        private void ActualizarFechaHora()
        {
            label4.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try { Capturador?.StopCapture(); } catch { }
            Home home = new Home(_nombreDocente, _idDocente);
            this.Hide();
            home.Show();
        }

        private void InitHardware()
        {
            try
            {
                Capturador = new DPFP.Capture.Capture();

                if (Capturador == null)
                {
                    Estado.Text = "• ESTADO DEL SISTEMA: Lector no detectado";
                    Estado.ForeColor = System.Drawing.Color.Red;
                    Estado.BackColor = System.Drawing.Color.Transparent;
                    return;
                }

                Capturador.EventHandler = this;

                // Sin texto de "esperando" — OnReaderConnect lo pondrá verde de inmediato
                Estado.Text = "• ESTADO DEL SISTEMA";
                Estado.ForeColor = System.Drawing.Color.Gray;
                Estado.BackColor = System.Drawing.Color.Transparent;
            }
            catch
            {
                Estado.Text = "• ESTADO DEL SISTEMA: Error al inicializar";
                Estado.ForeColor = System.Drawing.Color.Red;
                Estado.BackColor = System.Drawing.Color.Transparent;
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (!this.Visible)
            {
                try { Capturador?.StopCapture(); } catch { }
                _asistenciaActiva = false;
            }
        }

        // ── Cargar solo los grados del docente ──────────────────────────────────
        private void CargarGradosDocente()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("-- Seleccione un grado académico --");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT nombre FROM Aula 
                                 WHERE idDocente = @idDocente 
                                 ORDER BY nombre ASC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idDocente", _idDocente);
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

        private bool AsegurarSesionActiva()
        {
            if (_asistenciaActiva) return true;

            if (comboBox1.SelectedIndex <= 0)
            {
                MessageBox.Show("Seleccione un grado académico primero.");
                return false;
            }

            _asistenciasRegistradas.Clear();
            _asistenciaActiva = true;
            btnDetener.Visible = true;
            btnDetener.Enabled = true;
            button2.Enabled = false;

            CargarListaEstudiantes();   // lista con columna "Pendiente"
            return true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (Capturador == null)
            {
                MessageBox.Show("El lector biométrico no está listo.\n" +
                                "Verifique que esté conectado e intente de nuevo.",
                                "Lector no disponible",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!AsegurarSesionActiva()) return;

            MessageBox.Show("Asistencia iniciada.\nLos estudiantes pueden colocar el dedo " +
                            "en el sensor o ingresar su identificación.",
                            "Asistencia en proceso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

            try { Capturador.StartCapture(); }
            catch (Exception ex)
            {
                _asistenciaActiva = false;
                button2.Enabled = true;
                btnDetener.Enabled = false;
                MessageBox.Show("Error al iniciar captura: " + ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnDetener_Click(object sender, EventArgs e)
        {
            
            btnDetener.Enabled = false;
            button2.Enabled = true;
            _asistenciaActiva = false;
            try { Capturador.StopCapture(); } catch { }

            GuardarAsistenciasEnBD();

            Estado.Text = "Asistencia registrada correctamente.";
            Estado.ForeColor = Color.Green;
            button2.Enabled = true;
            button2.Text = "Asistencia registrada";
            btnDetener.Visible = false;
            txtIdManual.Visible = false;
            dgvLista.Visible = false;
            lblContador.Visible = false;
        }

        private void GuardarAsistenciasEnBD()
        {
            if (_asistenciasRegistradas.Count == 0) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction tx = conn.BeginTransaction();
                try
                {
                    foreach (var reg in _asistenciasRegistradas)
                    {
                        string query = @"INSERT INTO Registro (idEstudiante, idDocente, fecha, hora)
                                         VALUES (@idEst, @idDoc, @fecha, @hora)";
                        SqlCommand cmd = new SqlCommand(query, conn, tx);
                        cmd.Parameters.AddWithValue("@idEst", reg.idEstudiante);
                        cmd.Parameters.AddWithValue("@idDoc", _idDocente);
                        cmd.Parameters.AddWithValue("@fecha", reg.fecha.Date);
                        cmd.Parameters.AddWithValue("@hora", reg.hora);
                        cmd.ExecuteNonQuery();
                    }
                    tx.Commit();
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    MessageBox.Show("Error al guardar asistencias: " + ex.Message);
                }
            }
        }

        // ── Procesar captura del SDK ────────────────────────────────────────────
        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            if (!_asistenciaActiva) return;
            VerificarHuella(Sample);
        }

        private void VerificarHuella(DPFP.Sample Sample)
        {
            // Extraer características de verificación
            DPFP.Processing.FeatureExtraction extractor = new DPFP.Processing.FeatureExtraction();
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet features = new DPFP.FeatureSet();
            extractor.CreateFeatureSet(Sample, DPFP.Processing.DataPurpose.Verification, ref feedback, ref features);

            if (feedback != DPFP.Capture.CaptureFeedback.Good)
            {
                MostrarMensaje("Lectura de huella incorrecta, intente nuevamente.", Color.Orange);
                return;
            }

            // Obtener estudiantes del grado seleccionado con su huella
            string grado = "";
            this.Invoke(new Action(() => grado = comboBox1.SelectedItem?.ToString() ?? ""));

            List<(int id, string nombre, byte[] huella)> estudiantes = ObtenerEstudiantesConHuella(grado);

            DPFP.Verification.Verification verificador = new DPFP.Verification.Verification();

            foreach (var est in estudiantes)
            {
                if (est.huella == null) continue;

                try
                {
                    DPFP.Template template = new DPFP.Template();
                    template.DeSerialize(est.huella);

                    DPFP.Verification.Verification.Result resultado = new DPFP.Verification.Verification.Result();
                    verificador.Verify(features, template, ref resultado);

                    if (resultado.Verified)
                    {
                        // Evitar doble registro en la misma sesión
                        if (_asistenciasRegistradas.Exists(r => r.idEstudiante == est.id))
                        {
                            MostrarMensaje($"{est.nombre} ya registró asistencia en esta sesión.", Color.Orange);
                            return;
                        }

                        _asistenciasRegistradas.Add((est.id, DateTime.Now, DateTime.Now.TimeOfDay));
                        MostrarMensaje($"{est.nombre} – Asistencia confirmada.", Color.Green);
                        return;
                    }
                }
                catch { /* huella corrupta, continuar */ }
            }

            MostrarMensaje("Huella no reconocida, intente nuevamente.", Color.Red);
        }

        private List<(int id, string nombre, byte[] huella)> ObtenerEstudiantesConHuella(string grado)
        {
            var lista = new List<(int, string, byte[])>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT E.idEstudiante,
                                        E.primerNombre + ' ' + E.primerApellido AS Nombre,
                                        E.Huella
                                 FROM Estudiante E
                                 INNER JOIN AulaEstudiante AE ON E.idEstudiante = AE.idEstudiante
                                 INNER JOIN Aula A ON AE.idAula = A.idAula
                                 WHERE A.nombre = @grado AND E.Huella IS NOT NULL";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@grado", grado);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add((
                        Convert.ToInt32(dr["idEstudiante"]),
                        dr["Nombre"].ToString(),
                        dr["Huella"] as byte[]
                    ));
                }
            }
            return lista;
        }

        private void MostrarMensaje(string texto, Color color)
        {
            if (this.IsHandleCreated)
                this.Invoke(new Action(() =>
                {
                    Estado.Text = texto;
                    Estado.ForeColor = color;
                }));
        }

        // Métodos vacíos de interfaz
        // ── Lector conectado → label verde ───────────────────────────────────────
        // ── Lector conectado → label verde ───────────────────────────────────────
        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            ActualizarEstado("• ESTADO DEL SISTEMA: Lector conectado ✔",
                             System.Drawing.Color.Green);
        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
            ActualizarEstado("• ESTADO DEL SISTEMA: Lector desconectado ✖",
                             System.Drawing.Color.Red);
        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }
        public void OnFingerGone(object Capture, string ReaderSerialNumber) { }

        // ── Helper hilo-seguro ────────────────────────────────────────────────────
        private void ActualizarEstado(string texto,
                               System.Drawing.Color colorTexto)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => ActualizarEstado(texto, colorTexto)));
                return;
            }

            Estado.Text = texto;
            Estado.ForeColor = colorTexto;
            Estado.BackColor = System.Drawing.Color.Transparent; // sin sombreado
        }
        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback) { }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnMarcarID_Click(object sender, EventArgs e)
        {
            dgvLista.Visible = true;
            lblContador.Visible = true;
            txtIdManual.Visible = true;
            string id = txtIdManual.Text.Trim();

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    MessageBox.Show("Ingrese la identificación del estudiante.");
                    return;
                }

                // Si no hay sesión, la inicia: valida grado y carga la lista
                if (!AsegurarSesionActiva()) return;

                RegistrarAsistenciaPorId(id);
            }
            finally
            {
                // Se limpia SIEMPRE, haya funcionado o no, para ingresar el siguiente
                txtIdManual.Clear();
                txtIdManual.Focus();
            }
        }

        private void RegistrarAsistenciaPorId(string idEstudiante)
        {
            // Verificar que el estudiante pertenece al grado seleccionado
            string nombreGrado = comboBox1.SelectedItem?.ToString() ?? "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 1. Verificar que existe en el grado
                SqlCommand cmdVerif = new SqlCommand(
                    @"SELECT COUNT(1)
              FROM Estudiante E
              INNER JOIN AulaEstudiante AE ON E.idEstudiante = AE.idEstudiante
              INNER JOIN Aula A             ON AE.idAula     = A.idAula
              WHERE E.idEstudiante = @id AND A.nombre = @grado", conn);
                cmdVerif.Parameters.AddWithValue("@id", idEstudiante);
                cmdVerif.Parameters.AddWithValue("@grado", nombreGrado);
                int existe = Convert.ToInt32(cmdVerif.ExecuteScalar());

                if (existe == 0)
                {
                    MessageBox.Show($"El estudiante {idEstudiante} no pertenece al grado {nombreGrado}.");
                    return;
                }

                // 2. Verificar duplicado en el día
                SqlCommand cmdDup = new SqlCommand(
                    @"SELECT COUNT(1) FROM Registro
              WHERE idEstudiante = @id AND fecha = CAST(GETDATE() AS DATE)", conn);
                cmdDup.Parameters.AddWithValue("@id", idEstudiante);
                bool yaRegistrado = Convert.ToInt32(cmdDup.ExecuteScalar()) > 0;

                if (yaRegistrado)
                {
                    MessageBox.Show($"El estudiante {idEstudiante} ya tiene asistencia registrada hoy.");
                    return;
                }

                // 3. Registrar
                SqlCommand cmdIns = new SqlCommand(
                    @"INSERT INTO Registro (idEstudiante, idDocente, fecha, hora)
              VALUES (@idEst, @idDoc,
                      CAST(GETDATE() AS DATE),
                      CAST(GETDATE() AS TIME))", conn);
                cmdIns.Parameters.AddWithValue("@idEst", idEstudiante);
                cmdIns.Parameters.AddWithValue("@idDoc", _idDocente); // campo que ya tienes
                cmdIns.ExecuteNonQuery();
            }

            // 4. Marcar visualmente en la lista
            MarcarEstudiantePresente(idEstudiante.ToString());
        }

        // ── Cargar lista al presionar "Registrar asistencia" ──────────────────────
        private void CargarListaEstudiantes()
        {
            string nombreGrado = comboBox1.SelectedItem?.ToString() ?? "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT E.idEstudiante,
                     E.primerNombre + ' ' + E.primerApellido AS NombreCompleto
              FROM Estudiante E
              INNER JOIN AulaEstudiante AE ON E.idEstudiante = AE.idEstudiante
              INNER JOIN Aula A             ON AE.idAula     = A.idAula
              WHERE A.nombre = @grado
              ORDER BY E.primerApellido, E.primerNombre", conn);
                cmd.Parameters.AddWithValue("@grado", nombreGrado);

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                // Agregar columna de estado
                dt.Columns.Add("Estado", typeof(string));
                foreach (DataRow row in dt.Rows)
                    row["Estado"] = "Pendiente";

                dgvLista.DataSource = dt;
                EstilizarListaAsistencia();
            }
        }

        private void EstilizarListaAsistencia()
        {
            dgvLista.AllowUserToAddRows = false;
            dgvLista.ReadOnly = true;
            dgvLista.RowHeadersVisible = false;
            dgvLista.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLista.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLista.EnableHeadersVisualStyles = false;
            dgvLista.RowTemplate.Height = 34;

            dgvLista.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            dgvLista.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvLista.ColumnHeadersDefaultCellStyle.Font =
                new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            if (dgvLista.Columns["idEstudiante"] != null)
                dgvLista.Columns["idEstudiante"].Visible = false; // ocultar ID, solo para lógica interna

            if (dgvLista.Columns["NombreCompleto"] != null)
                dgvLista.Columns["NombreCompleto"].HeaderText = "Estudiante";

            if (dgvLista.Columns["Estado"] != null)
            {
                dgvLista.Columns["Estado"].HeaderText = "Estado";
                dgvLista.Columns["Estado"].FillWeight = 60;
            }
        }

        // ── Cambiar color de la fila cuando el estudiante marca ───────────────────
        private void MarcarEstudiantePresente(string idBuscado)
        {
            foreach (DataGridViewRow row in dgvLista.Rows)
            {
                if (row.Cells["idEstudiante"].Value?.ToString()?.Trim() == idBuscado.Trim())
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(220, 252, 231);
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(22, 101, 52);
                    row.Cells["Estado"].Value = "✔ Presente";
                    dgvLista.FirstDisplayedScrollingRowIndex = Math.Max(0, row.Index - 3); // lo deja a la vista
                    break;
                }
            }

            int presentes = dgvLista.Rows.Cast<DataGridViewRow>()
                .Count(r => r.Cells["Estado"].Value?.ToString().StartsWith("✔") == true);
            lblContador.Text = $"Presentes: {presentes} / {dgvLista.Rows.Count}";
        }

        // ── Integrar con button2_Click: cargar lista al iniciar ───────────────────

        private void txtIdManual_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            e.Handled = true;
            e.SuppressKeyPress = true;   // evita el "beep" de Windows
            btnMarcarID_Click(sender, EventArgs.Empty);
        }

        private void txtIdManual_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
