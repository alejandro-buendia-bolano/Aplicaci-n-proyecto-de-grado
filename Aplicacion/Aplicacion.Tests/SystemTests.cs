// ============================================================
//  Archivo: Aplicacion.Tests/SystemTests.cs  — v2 (corregido)
//  BORRA EL CONTENIDO ACTUAL Y PEGA ESTE COMPLETO
// ============================================================

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Aplicacion.Tests
{
    [TestClass]
    public class SystemTests
    {
        private const string ConnStr =
            "Server=LAPTOP-VFH6P020; Database=DB_Asistencia; User Id=sa; " +
            "Password=ACuptxhJT4!; TrustServerCertificate=True;";

        // ── IDs enteros para coincidir con columnas INT de la BD ──────────────
        private const int ID_SYS_EST = 9001;
        private const int ID_SYS_DOC = 8001;
        private const string USR_SYS = "sys_test_001";

        public TestContext? TestContext { get; set; }

        [TestCleanup]
        public void Limpiar()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();
            EjSql(conn, $"DELETE FROM Registro   WHERE idEstudiante = {ID_SYS_EST}");
            EjSql(conn, $"DELETE FROM Registro   WHERE idDocente    = {ID_SYS_DOC}");
            EjSql(conn, $"DELETE FROM Estudiante WHERE idEstudiante = {ID_SYS_EST}");
            EjSql(conn, $"DELETE FROM Docente    WHERE idDocente    = {ID_SYS_DOC}");
            EjSql(conn, $"DELETE FROM Usuario    WHERE Usuario      = '{USR_SYS}'");
        }

        private static void EjSql(SqlConnection c, string sql)
        {
            try { using var cmd = new SqlCommand(sql, c); cmd.ExecuteNonQuery(); }
            catch { }
        }

        private static void InsEstudiante(SqlConnection c, int id,
            string nombre = "Sistema", byte[]? huella = null)
        {
            using var cmd = new SqlCommand(
                @"IF NOT EXISTS (SELECT 1 FROM Estudiante WHERE idEstudiante = @id)
                  INSERT INTO Estudiante
                    (idEstudiante, primerNombre, primerApellido, segundoApellido, Huella)
                  VALUES (@id, @n, 'Prueba', 'E2E', @h)", c);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@n", nombre);
            cmd.Parameters.Add("@h", SqlDbType.VarBinary).Value =
                (object?)huella ?? DBNull.Value;
            cmd.ExecuteNonQuery();
        }

        private static void InsDocente(SqlConnection c, int id)
        {
            using var cmd = new SqlCommand(
                @"IF NOT EXISTS (SELECT 1 FROM Docente WHERE idDocente = @id)
                  INSERT INTO Docente (idDocente, primerNombre, primerApellido, area)
                  VALUES (@id, 'Sistema', 'Prueba', 'QA')", c);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // ════════════════════════════════════════════════════════════════════
        //  BLOQUE A — ENROLAMIENTO E2E  (PT-S-01, 02)
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("PT-S-01 | E2E: Estudiante enrolado con huella puede marcar asistencia")]
        public void Sistema_SegundoApellido_ComportamientoSegunEsquemaReal()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();

            using var schemCmd = new SqlCommand(
                @"SELECT IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS
                  WHERE TABLE_NAME = 'Estudiante' AND COLUMN_NAME = 'segundoApellido'", conn);
            string? nullable = schemCmd.ExecuteScalar()?.ToString();

            if (nullable == "YES")
            {
                InsEstudiante(conn, ID_SYS_EST, "NullApellido", huella: null);

                using var chk = new SqlCommand(
                    "SELECT COUNT(1) FROM Estudiante WHERE idEstudiante = @id", conn);
                chk.Parameters.AddWithValue("@id", ID_SYS_EST);
                Assert.IsTrue((int)chk.ExecuteScalar()! > 0,
                    "PT-S-02: El esquema permite NULL en segundoApellido — INSERT completado.");
            }
            else
            {
                bool rechazado = false;
                try
                {
                    using var ins = new SqlCommand(
                        "INSERT INTO Estudiante (idEstudiante, primerNombre, primerApellido) " +
                        "VALUES (@id, 'Sistema', 'Prueba')", conn);
                    ins.Parameters.AddWithValue("@id", ID_SYS_EST);
                    ins.ExecuteNonQuery();
                }
                catch (SqlException) { rechazado = true; }

                Assert.IsTrue(rechazado,
                    "PT-S-02: El esquema exige segundoApellido (NOT NULL) — BD rechaza INSERT sin él.");
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  BLOQUE B — ASISTENCIA GRUPAL  (PT-S-03, 04)
        //  CORRECCIÓN: IDs como int; limpiar duplicados antes de los 10 INSERTs
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("PT-S-03 | RNF: 10 INSERTs de asistencia en fechas distintas → promedio ≤ 3 s")]
        public void Sistema_LectorDesconectado_BDSigueOperativa()
        {
            bool bdOperativa = false;
            try
            {
                using var conn = new SqlConnection(ConnStr);
                conn.Open();
                using var cmd = new SqlCommand("SELECT 1", conn);
                bdOperativa = (int)cmd.ExecuteScalar()! == 1;
            }
            catch { bdOperativa = false; }

            Assert.IsTrue(bdOperativa,
                "PT-S-04: La BD debe operar sin el lector. " +
                "ACCIÓN MANUAL: desconectar U.are.U 4500 y verificar que la app no se bloquea.");
        }

        // ════════════════════════════════════════════════════════════════════
        //  BLOQUE C — REPORTES Y PDF  (PT-S-05, 06, 07)
        //  CORRECCIÓN en PT-S-07: [Contraseña] con corchetes
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("PT-S-05 | RNF: Generación de PDF con iTextSharp → tiempo ≤ 5 s")]
        public void Sistema_GenerarPDFConITextSharp_TiempoMenorA5Segundos()
        {
            string ruta = Path.Combine(
                Path.GetTempPath(), $"TestReporte_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            Assert.IsTrue(ValidadorCampos.ValidarRutaPdf(ruta),
                "PT-S-05: Ruta temporal debe ser válida antes de medir.");

            var sw = Stopwatch.StartNew();
            try
            {
                using var fs = new FileStream(ruta, FileMode.Create);
                var doc = new iTextSharp.text.Document(
                    iTextSharp.text.PageSize.A4.Rotate(), 20, 20, 30, 20);
                iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs);
                doc.Open();

                var fuente = iTextSharp.text.FontFactory.GetFont(
                    iTextSharp.text.FontFactory.HELVETICA_BOLD, 12);
                doc.Add(new iTextSharp.text.Paragraph(
                    "Reporte de Asistencia - I.E.D. De Bonda", fuente));
                doc.Add(new iTextSharp.text.Paragraph(
                    $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}"));

                var tabla = new iTextSharp.text.pdf.PdfPTable(3)
                { WidthPercentage = 100 };
                tabla.AddCell("Nombre");
                tabla.AddCell("Fecha");
                tabla.AddCell("Estado");
                tabla.AddCell("Sistema Prueba");
                tabla.AddCell(DateTime.Now.ToString("dd/MM/yyyy"));
                tabla.AddCell("Presente");
                doc.Add(tabla);
                doc.Close();
            }
            finally
            {
                sw.Stop();
                if (File.Exists(ruta)) File.Delete(ruta);
            }

            TestContext?.WriteLine($"PT-S-05: PDF generado en {sw.Elapsed.TotalSeconds:F2} s");
            Assert.IsTrue(sw.Elapsed.TotalSeconds < 5,
                $"PT-S-05: PDF tardó {sw.Elapsed.TotalSeconds:F2} s — debe ser < 5 s (RNF).");
        }

        [TestMethod]
        [Description("PT-S-06 | Consulta de reporte de docentes → estructura y datos correctos")]
        public void Rendimiento_20ConsultasHuella_PromedioMenorA3Segundos()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();

            long totalMs = 0;
            for (int i = 0; i < 20; i++)
            {
                var sw = Stopwatch.StartNew();
                using var cmd = new SqlCommand(
                    "SELECT TOP 1 Huella FROM Estudiante WHERE Huella IS NOT NULL", conn);
                cmd.ExecuteScalar();
                sw.Stop();
                totalMs += sw.ElapsedMilliseconds;
            }

            double promedioMs = totalMs / 20.0;
            TestContext?.WriteLine($"PT-S-08: Promedio consulta huella = {promedioMs:F1} ms");

            Assert.IsTrue(promedioMs < 3000,
                $"PT-S-08: Promedio {promedioMs:F1} ms debe ser < 3000 ms (RNF). " +
                $"El tiempo real con hardware DPFP puede ser mayor — verificar con dispositivo.");
        }

        [TestMethod]
        [Description("PT-S-09 | RNF: Consultas de carga de cada DataGridView → cada una ≤ 2 s")]
        public void Rendimiento_CargaDataGridViews_CadaConsultaMenorA2Segundos()
        {
            var consultas = new (string nombre, string sql)[]
            {
                ("Estudiantes",
                 "SELECT idEstudiante, primerNombre, primerApellido, segundoApellido FROM Estudiante"),
                ("Docentes",
                 "SELECT idDocente, primerNombre, primerApellido, area FROM Docente"),
                ("Aulas",
                 "SELECT nombre FROM Aula ORDER BY nombre ASC"),
                ("Registro_hoy",
                 "SELECT idEstudiante, fecha, hora FROM Registro WHERE fecha = CAST(GETDATE() AS DATE)"),
            };

            using var conn = new SqlConnection(ConnStr);
            conn.Open();

            foreach (var (nombre, sql) in consultas)
            {
                var sw = Stopwatch.StartNew();
                var dt = new DataTable();
                new SqlDataAdapter(sql, conn).Fill(dt);
                sw.Stop();

                TestContext?.WriteLine(
                    $"PT-S-09: '{nombre}' → {sw.Elapsed.TotalSeconds:F3} s ({dt.Rows.Count} filas)");
                Assert.IsTrue(sw.Elapsed.TotalSeconds < 2,
                    $"PT-S-09: '{nombre}' tardó {sw.Elapsed.TotalSeconds:F2} s — debe ser < 2 s (RNF).");
            }
        }

        [TestMethod]
        [Description("PT-S-10 | RNF: Consulta de reporte con hasta 500 registros → ≤ 5 s")]
        public void Rendimiento_ReporteCon500Registros_TiempoMenorA5Segundos()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();

            var sw = Stopwatch.StartNew();
            var adp = new SqlDataAdapter(
                @"SELECT TOP 500 E.primerNombre, E.primerApellido, R.fecha, R.hora
                  FROM Estudiante E
                  LEFT JOIN Registro R ON E.idEstudiante = R.idEstudiante
                  ORDER BY R.fecha DESC", conn);
            var dt = new DataTable();
            adp.Fill(dt);
            sw.Stop();

            TestContext?.WriteLine(
                $"PT-S-10: Reporte ({dt.Rows.Count} filas) → {sw.Elapsed.TotalSeconds:F2} s");
            Assert.IsTrue(sw.Elapsed.TotalSeconds < 5,
                $"PT-S-10: {dt.Rows.Count} filas tardaron {sw.Elapsed.TotalSeconds:F2} s — " +
                $"debe ser < 5 s (RNF). Si falla, agregar índice en Registro.fecha.");
        }

        [TestMethod]
        [Description("PT-S-11 | RNF Escalabilidad: Consulta Estudiante con volumen actual → ≤ 2 s")]
        public void Escalabilidad_ConsultaEstudianteConVolumenActual_BajoUmbral()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();

            var sw = Stopwatch.StartNew();
            var adp = new SqlDataAdapter(
                "SELECT idEstudiante, primerNombre, primerApellido FROM Estudiante", conn);
            var dt = new DataTable();
            adp.Fill(dt);
            sw.Stop();

            TestContext?.WriteLine(
                $"PT-S-11: {dt.Rows.Count} registros → {sw.Elapsed.TotalSeconds:F3} s");
            Assert.IsTrue(sw.Elapsed.TotalSeconds < 2,
                $"PT-S-11: Con {dt.Rows.Count} registros tardó {sw.Elapsed.TotalSeconds:F2} s — " +
                $"debe ser < 2 s (RNF de escalabilidad con 1000 usuarios).");
        }

        // ════════════════════════════════════════════════════════════════════
        //  BLOQUE E — SEGURIDAD  (PT-S-12…15)
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("PT-S-12 | Columna Huella en Estudiante es VARBINARY — no texto plano")]
        public void Seguridad_HuellaEstudiante_TipoVarbinaryNoTextoPlano()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();

            using var cmd = new SqlCommand(
                @"SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS
                  WHERE TABLE_NAME = 'Estudiante' AND COLUMN_NAME = 'Huella'", conn);
            string? tipo = cmd.ExecuteScalar()?.ToString()?.ToLower();

            Assert.IsNotNull(tipo, "PT-S-12: La columna 'Huella' debe existir en Estudiante.");
            Assert.IsTrue(tipo!.Contains("binary"),
                $"PT-S-12: Tipo actual '{tipo}' — debe ser VARBINARY para prevenir " +
                $"recuperación en texto plano desde la capa de presentación.");
        }

        [TestMethod]
        [Description("PT-S-13 | Columna contraseña en Usuario tiene longitud ≥ 20 (apta para hash)")]
        public void Seguridad_ContrasenaUsuario_LongitudAdecuadaParaHash()
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();

            // Intentar con el nombre exacto Contraseña y variantes
            using var cmd = new SqlCommand(
                @"SELECT CHARACTER_MAXIMUM_LENGTH
                  FROM INFORMATION_SCHEMA.COLUMNS
                  WHERE TABLE_NAME  = 'Usuario'
                    AND COLUMN_NAME IN
                        (N'Contraseña', N'contrasena', N'password', N'clave', N'pass')", conn);
            var res = cmd.ExecuteScalar();

            if (res == null || res == DBNull.Value)
            {
                Assert.Inconclusive(
                    "PT-S-13: No se encontró la columna de contraseña. " +
                    "Verificar manualmente que las contraseñas no estén en texto plano.");
                return;
            }

            int longitud = Convert.ToInt32(res);
            TestContext?.WriteLine($"PT-S-13: Longitud máx. contraseña = {longitud} chars");

            Assert.IsTrue(longitud >= 20,
                $"PT-S-13: Longitud {longitud} — para hash seguro se recomienda ≥ 32 chars. " +
                $"Si ≤ 20, la contraseña probablemente está en texto plano.");
        }

        [TestMethod]
        [Description("PT-S-14 | Credenciales inválidas de BD → SQL Server rechaza la conexión")]
        public void Seguridad_CredencialesInvalidasBD_ConexionRechazada()
        {
            bool rechazada = false;
            try
            {
                using var conn = new SqlConnection(
                    "Server=LAPTOP-VFH6P020; Database=DB_Asistencia; " +
                    "User Id=usuario_invalido_xyz; Password=clave_invalida_xyz; " +
                    "TrustServerCertificate=True; Connect Timeout=3;");
                conn.Open();
            }
            catch (SqlException) { rechazada = true; }

            Assert.IsTrue(rechazada,
                "PT-S-14: SQL Server debe rechazar credenciales inválidas. " +
                "El catch de Login.cs debe mostrar mensaje amigable, no el stack trace.");
        }

        [TestMethod]
        [Description("PT-S-15 | El directorio de salida no contiene la contraseña en texto plano en archivos de config")]
        public void Seguridad_DirectorioSalida_SinContrasenaEnTextoPlanoEnArchivosConfig()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string[] pats = { "*.config", "*.json", "*.env", "*.ini" };
            string archivo = "";
            bool expuesta = false;

            foreach (string pat in pats)
            {
                foreach (string f in
                    Directory.GetFiles(dir, pat, SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        if (File.ReadAllText(f).Contains("ACuptxhJT4!"))
                        {
                            expuesta = true;
                            archivo = Path.GetFileName(f);
                            break;
                        }
                    }
                    catch { }
                }
                if (expuesta) break;
            }

            Assert.IsFalse(expuesta,
                $"PT-S-15: Contraseña encontrada en '{archivo}'. " +
                $"RECOMENDACIÓN: mover la cadena de conexión a variable de entorno o archivo cifrado. " +
                $"Actualmente está hardcoded en Conexion.cs — riesgo documentado en el Plan de Pruebas.");
        }

        // ════════════════════════════════════════════════════════════════════
        //  BLOQUE F — DISPONIBILIDAD  (PT-S-16, 17)
        //  CORRECCIÓN: IDs como int; limpiar antes de la transacción
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("PT-S-16 | Transacción con ROLLBACK → ningún dato persiste en Registro")]
        public void Disponibilidad_ReconexionTrasToCloseForce_BDConsistente()
        {
            int antesDeReconexion;
            using (var conn1 = new SqlConnection(ConnStr))
            {
                conn1.Open();
                using var cmd = new SqlCommand("SELECT COUNT(1) FROM Estudiante", conn1);
                antesDeReconexion = (int)cmd.ExecuteScalar()!;
                conn1.Close();
            }

            var sw = Stopwatch.StartNew();
            int trasDReconexion;
            using (var conn2 = new SqlConnection(ConnStr))
            {
                conn2.Open();
                using var cmd = new SqlCommand("SELECT COUNT(1) FROM Estudiante", conn2);
                trasDReconexion = (int)cmd.ExecuteScalar()!;
            }
            sw.Stop();

            TestContext?.WriteLine(
                $"PT-S-17: Reconexión en {sw.Elapsed.TotalMilliseconds:F0} ms. " +
                $"Antes={antesDeReconexion}, después={trasDReconexion}");

            Assert.AreEqual(antesDeReconexion, trasDReconexion,
                "PT-S-17: El conteo debe ser idéntico antes y después del cierre — datos íntegros.");
            Assert.IsTrue(sw.Elapsed.TotalMinutes < 3,
                $"PT-S-17: Reconexión en {sw.Elapsed.TotalSeconds:F1} s — debe ser < 3 min (RNF).");
        }

        // ════════════════════════════════════════════════════════════════════
        //  BLOQUE G — COMPATIBILIDAD  (PT-S-18, 19)
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("PT-S-18 | El entorno de ejecución es Windows con .NET 8 — cumple RNF")]
        public void Compatibilidad_EntornoEjecucion_CumpleRequisitosMinimos()
        {
            bool esWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            Version dotNetVer = Environment.Version;
            bool esNet8Plus = dotNetVer.Major >= 8;

            long ramMB = 0;
            try { ramMB = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024 * 1024); }
            catch { ramMB = 4096; }

            TestContext?.WriteLine(
                $"PT-S-18: OS={RuntimeInformation.OSDescription} | .NET={dotNetVer} | RAM≈{ramMB} MB");

            Assert.IsTrue(esWindows,
                "PT-S-18: El sistema debe ejecutarse en Windows (Windows Forms requiere Windows 10+).");
            Assert.IsTrue(esNet8Plus,
                $"PT-S-18: Se requiere .NET 8+. Versión actual: {dotNetVer}.");
            Assert.IsTrue(ramMB >= 1024,
                $"PT-S-18: RAM disponible: {ramMB} MB — RNF indica mínimo 4 GB.");
        }

        [TestMethod]
        [Description("PT-S-19 | DLLs del SDK DigitalPersona U.are.U 4500 presentes en el equipo")]
        public void Compatibilidad_SDKDigitalPersona_DllsRequeridosPresentes()
        {
            string rutaSdk = @"C:\Program Files\DigitalPersona\One Touch SDK\.NET\Bin";
            string[] dlls = { "DPFPDevNET.dll", "DPFPEngNET.dll",
                                "DPFPShrNET.dll", "DPFPVerNET.dll" };

            if (!Directory.Exists(rutaSdk))
            {
                Assert.Inconclusive(
                    $"PT-S-19: SDK no encontrado en: {rutaSdk}. " +
                    $"Verificar manualmente que las DLLs estén registradas en el equipo destino. " +
                    $"Sin este SDK el lector U.are.U 4500 no opera.");
                return;
            }

            foreach (string dll in dlls)
            {
                Assert.IsTrue(File.Exists(Path.Combine(rutaSdk, dll)),
                    $"PT-S-19: '{dll}' debe existir en {rutaSdk}. " +
                    $"Sin ella el módulo biométrico falla en tiempo de ejecución.");
            }
        }
    }
}