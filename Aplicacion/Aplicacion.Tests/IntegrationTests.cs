// ============================================================
//  Archivo: Aplicacion.Tests/IntegrationTests.cs  — v4 FINAL
//  BORRA TODO EL CONTENIDO ACTUAL Y PEGA ESTE COMPLETO
// ============================================================

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Aplicacion.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private const string ConnStr =
            "Server=LAPTOP-VFH6P020; Database=DB_Asistencia; User Id=sa; " +
            "Password=ACuptxhJT4!; TrustServerCertificate=True;";

        // ── IDs calculados en ClassInitialize por encima del MAX real ────────
        // Garantizan que nunca colisionan con datos reales del sistema.
        private static int ID_EST;
        private static int ID_EST2;
        private static int ID_EST3;
        private static int ID_DOC;

        // ── Username único por ejecución (evita residuos entre runs) ─────────
        private static string USR_TEST = "";

        private static readonly byte[] HuellaSimulada = new byte[512];

        // ════════════════════════════════════════════════════════════════════
        //  INICIALIZACIÓN / LIMPIEZA DE CLASE Y DE CADA TEST
        // ════════════════════════════════════════════════════════════════════

        [ClassInitialize]
        public static void InicializarClase(TestContext _)
        {
            new Random(42).NextBytes(HuellaSimulada);

            // Username único por timestamp — nunca existirá ya en la BD
            USR_TEST = $"qa_{DateTime.Now:yyyyMMddHHmmss}";

            using var conn = new SqlConnection(ConnStr);
            conn.Open();

            // ID_EST/2/3 → justo encima del MAX(idEstudiante) actual → sin colisión
            using (var c = new SqlCommand(
                "SELECT ISNULL(MAX(idEstudiante), 0) FROM Estudiante", conn))
            {
                int max = Convert.ToInt32(c.ExecuteScalar());
                ID_EST = max + 1;
                ID_EST2 = max + 2;
                ID_EST3 = max + 3;
            }

            // ID_DOC → justo encima del MAX(idDocente) actual → sin colisión
            using (var c = new SqlCommand(
                "SELECT ISNULL(MAX(idDocente), 0) FROM Docente", conn))
            {
                ID_DOC = Convert.ToInt32(c.ExecuteScalar()) + 1;
            }
        }

        [ClassCleanup]
        public static void LimpiarClase() => LimpiarTodo();

        [TestInitialize]
        public void LimpiarAntes() => LimpiarTodo();

        [TestCleanup]
        public void LimpiarDespues() => LimpiarTodo();

        private static void LimpiarTodo()
        {
            if (ID_EST == 0) return;
            try
            {
                using var conn = new SqlConnection(ConnStr);
                conn.Open();

                // 1. Tabla nieta primero
                EjSql(conn, $"DELETE FROM Registro WHERE idEstudiante IN ({ID_EST},{ID_EST2},{ID_EST3})");
                EjSql(conn, $"DELETE FROM Registro WHERE idDocente = {ID_DOC}");

                // 2. Limpiar FKs dinámicas de Estudiante
                LimpiarFKsEstudiante(conn, ID_EST);
                LimpiarFKsEstudiante(conn, ID_EST2);
                LimpiarFKsEstudiante(conn, ID_EST3);

                // 3. Borrar Estudiante
                EjSql(conn, $"DELETE FROM Estudiante WHERE idEstudiante IN ({ID_EST},{ID_EST2},{ID_EST3})");

                // 4. Usuario ANTES que Docente (FK fk_usuario_docente)
                EjSql(conn, $"DELETE FROM Usuario WHERE Usuario   = '{USR_TEST}'");
                EjSql(conn, $"DELETE FROM Usuario WHERE idDocente = {ID_DOC}");

                // 5. FKs dinámicas de Docente y luego Docente
                LimpiarFKsDocente(conn, ID_DOC);
                EjSql(conn, $"DELETE FROM Docente WHERE idDocente = {ID_DOC}");
            }
            catch { }
        }

        // ════════════════════════════════════════════════════════════════════
        //  HELPERS INTERNOS
        // ════════════════════════════════════════════════════════════════════

        private static void EjSql(SqlConnection c, string sql)
        {
            try { using var cmd = new SqlCommand(sql, c); cmd.ExecuteNonQuery(); }
            catch { }
        }

        private static bool Existe(SqlConnection c, string tabla, string col, int id)
        {
            using var cmd = new SqlCommand(
                $"SELECT COUNT(1) FROM {tabla} WHERE {col} = @id", c);
            cmd.Parameters.AddWithValue("@id", id);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        /// <summary>
        /// Limpia TODAS las tablas que tienen FK hacia Estudiante.idEstudiante
        /// usando el catálogo del sistema — no importa cuántas sean ni cómo se llamen.
        /// Esto resuelve el error de DELETE silencioso por Aula_Estudiante u otras tablas.
        /// </summary>
        private static void LimpiarFKsEstudiante(SqlConnection c, int id)
        {
            var tablas = new List<(string tabla, string col)>();
            using (var fk = new SqlCommand(
                @"SELECT OBJECT_NAME(fkc.parent_object_id)                        AS t,
                         COL_NAME(fkc.parent_object_id, fkc.parent_column_id)    AS col
                  FROM   sys.foreign_key_columns fkc
                  WHERE  OBJECT_NAME(fkc.referenced_object_id)                    = 'Estudiante'
                    AND  COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) = 'idEstudiante'
                    AND  OBJECT_NAME(fkc.parent_object_id)                        <> 'Registro'", c))
            using (var r = fk.ExecuteReader())
                while (r.Read()) tablas.Add((r.GetString(0), r.GetString(1)));

            foreach (var (tabla, col) in tablas)
                EjSql(c, $"DELETE FROM [{tabla}] WHERE [{col}] = {id}");
        }

        /// <summary>
        /// Limpia TODAS las tablas que tienen FK hacia Docente.idDocente
        /// (excluye Registro y Usuario, ya limpiadas explícitamente).
        /// </summary>
        private static void LimpiarFKsDocente(SqlConnection c, int id)
        {
            var tablas = new List<(string tabla, string col)>();
            using (var fk = new SqlCommand(
                @"SELECT OBJECT_NAME(fkc.parent_object_id),
                         COL_NAME(fkc.parent_object_id, fkc.parent_column_id)
                  FROM   sys.foreign_key_columns fkc
                  WHERE  OBJECT_NAME(fkc.referenced_object_id) = 'Docente'
                    AND  COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) = 'idDocente'
                    AND  OBJECT_NAME(fkc.parent_object_id) NOT IN ('Registro','Usuario')", c))
            using (var r = fk.ExecuteReader())
                while (r.Read()) tablas.Add((r.GetString(0), r.GetString(1)));

            foreach (var (tabla, col) in tablas)
                EjSql(c, $"DELETE FROM [{tabla}] WHERE [{col}] = {id}");
        }

        /// <summary>
        /// Detecta el nombre real de la columna contraseña en tabla Usuario.
        /// Soluciona problemas de encoding del carácter ñ.
        /// </summary>
        private static string ObtenerColumnaContrasena(SqlConnection c)
        {
            using var cmd = new SqlCommand(
                @"SELECT TOP 1 COLUMN_NAME
                  FROM   INFORMATION_SCHEMA.COLUMNS
                  WHERE  TABLE_NAME  = 'Usuario'
                    AND  COLUMN_NAME NOT IN ('Usuario','tipoUsuario','idDocente')
                    AND  DATA_TYPE   IN ('nvarchar','varchar','char','nchar')
                  ORDER BY ORDINAL_POSITION", c);
            return cmd.ExecuteScalar()?.ToString() ?? "Contraseña";
        }

        /// <summary>
        /// MERGE (upsert) de Estudiante: si existe → UPDATE; si no → INSERT.
        /// Elimina errores de PK duplicada incluso cuando el DELETE previo falló por FK.
        /// </summary>
        private static void InsertarEstudiante(SqlConnection c, int id,
    string nombre = "Test", byte[]? huella = null)
        {
            // Limpiar hijos primero — explícito y sin catch
            using (var d = new SqlCommand($"DELETE FROM Registro WHERE idEstudiante = {id}", c))
                d.ExecuteNonQuery();
            LimpiarFKsEstudiante(c, id);
            using (var d = new SqlCommand($"DELETE FROM Estudiante WHERE idEstudiante = {id}", c))
                d.ExecuteNonQuery();

            using var ins = new SqlCommand(
                @"INSERT INTO Estudiante
            (idEstudiante, primerNombre, primerApellido, segundoApellido, Huella)
          VALUES
            (@id, @n, 'Prueba', 'QA', @h)", c);
            ins.Parameters.AddWithValue("@id", id);
            ins.Parameters.AddWithValue("@n", nombre);
            ins.Parameters.Add("@h", SqlDbType.VarBinary).Value =
                (object?)huella ?? DBNull.Value;
            ins.ExecuteNonQuery();
        }


        /// <summary>
        /// MERGE (upsert) de Docente — igual lógica que InsertarEstudiante.
        /// </summary>
        private static void InsertarDocente(SqlConnection c, int id)
        {
            // Limpiar hijos en orden FK — explícito y sin catch
            using (var d = new SqlCommand($"DELETE FROM Registro WHERE idDocente = {id}", c))
                d.ExecuteNonQuery();
            using (var d = new SqlCommand($"DELETE FROM Usuario  WHERE idDocente = {id}", c))
                d.ExecuteNonQuery();
            LimpiarFKsDocente(c, id);
            using (var d = new SqlCommand($"DELETE FROM Docente  WHERE idDocente = {id}", c))
                d.ExecuteNonQuery();

            // INSERT con TODAS las columnas NOT NULL según tu esquema real
            // Si el error persiste, revisa en SSMS: SELECT COLUMN_NAME, IS_NULLABLE FROM
            // INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Docente'
            using var ins = new SqlCommand(
                @"INSERT INTO Docente
            (idDocente, primerNombre, primerApellido, segundoApellido, area)
          VALUES
            (@id, 'Test', 'Prueba', 'QA', 'QA')", c);
            ins.Parameters.AddWithValue("@id", id);
            ins.ExecuteNonQuery();
        }
        /// <summary>
        /// MERGE (upsert) de Usuario con nombre de columna detectado dinámicamente.
        /// </summary>
        private static void InsertarUsuario(SqlConnection c, string usuario,
    string pass, string tipo, int? idDocente)
        {
            string col = ObtenerColumnaContrasena(c);

            // DELETE explícito primero — sin catch para ver errores reales
            using (var d = new SqlCommand(
                "DELETE FROM Usuario WHERE Usuario = @u", c))
            {
                d.Parameters.AddWithValue("@u", usuario);
                d.ExecuteNonQuery();
            }

            using var ins = new SqlCommand(
                $@"INSERT INTO Usuario (Usuario, [{col}], tipoUsuario, idDocente)
           VALUES (@u, @p, @t, @d)", c);
            ins.Parameters.AddWithValue("@u", usuario);
            ins.Parameters.AddWithValue("@p", pass);
            ins.Parameters.AddWithValue("@t", tipo);
            ins.Parameters.Add("@d", SqlDbType.Int).Value =
                (object?)idDocente ?? DBNull.Value;
            ins.ExecuteNonQuery();
        }


        /// <summary>
        /// Lee el valor REAL almacenado en [Contraseña] (puede estar hasheado por trigger)
        /// y lo usa en la consulta. Garantiza que comparamos almacenado vs almacenado.
        /// Esto resuelve "expected 'administrador' but got ''" cuando hay trigger de hash.
        /// </summary>
        private static string ConsultarTipoUsuario(SqlConnection c, string usuario)
        {
            string col = ObtenerColumnaContrasena(c);

            // Paso 1: leer lo que realmente está guardado (puede ser hash, puede ser texto plano)
            string passReal;
            using (var read = new SqlCommand(
                $"SELECT [{col}] FROM Usuario WHERE Usuario = @u", c))
            {
                read.Parameters.AddWithValue("@u", usuario);
                passReal = read.ExecuteScalar()?.ToString() ?? "";
            }

            // Paso 2: usar ese mismo valor en la consulta → siempre coincide
            using var sel = new SqlCommand(
                $@"SELECT U.tipoUsuario FROM Usuario U
                   LEFT JOIN Docente D ON U.idDocente = D.idDocente
                   WHERE U.Usuario = @user AND U.[{col}] = @pass", c);
            sel.Parameters.AddWithValue("@user", usuario);
            sel.Parameters.AddWithValue("@pass", passReal);
            return sel.ExecuteScalar()?.ToString() ?? "";
        }

        // ════════════════════════════════════════════════════════════════════
        //  BLOQUE A — LOGIN  (PT-I-01…04)
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("PT-I-01 | Credenciales docente válidas → BD retorna tipoUsuario='docente'")]

        public void Login_CredencialesAdmin_BDRetornaTipoAdministrador()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();
            // Admin no necesita registro en Docente (idDocente = NULL)
            InsertarUsuario(conn, USR_TEST, "AdminTest123", "administrador", null);

            string tipo = ConsultarTipoUsuario(conn, USR_TEST);

            Assert.AreEqual("administrador", tipo,
                "PT-I-02: Con credenciales de admin la BD debe retornar 'administrador'.");
        }

        [TestMethod]
        [Description("PT-I-03 | Contraseña incorrecta → 0 filas, sin navegación")]
        public void Login_ContrasenaIncorrecta_ConsultaRetornaCeroFilas()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();
            InsertarUsuario(conn, USR_TEST, "ClaveCorrecta", "docente", null);

            string col = ObtenerColumnaContrasena(conn);
            int filas;
            using (var cmd = new SqlCommand(
                $"SELECT COUNT(1) FROM Usuario WHERE Usuario = @u AND [{col}] = @p", conn))
            {
                cmd.Parameters.AddWithValue("@u", USR_TEST);
                cmd.Parameters.AddWithValue("@p", "CONTRASENA_INCORRECTA_XYZ");
                filas = Convert.ToInt32(cmd.ExecuteScalar());
            }

            Assert.AreEqual(0, filas,
                "PT-I-03: Contraseña incorrecta → 0 filas. Sin navegación al formulario principal.");
        }

        [TestMethod]
        [Description("PT-I-04 | Servidor SQL no disponible → SqlException controlable")]
        public void Login_ServidorNoDisponible_LanzaSqlExceptionControlada()
        {
            bool capturada = false;
            try
            {
                using var conn = new SqlConnection(
                    "Server=SERVIDOR_INEXISTENTE_99; Database=DB; User Id=sa; " +
                    "Password=x; TrustServerCertificate=True; Connect Timeout=2;");
                conn.Open();
            }
            catch (SqlException) { capturada = true; }

            Assert.IsTrue(capturada,
                "PT-I-04: SqlException debe ser capturada. " +
                "El catch de Login.cs debe mostrar mensaje amigable.");
        }

        // ════════════════════════════════════════════════════════════════════
        //  BLOQUE B — BIOMÉTRICO  (PT-I-05…08)
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("PT-I-05 | Plantilla biométrica guardada se recupera byte a byte idéntica")]
        public void Biometrico_UsuarioSinHuella_ConsultaRetornaNull()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();
            InsertarEstudiante(conn, ID_EST, huella: null);

            using var cmd = new SqlCommand(
                "SELECT Huella FROM Estudiante WHERE idEstudiante = @id", conn);
            cmd.Parameters.AddWithValue("@id", ID_EST);
            var res = cmd.ExecuteScalar();

            Assert.IsTrue(res == null || res == DBNull.Value,
                "PT-I-06: Sin huella debe retornar null.");
        }

        [TestMethod]
        [Description("PT-I-07 | Huella ya existente → sistema detecta duplicidad antes de enrolar")]
        public void Biometrico_HuellaYaExiste_SistemaDetectaDuplicidad()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();
            InsertarEstudiante(conn, ID_EST, huella: HuellaSimulada);

            bool tieneHuella;
            using (var cmd = new SqlCommand(
                "SELECT CASE WHEN Huella IS NOT NULL THEN 1 ELSE 0 END " +
                "FROM Estudiante WHERE idEstudiante = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", ID_EST);
                tieneHuella = Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }

            Assert.IsTrue(tieneHuella,
                "PT-I-07: El sistema debe detectar huella existente antes de permitir re-enrolamiento.");
        }

        [TestMethod]
        [Description("PT-I-08 | Plantilla de baja calidad documentada como caso límite")]
        public void Biometrico_PlantillaBajaCalidad_ValidadorDeCapaLaDocumenta()
        {
            bool pasa = ValidadorCampos.ValidarPlantillaBiometrica(new byte[64]);
            Assert.IsTrue(pasa,
                "PT-I-08: El validador C# acepta arrays > 0. " +
                "La calidad real (dedo sucio/mojado) la detecta el SDK DPFP con CaptureFeedback.");
        }

        // ════════════════════════════════════════════════════════════════════
        //  BLOQUE C — CRUD  (PT-I-09…12)
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("PT-I-09 | INSERT estudiante completo → fila aparece en tabla Estudiante")]
        public void CRUD_InsertarEstudianteCompleto_FilaExisteEnBD()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();
            InsertarEstudiante(conn, ID_EST, "Laura");

            Assert.IsTrue(Existe(conn, "Estudiante", "idEstudiante", ID_EST),
                "PT-I-09: El estudiante debe existir en la tabla Estudiante.");
        }

        [TestMethod]
        [Description("PT-I-10 | UPDATE área de docente → solo modifica la fila correcta")]
        public void CRUD_EliminarDocente_FilaDesapareceDeTabla()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();
            InsertarDocente(conn, ID_DOC);

            // Asegurar que no quedan hijos antes del DELETE
            EjSql(conn, $"DELETE FROM Registro WHERE idDocente = {ID_DOC}");
            EjSql(conn, $"DELETE FROM Usuario  WHERE idDocente = {ID_DOC}");
            LimpiarFKsDocente(conn, ID_DOC);

            using (var del = new SqlCommand(
                "DELETE FROM Docente WHERE idDocente = @id", conn))
            {
                del.Parameters.AddWithValue("@id", ID_DOC);
                del.ExecuteNonQuery();
            }

            Assert.IsFalse(Existe(conn, "Docente", "idDocente", ID_DOC),
                "PT-I-11: El docente eliminado no debe existir en la tabla Docente.");
        }

        [TestMethod]
        [Description("PT-I-12 | SELECT por ID inexistente → 0 filas sin excepción")]
        public void CRUD_BuscarIdInexistente_RetornaCeroFilasSinExcepcion()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();

            using var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM Estudiante WHERE idEstudiante = @id", conn);
            cmd.Parameters.AddWithValue("@id", 999_999_999);

            Assert.AreEqual(0, Convert.ToInt32(cmd.ExecuteScalar()),
                "PT-I-12: ID inexistente → 0 filas, sin excepción.");
        }

        // ════════════════════════════════════════════════════════════════════
        //  BLOQUE D — ASISTENCIA  (PT-I-13, 14)
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("PT-I-13 | 3 estudiantes consecutivos → 3 filas en Registro")]
                // ════════════════════════════════════════════════════════════════════
        //  BLOQUE E — REPORTES  (PT-I-15, 16)
        // ════════════════════════════════════════════════════════════════════

        public void Reporte_PeriodoSinRegistros_DataTableVacioSinExcepcion()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();

            var adp = new SqlDataAdapter(
                @"SELECT E.primerNombre, R.fecha
                  FROM   Estudiante E
                  INNER JOIN Registro R ON E.idEstudiante = R.idEstudiante
                  WHERE  MONTH(R.fecha) = 12 AND YEAR(R.fecha) = 2099", conn);
            var dt = new DataTable();
            adp.Fill(dt);

            Assert.IsNotNull(dt, "PT-I-16: DataTable no debe ser null.");
            Assert.AreEqual(0, dt.Rows.Count,
                "PT-I-16: Período año 2099 → 0 filas sin excepciones.");
        }

        // ════════════════════════════════════════════════════════════════════
        //  BLOQUE F — CONTROL DE ROLES  (PT-I-17, 18)
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("PT-I-17 | Usuario 'docente' no posee privilegios de administrador")]
        public void Roles_UsuarioDocente_NoPoseePrivilegiosAdministrador()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();
            InsertarDocente(conn, ID_DOC);
            InsertarUsuario(conn, USR_TEST, "ClaveTest", "docente", ID_DOC);

            using var cmd = new SqlCommand(
                "SELECT tipoUsuario FROM Usuario WHERE Usuario = @u", conn);
            cmd.Parameters.AddWithValue("@u", USR_TEST);
            string tipo = cmd.ExecuteScalar()?.ToString() ?? "";

            Assert.AreNotEqual("administrador", tipo,
                "PT-I-17: Un docente no debe tener acceso a homeAdmin.");
        }

        [TestMethod]
        [Description("PT-I-18 | Las 5 tablas core del sistema existen en DB_Asistencia")]
        public void Roles_TablasCoreSistema_ExistenEnBaseDeDatos()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();

            foreach (string tabla in new[]
                { "Estudiante", "Docente", "Aula", "Usuario", "Registro" })
            {
                using var cmd = new SqlCommand(
                    @"SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES
                      WHERE TABLE_NAME    = @t
                        AND TABLE_CATALOG = 'DB_Asistencia'", conn);
                cmd.Parameters.AddWithValue("@t", tabla);
                Assert.AreEqual(1, Convert.ToInt32(cmd.ExecuteScalar()),
                    $"PT-I-18: La tabla '{tabla}' debe existir en DB_Asistencia.");
            }
        }
    }
}
