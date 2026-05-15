using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using DPFP;
using DPFP.Capture;
using DPFP.Processing;

namespace Aplicacion
{
    public partial class administrarEstudiantes : Form, DPFP.Capture.EventHandler
    {
        string connectionString = "Server=LAPTOP-VFH6P020; Database=DB_Asistencia; User Id=sa; Password=ACuptxhJT4!; TrustServerCertificate=True;";

        private DPFP.Capture.Capture Capturador;
        private DPFP.Processing.Enrollment Enrolador;
        private DPFP.Template Plantilla;

        private readonly Color COLOR_HUELLA_REGISTRADA = Color.FromArgb(52, 152, 219);
        private readonly Color COLOR_HUELLA_PENDIENTE = Color.FromArgb(52, 73, 94);
        private bool _actualizandoCheck = false;

        public administrarEstudiantes()
        {
            InitializeComponent();
            grados.CheckOnClick = true;
            grados.ItemCheck += Grados_ItemCheck;
            dataGridView1.CellClick += dataGridView1_CellClick;
            ActualizarTabla();
            CargarGrados();
            InitHardware();
        }

        // ── Hardware ────────────────────────────────────────────────────────────
        protected virtual void InitHardware()
        {
            try
            {
                Capturador = new DPFP.Capture.Capture();
                Capturador.EventHandler = this;
                Enrolador = new DPFP.Processing.Enrollment();
                Capturador.StartCapture();
                estado.Text = "• ESTADO DEL SISTEMA";
                estado.ForeColor = Color.Green;
            }
            catch
            {
                estado.Text = "• ESTADO DEL SISTEMA";
                estado.ForeColor = Color.Red;
            }
        }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            if (this.IsHandleCreated)
                this.Invoke(new Action(() =>
                {
                    estado.Text = "• ESTADO DEL SISTEMA";
                    estado.ForeColor = Color.Green;
                }));
        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
            if (this.IsHandleCreated)
                this.Invoke(new Action(() =>
                {
                    estado.Text = "• ESTADO DEL SISTEMA";
                    estado.ForeColor = Color.Red;
                }));
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible)
            {
                try { Capturador?.StopCapture(); } catch { }
                InitHardware();
            }
        }

        // ── Tabla ───────────────────────────────────────────────────────────────
        private void ActualizarTabla()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT idEstudiante, primerNombre, segundoNombre, 
                                        primerApellido, segundoApellido 
                                 FROM Estudiante";
                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        // ── Cargar grados ───────────────────────────────────────────────────────
        private void CargarGrados()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT nombre FROM Aula ORDER BY nombre ASC";
                SqlCommand cmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    grados.Items.Clear();
                    grados.Items.Add("-- TODOS LOS GRADOS --");
                    while (dr.Read())
                        grados.Items.Add(dr["nombre"].ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar grados: " + ex.Message);
                }
            }
        }

        // ── Color botón huella ──────────────────────────────────────────────────
        private void ActualizarColorBotonHuella(string idEst)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT Huella FROM Estudiante WHERE idEstudiante = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", idEst);
                    conn.Open();
                    object resultado = cmd.ExecuteScalar();
                    bool tieneHuella = resultado != null && resultado != DBNull.Value;

                    registraHuella.BackColor = tieneHuella ? COLOR_HUELLA_REGISTRADA : COLOR_HUELLA_PENDIENTE;
                    registraHuella.Text = tieneHuella ? "HUELLA REGISTRADA" : "REGISTRAR HUELLA";
                }
            }
            catch
            {
                registraHuella.BackColor = COLOR_HUELLA_PENDIENTE;
                registraHuella.Text = "REGISTRAR HUELLA";
            }
        }

        // ── Limpiar formulario ──────────────────────────────────────────────────
        private void LimpiarFormulario()
        {
            idEstudiante.Text = "";
            primerNombre.Text = "";
            segundoNombre.Text = "";
            primerApellido.Text = "";
            segundoApellido.Text = "";
            idEstudiante.ReadOnly = false;

            for (int i = 0; i < grados.Items.Count; i++)
                grados.SetItemChecked(i, false);

            registraHuella.BackColor = COLOR_HUELLA_PENDIENTE;
            registraHuella.Text = "REGISTRAR HUELLA";
        }

        // ── AGREGAR ─────────────────────────────────────────────────────────────
        private void agregar_Click(object sender, EventArgs e)
        {

        }

        // ── ELIMINAR ────────────────────────────────────────────────────────────
        private void eliminar_Click(object sender, EventArgs e)
        {

        }

        // ── ACTUALIZAR ──────────────────────────────────────────────────────────
        private void actualizar_Click(object sender, EventArgs e)
        {

        }

        // ── CLIC EN FILA ────────────────────────────────────────────────────────
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];
            string idEst = fila.Cells["idEstudiante"].Value.ToString();

            idEstudiante.Text = idEst;
            primerNombre.Text = fila.Cells["primerNombre"].Value?.ToString() ?? "";
            segundoNombre.Text = fila.Cells["segundoNombre"].Value?.ToString() ?? "";
            primerApellido.Text = fila.Cells["primerApellido"].Value?.ToString() ?? "";
            segundoApellido.Text = fila.Cells["segundoApellido"].Value?.ToString() ?? "";

            idEstudiante.ReadOnly = true;
            ActualizarColorBotonHuella(idEst);
        }

        // ── CheckedListBox ──────────────────────────────────────────────────────
        private void Grados_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_actualizandoCheck) return;

            this.BeginInvoke(new Action(() =>
            {
                _actualizandoCheck = true;

                if (e.Index == 0)
                {
                    bool marcar = grados.GetItemChecked(0);
                    for (int i = 1; i < grados.Items.Count; i++)
                        grados.SetItemChecked(i, marcar);
                }
                else
                {
                    if (!grados.GetItemChecked(e.Index))
                        grados.SetItemChecked(0, false);

                    bool todosMarcados = Enumerable.Range(1, grados.Items.Count - 1)
                                                   .All(i => grados.GetItemChecked(i));
                    if (todosMarcados) grados.SetItemChecked(0, true);
                }

                _actualizandoCheck = false;

                List<string> seleccionados = grados.CheckedItems.Cast<string>()
                                                    .Where(g => g != "-- TODOS LOS GRADOS --")
                                                    .ToList();
                if (seleccionados.Count == 0) { ActualizarTabla(); return; }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string filtro = string.Join("','", seleccionados);
                    string query = $@"SELECT DISTINCT E.idEstudiante, E.primerNombre, E.segundoNombre,
                                             E.primerApellido, E.segundoApellido
                              FROM Estudiante E
                              INNER JOIN AulaEstudiante AE ON E.idEstudiante = AE.idEstudiante
                              INNER JOIN Aula A ON AE.idAula = A.idAula
                              WHERE A.nombre IN ('{filtro}')";
                    SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }));
        }

        // ── VER REGISTROS ───────────────────────────────────────────────────────
        private void verRegistros_Click(object sender, EventArgs e)
        {

        }

        // ── REGISTRAR HUELLA ────────────────────────────────────────────────────
        private void registraHuella_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idEstudiante.Text))
            {
                MessageBox.Show("Primero seleccione un estudiante de la tabla.");
                return;
            }
            Enrolador = new DPFP.Processing.Enrollment();
            MessageBox.Show($"Iniciando registro para el estudiante {idEstudiante.Text}. Coloque el dedo 4 veces en el lector.");
        }

        protected void ProcesarCaptura(DPFP.Sample Sample)
        {
            DPFP.FeatureSet caracteristicas = ExtraerCaracteristicas(Sample, DPFP.Processing.DataPurpose.Enrollment);
            if (caracteristicas == null) return;

            Enrolador.AddFeatures(caracteristicas);
            this.Invoke(new Action(() =>
                Console.WriteLine("Muestras restantes: " + Enrolador.FeaturesNeeded)));

            if (Enrolador.TemplateStatus == DPFP.Processing.Enrollment.Status.Ready)
            {
                Plantilla = Enrolador.Template;
                GuardarHuellaEnBD(Plantilla);
            }
        }

        private void GuardarHuellaEnBD(DPFP.Template plantilla)
        {
            byte[] huellaBytes = plantilla.Bytes;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Estudiante SET Huella = @huella WHERE idEstudiante = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@huella", SqlDbType.VarBinary).Value = huellaBytes;
                    cmd.Parameters.AddWithValue("@id", idEstudiante.Text);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show("¡Huella dactilar guardada exitosamente!");
                        ActualizarColorBotonHuella(idEstudiante.Text);
                    }));
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() => MessageBox.Show("Error al guardar en BD: " + ex.Message)));
            }
        }

        protected DPFP.FeatureSet ExtraerCaracteristicas(DPFP.Sample Sample, DPFP.Processing.DataPurpose Objetivo)
        {
            DPFP.Processing.FeatureExtraction extractor = new DPFP.Processing.FeatureExtraction();
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet caracteristicas = new DPFP.FeatureSet();
            extractor.CreateFeatureSet(Sample, Objetivo, ref feedback, ref caracteristicas);
            return (feedback == DPFP.Capture.CaptureFeedback.Good) ? caracteristicas : null;
        }

        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
            => ProcesarCaptura(Sample);

        public void OnFingerGone(object Capture, string ReaderSerialNumber) { }
        public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }
        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback) { }

        private void ConfigurarEstiloDgv()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            if (dataGridView1.Columns["idEstudiante"] != null) dataGridView1.Columns["idEstudiante"].HeaderText = "Identificación";
            if (dataGridView1.Columns["primerNombre"] != null) dataGridView1.Columns["primerNombre"].HeaderText = "Primer Nombre";
            if (dataGridView1.Columns["segundoNombre"] != null) dataGridView1.Columns["segundoNombre"].HeaderText = "Segundo Nombre";
            if (dataGridView1.Columns["primerApellido"] != null) dataGridView1.Columns["primerApellido"].HeaderText = "Primer Apellido";
            if (dataGridView1.Columns["segundoApellido"] != null) dataGridView1.Columns["segundoApellido"].HeaderText = "Segundo Apellido";
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.None;
        }

        // ── Navegación ──────────────────────────────────────────────────────────
        private void button1_Click(object sender, EventArgs e)
        {
            try { Capturador?.StopCapture(); } catch { }
            homeAdmin admin = new homeAdmin();
            this.Hide();
            admin.Show();
        }

        private void eliminar_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            string id = dataGridView1.CurrentRow.Cells["idEstudiante"].Value.ToString();

            DialogResult confirm = MessageBox.Show(
                "¿Está seguro que desea eliminar este estudiante?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    // 1. Eliminar registros de asistencia
                    string qReg = "DELETE FROM Registro WHERE idEstudiante = @id";
                    SqlCommand cmdReg = new SqlCommand(qReg, conn, transaction);
                    cmdReg.Parameters.AddWithValue("@id", id);
                    cmdReg.ExecuteNonQuery();

                    // 2. Eliminar de AulaEstudiante
                    string qAulaEst = "DELETE FROM AulaEstudiante WHERE idEstudiante = @id";
                    SqlCommand cmdAE = new SqlCommand(qAulaEst, conn, transaction);
                    cmdAE.Parameters.AddWithValue("@id", id);
                    cmdAE.ExecuteNonQuery();

                    // 3. Eliminar el estudiante
                    string qEst = "DELETE FROM Estudiante WHERE idEstudiante = @id";
                    SqlCommand cmdEst = new SqlCommand(qEst, conn, transaction);
                    cmdEst.Parameters.AddWithValue("@id", id);
                    cmdEst.ExecuteNonQuery();

                    transaction.Commit();
                    MessageBox.Show("Estudiante eliminado correctamente.");
                    ActualizarTabla();
                    LimpiarFormulario();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            }
        }

        private void registraHuella_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idEstudiante.Text))
            {
                MessageBox.Show("Primero seleccione un estudiante de la tabla.");
                return;
            }
            Enrolador = new DPFP.Processing.Enrollment();
            MessageBox.Show($"Iniciando registro para el estudiante {idEstudiante.Text}. Coloque el dedo 4 veces en el lector.");
        }

        private void agregar_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idEstudiante.Text) || grados.CheckedItems.Count == 0)
            {
                MessageBox.Show("Debe ingresar la identificación y seleccionar al menos un grado.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaccion = conn.BeginTransaction();
                try
                {
                    // 1. Insertar estudiante
                    string queryEst = @"INSERT INTO Estudiante 
                                        (idEstudiante, primerNombre, segundoNombre, primerApellido, segundoApellido) 
                                        VALUES (@id, @pN, @sN, @pA, @sA)";
                    SqlCommand cmdEst = new SqlCommand(queryEst, conn, transaccion);
                    cmdEst.Parameters.AddWithValue("@id", idEstudiante.Text);
                    cmdEst.Parameters.AddWithValue("@pN", primerNombre.Text);
                    cmdEst.Parameters.AddWithValue("@sN", string.IsNullOrEmpty(segundoNombre.Text) ? (object)DBNull.Value : segundoNombre.Text);
                    cmdEst.Parameters.AddWithValue("@pA", primerApellido.Text);
                    cmdEst.Parameters.AddWithValue("@sA", segundoApellido.Text);
                    cmdEst.ExecuteNonQuery();

                    // 2. Asociar a las aulas seleccionadas en AulaEstudiante
                    foreach (var item in grados.CheckedItems)
                    {
                        string nombreGrado = item.ToString();
                        if (nombreGrado == "-- TODOS LOS GRADOS --") continue;

                        // Obtener idAula por nombre
                        string queryIdAula = "SELECT idAula FROM Aula WHERE nombre = @nombre";
                        SqlCommand cmdIdAula = new SqlCommand(queryIdAula, conn, transaccion);
                        cmdIdAula.Parameters.AddWithValue("@nombre", nombreGrado);
                        object idAulaObj = cmdIdAula.ExecuteScalar();

                        if (idAulaObj != null)
                        {
                            string queryAulaEst = @"INSERT INTO AulaEstudiante (idAula, idEstudiante) 
                                                    VALUES (@idAula, @idEst)";
                            SqlCommand cmdAulaEst = new SqlCommand(queryAulaEst, conn, transaccion);
                            cmdAulaEst.Parameters.AddWithValue("@idAula", Convert.ToInt32(idAulaObj));
                            cmdAulaEst.Parameters.AddWithValue("@idEst", idEstudiante.Text);
                            cmdAulaEst.ExecuteNonQuery();
                        }
                    }

                    transaccion.Commit();
                    MessageBox.Show("Estudiante agregado y aula asignada correctamente.");
                    ActualizarTabla();
                    LimpiarFormulario();
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    MessageBox.Show("Error al agregar: " + ex.Message);
                }
            }
        }

        private void actualizar_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idEstudiante.Text))
            {
                MessageBox.Show("Seleccione un estudiante de la tabla primero.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Estudiante 
                                 SET primerNombre    = @n1,
                                     segundoNombre   = @n2,
                                     primerApellido  = @ap1,
                                     segundoApellido = @ap2
                                 WHERE idEstudiante = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idEstudiante.Text);
                cmd.Parameters.AddWithValue("@n1", primerNombre.Text);
                cmd.Parameters.AddWithValue("@n2", segundoNombre.Text);
                cmd.Parameters.AddWithValue("@ap1", primerApellido.Text);
                cmd.Parameters.AddWithValue("@ap2", segundoApellido.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
                ActualizarTabla();
                MessageBox.Show("Registro actualizado.");
                LimpiarFormulario();
            }
        }

        private void verRegistros_Click_1(object sender, EventArgs e)
        {
            List<string> seleccionados = grados.CheckedItems.Cast<string>()
                                                .Where(g => g != "-- TODOS LOS GRADOS --")
                                                .ToList();
            if (seleccionados.Count == 0) { ActualizarTabla(); return; }

            string filtroSQL = string.Join("','", seleccionados);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = $@"SELECT DISTINCT E.idEstudiante, E.primerNombre, E.segundoNombre,
                                         E.primerApellido, E.segundoApellido
                          FROM Estudiante E
                          INNER JOIN AulaEstudiante AE ON E.idEstudiante = AE.idEstudiante
                          INNER JOIN Aula A ON AE.idAula = A.idAula
                          WHERE A.nombre IN ('{filtroSQL}')";
                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                dataGridView1.DataSource = dt;
                ConfigurarEstiloDgv();
            }
        }
    }
}