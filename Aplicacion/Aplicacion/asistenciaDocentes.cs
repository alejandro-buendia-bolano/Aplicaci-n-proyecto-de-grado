using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using DPFP;
using DPFP.Capture;
using DPFP.Verification;
namespace Aplicacion
{

    public partial class asistenciaDocentes : Form, DPFP.Capture.EventHandler
    {
        private string _nombreDocente;
        private int _idDocente;
        string connectionString = "Server=LAPTOP-VFH6P020; Database=DB_Asistencia; User Id=sa; Password=ACuptxhJT4!; TrustServerCertificate=True;";

        private DPFP.Capture.Capture Capturador;
        private bool _esperandoHuella = false;
        private DPFP.Processing.Enrollment Enrolador;
        private List<(int idEstudiante, DateTime fecha, TimeSpan hora)> _asistenciasRegistradas
           = new List<(int, DateTime, TimeSpan)>();

        public asistenciaDocentes(string nombreDocente, int idDocente)
        {
            _nombreDocente = nombreDocente;
            _idDocente = idDocente;
            InitializeComponent();
            InicializarReloj();
            InitHardware();
        }

        private void InitHardware()
        {
            try
            {
                Capturador = new DPFP.Capture.Capture();
                Capturador.EventHandler = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inicializar el lector: " + ex.Message);
            }
        }


        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (!this.Visible)
            {
                try { Capturador?.StopCapture(); } catch { }
                _esperandoHuella = false;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try { Capturador?.StopCapture(); } catch { }
            Home home = new Home(_nombreDocente, _idDocente);
            this.Hide();
            home.Show();
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
            label3.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _esperandoHuella = true;
            button3.Enabled = false;
            labelEstado.Text = "👆 Coloque su dedo en el sensor para marcar la asistencia.";
            labelEstado.ForeColor = Color.DarkBlue;

            try
            {
                Capturador.StartCapture();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error al iniciar captura: " + ex.Message); 
            }
  

        }

        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            if (!_esperandoHuella) return;
            VerificarYRegistrar(Sample);
        }

        private void VerificarYRegistrar(DPFP.Sample Sample)
        {
            // Extraer características
            DPFP.Processing.FeatureExtraction extractor = new DPFP.Processing.FeatureExtraction();
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet features = new DPFP.FeatureSet();
            extractor.CreateFeatureSet(Sample, DPFP.Processing.DataPurpose.Verification, ref feedback, ref features);

            if (feedback != DPFP.Capture.CaptureFeedback.Good)
            {
                MostrarMensaje("Lectura incorrecta, intente de nuevo.", Color.Orange);
                return;
            }

            // Obtener huella del docente desde BD
            byte[] huellaGuardada = ObtenerHuellaDocente(_idDocente);

            if (huellaGuardada == null)
            {
                MostrarMensaje("❌ No tiene huella registrada en el sistema.", Color.Red);
                _esperandoHuella = false;
                try { Capturador.StopCapture(); } catch { }
                this.Invoke(new Action(() => button3.Enabled = true));
                return;
            }

            try
            {
                DPFP.Template template = new DPFP.Template();
                template.DeSerialize(huellaGuardada);

                DPFP.Verification.Verification verificador = new DPFP.Verification.Verification();
                DPFP.Verification.Verification.Result resultado = new DPFP.Verification.Verification.Result();
                verificador.Verify(features, template, ref resultado);

                if (resultado.Verified)
                {
                    _esperandoHuella = false;
                    try { Capturador.StopCapture(); } catch { }
                    GuardarRegistroDocente();
                    MostrarMensaje("✅ Asistencia confirmada.", Color.Green);

                    this.Invoke(new Action(() =>
                    {
                        button3.Enabled = true;
                        button3.Text = "Asistencia registrada";
                    }));
                }
                else
                {
                    MostrarMensaje("❌ Huella no reconocida, intente nuevamente.", Color.Red);
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al verificar: " + ex.Message, Color.Red);
            }
        }

        private byte[] ObtenerHuellaDocente(int idDoc)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT huella FROM Docente WHERE idDocente = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idDoc);
                conn.Open();
                object resultado = cmd.ExecuteScalar();
                return (resultado != null && resultado != DBNull.Value) ? (byte[])resultado : null;
            }
        }

        private void GuardarRegistroDocente()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Registro (idDocente, fecha, hora)
                                     VALUES (@idDoc, @fecha, @hora)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idDoc", _idDocente);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now.Date);
                    cmd.Parameters.AddWithValue("@hora", DateTime.Now.TimeOfDay);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al guardar registro: " + ex.Message, Color.Red);
            }
        }

        private void MostrarMensaje(string texto, Color color)
        {
            if (this.IsHandleCreated)
                this.Invoke(new Action(() =>
                {
                    labelEstado.Text = texto;
                    labelEstado.ForeColor = color;
                }));
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber) { }
        public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }
        public void OnReaderConnect(object Capture, string ReaderSerialNumber) { }
        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber) { }
        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback) { }

    }
}
