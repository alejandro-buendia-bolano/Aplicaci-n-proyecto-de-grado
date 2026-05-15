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
                Capturador.EventHandler = this;
                Enrolador = new DPFP.Processing.Enrollment();
                Capturador.StartCapture();
                Estado.Text = "• ESTADO DEL SISTEMA";
                Estado.ForeColor = Color.Green;
            }
            catch
            {
                Estado.Text = "• ESTADO DEL SISTEMA";
                Estado.ForeColor = Color.Red;
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


        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            { MessageBox.Show("Seleccione un grado académico primero."); return; }

            // Limpiar sesión anterior
            _asistenciasRegistradas.Clear();
            _asistenciaActiva = true;

            // Mostrar aviso y botón detener
            Estado.Text = "⏳ASISTENCIA EN PROCESO: Indique a los estudiantes que coloquen el dedo en el sensor.";
            Estado.ForeColor = Color.DarkBlue;
            button2.Enabled = false;
            btnDetener.Visible = true;
            btnDetener.Enabled = true;

            try { Capturador.StartCapture(); }
            catch (Exception ex) { MessageBox.Show("Error al iniciar captura: " + ex.Message); }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnDetener_Click(object sender, EventArgs e)
        {
            _asistenciaActiva = false;
            try { Capturador.StopCapture(); } catch { }

            GuardarAsistenciasEnBD();

            Estado.Text = "Asistencia registrada correctamente.";
            Estado.ForeColor = Color.Green;
            button2.Enabled = true;
            button2.Text = "Asistencia registrada";
            btnDetener.Visible = false;
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
        public void OnFingerGone(object Capture, string ReaderSerialNumber) { }
        public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }
        public void OnReaderConnect(object Capture, string ReaderSerialNumber) { }
        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber) { }
        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback) { }

    }
}
