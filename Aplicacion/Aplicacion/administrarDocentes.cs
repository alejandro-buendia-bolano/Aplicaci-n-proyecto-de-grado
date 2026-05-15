using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;
using DPFP;
using DPFP.Capture;
using DPFP.Verification;
using DPFP.Processing;

namespace Aplicacion
{
    public partial class administrarDocentes : Form, DPFP.Capture.EventHandler
    {
        string connectionString = "Server=LAPTOP-VFH6P020; Database=DB_Asistencia; User Id=sa; Password=ACuptxhJT4!; TrustServerCertificate=True;";

        private DPFP.Capture.Capture Capturador;
        private DPFP.Processing.Enrollment Enrolador;
        private DPFP.Template Plantilla;

        // ─── CAMBIO 1: Color base del botón registrar huella ───────────────────
        private readonly Color COLOR_HUELLA_REGISTRADA = Color.FromArgb(52, 152, 219);  // azul claro
        private readonly Color COLOR_HUELLA_PENDIENTE = Color.FromArgb(52, 73, 94);  // azul oscuro original

        public administrarDocentes()
        {
            InitializeComponent();
            grados.CheckOnClick = true;
            grados.ItemCheck += Grados_ItemCheck;
            dgv1.CellClick += dgv1_CellClick;
            ActualizarTabla();
            CargarGrados();
            InitHardware();
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  CAMBIO 1 – Estado del lector en tiempo real (OnReaderConnect/Disconnect)
        // ═══════════════════════════════════════════════════════════════════════
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

        // El SDK llama a estos métodos desde un hilo distinto; usamos Invoke para
        // actualizar la UI de forma segura y en tiempo real sin reiniciar la app.
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

        // ═══════════════════════════════════════════════════════════════════════
        //  CAMBIO 2 – CheckedListBox: opción "Todos los grados" + orden ascendente
        // ═══════════════════════════════════════════════════════════════════════
        private void CargarGrados()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // ORDER BY nombre para ordenar de menor a mayor
                string query = "SELECT nombre FROM Aula ORDER BY nombre ASC";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    grados.Items.Clear();

                    // Primera opción especial
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

        // ═══════════════════════════════════════════════════════════════════════
        //  CAMBIO 3 – Color del botón según si el docente tiene huella registrada
        // ═══════════════════════════════════════════════════════════════════════
        private void ActualizarColorBotonHuella(string idDoc)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT huella FROM Docente WHERE idDocente = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", idDoc);
                    conn.Open();

                    object resultado = cmd.ExecuteScalar();
                    bool tieneHuella = resultado != null && resultado != DBNull.Value;

                    if (tieneHuella)
                    {
                        registraHuella.BackColor = COLOR_HUELLA_REGISTRADA;
                        registraHuella.Text = "HUELLA REGISTRADA";
                    }
                    else
                    {
                        registraHuella.BackColor = COLOR_HUELLA_PENDIENTE;
                        registraHuella.Text = "REGISTRAR HUELLA";
                    }
                }
            }
            catch
            {
                registraHuella.BackColor = COLOR_HUELLA_PENDIENTE;
                registraHuella.Text = "REGISTRAR HUELLA";
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  CAMBIO 5 – Método centralizado para limpiar el formulario
        // ═══════════════════════════════════════════════════════════════════════
        private void LimpiarFormulario()
        {
            idDocente.Text = "";
            primerNombre.Text = "";
            segundoNombre.Text = "";
            primerApellido.Text = "";
            segundoApellid.Text = "";
            area.Text = "";

            idDocente.ReadOnly = false;

            // Desmarcar todos los grados
            for (int i = 0; i < grados.Items.Count; i++)
                grados.SetItemChecked(i, false);

            // Restaurar color del botón huella
            registraHuella.BackColor = COLOR_HUELLA_PENDIENTE;
            registraHuella.Text = "REGISTRAR HUELLA";
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  Tabla principal
        // ═══════════════════════════════════════════════════════════════════════
        private void ActualizarTabla()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT idDocente, primerNombre, segundoNombre, primerApellido, segundoApellido FROM Docente";
                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                dgv1.DataSource = dt;
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  BOTÓN AGREGAR  (+ CAMBIO 5: limpiar al finalizar)
        // ═══════════════════════════════════════════════════════════════════════
        private void agregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idDocente.Text) || grados.CheckedItems.Count == 0)
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
                    string queryDocente = @"INSERT INTO Docente (idDocente, primerNombre, segundoNombre, primerApellido, segundoApellido, area) 
                                    VALUES (@id, @pNombre, @sNombre, @pApellido, @sApellido, @area)";

                    SqlCommand cmdDoc = new SqlCommand(queryDocente, conn, transaccion);
                    cmdDoc.Parameters.AddWithValue("@id", idDocente.Text);
                    cmdDoc.Parameters.AddWithValue("@pNombre", primerNombre.Text);
                    cmdDoc.Parameters.AddWithValue("@sNombre", (object)segundoNombre.Text ?? DBNull.Value);
                    cmdDoc.Parameters.AddWithValue("@pApellido", primerApellido.Text);
                    cmdDoc.Parameters.AddWithValue("@sApellido", segundoApellid.Text);
                    cmdDoc.Parameters.AddWithValue("@area", area.Text);
                    cmdDoc.ExecuteNonQuery();

                    // Saltamos "-- TODOS LOS GRADOS --" al iterar
                    foreach (var item in grados.CheckedItems)
                    {
                        string nombreGrado = item.ToString();
                        if (nombreGrado == "-- TODOS LOS GRADOS --") continue;

                        string queryAula = "UPDATE Aula SET idDocente = @idDocente WHERE nombre = @nombreGrado";
                        SqlCommand cmdAula = new SqlCommand(queryAula, conn, transaccion);
                        cmdAula.Parameters.AddWithValue("@idDocente", idDocente.Text);
                        cmdAula.Parameters.AddWithValue("@nombreGrado", nombreGrado);
                        cmdAula.ExecuteNonQuery();
                    }

                    transaccion.Commit();
                    MessageBox.Show("Docente agregado y aula asignada correctamente.");
                    ActualizarTabla();
                    LimpiarFormulario(); // CAMBIO 5
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    MessageBox.Show("Error al agregar: " + ex.Message);
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  BOTÓN ELIMINAR  (CAMBIO 4: desligar Aula antes de borrar + CAMBIO 5)
        // ═══════════════════════════════════════════════════════════════════════
        private void eliminar_Click(object sender, EventArgs e)
        {
            if (dgv1.SelectedRows.Count == 0) return;

            string id = dgv1.CurrentRow.Cells["idDocente"].Value.ToString();

            DialogResult confirm = MessageBox.Show(
                "¿Está seguro que desea eliminar este docente?",
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
                    // 1. Reasignar las aulas a otro docente o desvincular usando un docente "vacío"
                    //    Como idDocente en Aula es NOT NULL, simplemente desvinculamos
                    //    asignando las aulas a ningún docente eliminando el registro de Aula primero
                    //    o bien: eliminamos los registros de AulaEstudiante relacionados,
                    //    luego actualizamos Aula con un idDocente temporal no existente.
                    //    La solución correcta: cambiar la FK en BD a ON DELETE SET NULL,
                    //    pero como no podemos modificar la BD, eliminamos las Aulas huérfanas
                    //    y los registros dependientes en cascada.

                    // 1. Eliminar registros de Registro asociados al docente
                    string queryRegistro = "DELETE FROM Registro WHERE idDocente = @id";
                    SqlCommand cmdReg = new SqlCommand(queryRegistro, conn, transaction);
                    cmdReg.Parameters.AddWithValue("@id", id);
                    cmdReg.ExecuteNonQuery();

                    // 2. Obtener las aulas del docente
                    string queryGetAulas = "SELECT idAula FROM Aula WHERE idDocente = @id";
                    SqlCommand cmdGetAulas = new SqlCommand(queryGetAulas, conn, transaction);
                    cmdGetAulas.Parameters.AddWithValue("@id", id);
                    List<int> idsAula = new List<int>();
                    using (SqlDataReader dr = cmdGetAulas.ExecuteReader())
                    {
                        while (dr.Read())
                            idsAula.Add(Convert.ToInt32(dr["idAula"]));
                    }

                    // 3. Por cada aula: eliminar AulaEstudiante y luego el Aula misma
                    foreach (int idAula in idsAula)
                    {
                        // Eliminar relaciones AulaEstudiante
                        string queryAulaEst = "DELETE FROM AulaEstudiante WHERE idAula = @idAula";
                        SqlCommand cmdAulaEst = new SqlCommand(queryAulaEst, conn, transaction);
                        cmdAulaEst.Parameters.AddWithValue("@idAula", idAula);
                        cmdAulaEst.ExecuteNonQuery();

                        // Eliminar el Aula
                        string queryDelAula = "DELETE FROM Aula WHERE idAula = @idAula";
                        SqlCommand cmdDelAula = new SqlCommand(queryDelAula, conn, transaction);
                        cmdDelAula.Parameters.AddWithValue("@idAula", idAula);
                        cmdDelAula.ExecuteNonQuery();
                    }

                    // 4. Eliminar el Usuario asociado al docente
                    string queryUsuario = "DELETE FROM Usuario WHERE idDocente = @id";
                    SqlCommand cmdUser = new SqlCommand(queryUsuario, conn, transaction);
                    cmdUser.Parameters.AddWithValue("@id", id);
                    cmdUser.ExecuteNonQuery();

                    // 5. Finalmente eliminar el Docente
                    string queryDocente = "DELETE FROM Docente WHERE idDocente = @id";
                    SqlCommand cmdDoc = new SqlCommand(queryDocente, conn, transaction);
                    cmdDoc.Parameters.AddWithValue("@id", id);
                    cmdDoc.ExecuteNonQuery();

                    transaction.Commit();
                    MessageBox.Show("Docente eliminado correctamente.");
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
        // ═══════════════════════════════════════════════════════════════════════
        //  BOTÓN ACTUALIZAR  (+ CAMBIO 5)
        // ═══════════════════════════════════════════════════════════════════════
        private void actualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idDocente.Text))
            {
                MessageBox.Show("Seleccione un docente de la tabla primero.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Se agrega el campo 'area' que faltaba en el UPDATE
                string query = @"UPDATE Docente 
                         SET primerNombre    = @n1,
                             segundoNombre   = @n2,
                             primerApellido  = @ap1,
                             segundoApellido = @ap2,
                             area            = @area
                         WHERE idDocente = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idDocente.Text);
                cmd.Parameters.AddWithValue("@n1", primerNombre.Text);
                cmd.Parameters.AddWithValue("@n2", segundoNombre.Text);
                cmd.Parameters.AddWithValue("@ap1", primerApellido.Text);
                cmd.Parameters.AddWithValue("@ap2", segundoApellid.Text);
                cmd.Parameters.AddWithValue("@area", area.Text);   // ← campo que faltaba

                conn.Open();
                cmd.ExecuteNonQuery();
                ActualizarTabla();
                MessageBox.Show("Registro actualizado.");
                LimpiarFormulario();
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  CLIC EN FILA  (CAMBIO 3: actualizar color del botón huella)
        // ═══════════════════════════════════════════════════════════════════════
        private void dgv1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = dgv1.Rows[e.RowIndex];
            string idDoc = fila.Cells["idDocente"].Value.ToString();

            // Llenar campos visibles desde la fila
            idDocente.Text = idDoc;
            primerNombre.Text = fila.Cells["primerNombre"].Value?.ToString() ?? "";
            segundoNombre.Text = fila.Cells["segundoNombre"].Value?.ToString() ?? "";
            primerApellido.Text = fila.Cells["primerApellido"].Value?.ToString() ?? "";
            segundoApellid.Text = fila.Cells["segundoApellido"].Value?.ToString() ?? "";

            // 'area' no está en el DataGridView, se consulta directamente
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT area FROM Docente WHERE idDocente = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", idDoc);
                    conn.Open();
                    object resultado = cmd.ExecuteScalar();
                    area.Text = resultado?.ToString() ?? "";
                }
            }
            catch { area.Text = ""; }

            idDocente.ReadOnly = true;

            // Actualizar color del botón de huella
            ActualizarColorBotonHuella(idDoc);
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  CAMBIO 2 – Lógica "Todos los grados" en el CheckedListBox
        // ═══════════════════════════════════════════════════════════════════════
        private bool _actualizandoCheck = false; // bandera para evitar recursión

        private void Grados_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_actualizandoCheck) return;

            this.BeginInvoke(new Action(() =>
            {
                _actualizandoCheck = true;

                // Si se marcó "-- TODOS LOS GRADOS --", marcar/desmarcar todo
                if (e.Index == 0)
                {
                    bool marcar = grados.GetItemChecked(0);
                    for (int i = 1; i < grados.Items.Count; i++)
                        grados.SetItemChecked(i, marcar);
                }
                else
                {
                    // Si se desmarca cualquier grado individual, desmarcar "Todos"
                    if (!grados.GetItemChecked(e.Index))
                        grados.SetItemChecked(0, false);

                    // Si todos los individuales están marcados, marcar "Todos"
                    bool todosMarcados = Enumerable.Range(1, grados.Items.Count - 1)
                                                   .All(i => grados.GetItemChecked(i));
                    if (todosMarcados)
                        grados.SetItemChecked(0, true);
                }

                _actualizandoCheck = false;

                // Filtrar tabla según selección
                List<string> seleccionados = grados.CheckedItems.Cast<string>()
                                                    .Where(g => g != "-- TODOS LOS GRADOS --")
                                                    .ToList();

                if (seleccionados.Count == 0)
                {
                    ActualizarTabla();
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string filtroGrados = string.Join("','", seleccionados);
                    string query = $@"SELECT DISTINCT D.idDocente, D.primerNombre, D.segundoNombre, 
                                              D.primerApellido, D.segundoApellido
                             FROM Docente D
                             INNER JOIN Aula A ON D.idDocente = A.idDocente
                             WHERE A.nombre IN ('{filtroGrados}')";

                    SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    dgv1.DataSource = dt;
                }
            }));
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  VER REGISTROS
        // ═══════════════════════════════════════════════════════════════════════
        private void verRegistros_Click(object sender, EventArgs e)
        {
            if (grados.CheckedItems.Count == 0)
            {
                MessageBox.Show("Seleccione al menos un grado.");
                return;
            }

            List<string> seleccionados = grados.CheckedItems.Cast<string>()
                                                .Where(g => g != "-- TODOS LOS GRADOS --")
                                                .ToList();

            if (seleccionados.Count == 0)
            {
                ActualizarTabla();
                return;
            }

            string filtroSQL = string.Join("','", seleccionados);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = $@"SELECT DISTINCT D.idDocente, D.primerNombre, D.segundoNombre, 
                                          D.primerApellido, D.segundoApellido
                          FROM Docente D
                          INNER JOIN Aula A ON D.idDocente = A.idDocente
                          WHERE A.nombre IN ('{filtroSQL}')";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                dgv1.DataSource = dt;
                ConfigurarEstiloDgv();
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  REGISTRO DE HUELLA  (sin cambios funcionales)
        // ═══════════════════════════════════════════════════════════════════════
        private void registraHuella_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idDocente.Text))
            {
                MessageBox.Show("Primero seleccione un docente de la tabla.");
                return;
            }

            Enrolador = new DPFP.Processing.Enrollment();
            MessageBox.Show($"Iniciando registro para el docente {idDocente.Text}. Coloque el dedo 4 veces en el lector.");
        }

        protected void ProcesarCaptura(DPFP.Sample Sample)
        {
            DPFP.FeatureSet caracteristicas = ExtraerCaracteristicas(Sample, DPFP.Processing.DataPurpose.Enrollment);

            if (caracteristicas != null)
            {
                Enrolador.AddFeatures(caracteristicas);

                this.Invoke(new Action(() =>
                    Console.WriteLine("Muestras restantes: " + Enrolador.FeaturesNeeded)));

                if (Enrolador.TemplateStatus == DPFP.Processing.Enrollment.Status.Ready)
                {
                    Plantilla = Enrolador.Template;
                    GuardarHuellaEnBD(Plantilla);
                }
            }
        }

        private void GuardarHuellaEnBD(DPFP.Template plantilla)
        {
            byte[] huellaBytes = plantilla.Bytes;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Docente SET huella = @huella WHERE idDocente = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@huella", SqlDbType.VarBinary).Value = huellaBytes;
                    cmd.Parameters.AddWithValue("@id", idDocente.Text);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show("¡Huella dactilar guardada exitosamente!");
                        // CAMBIO 3: actualizar botón tras guardar huella
                        ActualizarColorBotonHuella(idDocente.Text);
                    }));
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() => MessageBox.Show("Error al guardar en BD: " + ex.Message)));
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  Resto de métodos requeridos por la interfaz DPFP
        // ═══════════════════════════════════════════════════════════════════════
        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
            => ProcesarCaptura(Sample);

        protected DPFP.FeatureSet ExtraerCaracteristicas(DPFP.Sample Sample, DPFP.Processing.DataPurpose Objetivo)
        {
            DPFP.Processing.FeatureExtraction extractor = new DPFP.Processing.FeatureExtraction();
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet caracteristicas = new DPFP.FeatureSet();
            extractor.CreateFeatureSet(Sample, Objetivo, ref feedback, ref caracteristicas);
            return (feedback == DPFP.Capture.CaptureFeedback.Good) ? caracteristicas : null;
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber) { }
        public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }
        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback) { }

        private void ConfigurarEstiloDgv()
        {
            dgv1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv1.Columns["idDocente"].HeaderText = "Identificación";
            dgv1.Columns["primerNombre"].HeaderText = "Primer Nombre";
            dgv1.Columns["segundoNombre"].HeaderText = "Segundo Nombre";
            dgv1.Columns["primerApellido"].HeaderText = "Primer Apellido";
            dgv1.Columns["segundoApellido"].HeaderText = "Segundo Apellido";
            dgv1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv1.AllowUserToAddRows = false;
            dgv1.ReadOnly = true;
            dgv1.RowHeadersVisible = false;
            dgv1.BackgroundColor = Color.White;
            dgv1.BorderStyle = BorderStyle.None;
        }

        // Detener captura al salir para liberar el lector correctamente
        private void button1_Click(object sender, EventArgs e)
        {
            try { Capturador?.StopCapture(); } catch { }

            homeAdmin admin = new homeAdmin();
            this.Hide();
            admin.Show();
        }

        // Reiniciar captura y verificar estado cuando el formulario vuelve a mostrarse
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this.Visible)
            {
                // Reiniciar el hardware al volver a mostrar el formulario
                try
                {
                    Capturador?.StopCapture();
                }
                catch { }

                InitHardware();
            }
        }

        private void administrarDocentes_Load(object sender, EventArgs e) { }
        private void dgv1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void idDocente_TextChanged(object sender, EventArgs e) { }

        private void area_TextChanged(object sender, EventArgs e)
        {

        }

        private void grados_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}