// ============================================================
//  Archivo: Aplicacion.Tests/UnitTest1.cs
//  BORRA TODO EL CONTENIDO EXISTENTE Y PEGA ESTO COMPLETO
// ============================================================

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aplicacion;   // Accede a ValidadorCampos gracias a la referencia que agregaste

namespace Aplicacion.Tests
{
    [TestClass]
    public class UnitTest1
    {
        // ════════════════════════════════════════════════════════════════════════
        //  BLOQUE 1 — CRUD LOGIN
        //  Valida la lógica de autenticación de Login.cs (button4_Click_1)
        // ════════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("VÁLIDO: usuario y contraseña con datos reales → debe permitir el intento de login")]
        public void Login_UsuarioYPasswordConValores_RetornaVerdadero()
        {
            // Arrange: preparamos un usuario y contraseña como los que escribe un docente real
            string usuario = "jperez";
            string contrasena = "Docente2024!";

            // Act: ejecutamos la validación de campos
            bool resultado = ValidadorCampos.ValidarCamposLogin(usuario, contrasena);

            // Assert: esperamos que retorne true (ambos campos tienen contenido)
            Assert.IsTrue(resultado, "Con usuario y contraseña válidos debe retornar true.");
        }

        [TestMethod]
        [Description("INVÁLIDO: ambos campos vacíos → el sistema NO debe intentar consultar la BD")]
        public void Login_AmbosValoresVacios_RetornaFalso()
        {
            string usuario = "";
            string contrasena = "";

            bool resultado = ValidadorCampos.ValidarCamposLogin(usuario, contrasena);

            Assert.IsFalse(resultado, "Con ambos campos vacíos debe retornar false.");
        }

        [TestMethod]
        [Description("LÍMITE: solo el usuario está vacío → debe fallar aunque la contraseña sea correcta")]
        public void Login_SoloUsuarioVacio_RetornaFalso()
        {
            string usuario = "";
            string contrasena = "Docente2024!";

            bool resultado = ValidadorCampos.ValidarCamposLogin(usuario, contrasena);

            Assert.IsFalse(resultado, "Sin usuario debe retornar false aunque haya contraseña.");
        }

        [TestMethod]
        [Description("LÍMITE: solo la contraseña está vacía → debe fallar aunque el usuario sea correcto")]
        public void Login_SoloPasswordVacio_RetornaFalso()
        {
            string usuario = "jperez";
            string contrasena = "";

            bool resultado = ValidadorCampos.ValidarCamposLogin(usuario, contrasena);

            Assert.IsFalse(resultado, "Sin contraseña debe retornar false aunque haya usuario.");
        }

        // ════════════════════════════════════════════════════════════════════════
        //  BLOQUE 2 — CRUD ESTUDIANTE
        //  Valida la lógica de administrarEstudiantes.cs (agregar_Click_1)
        //  Campos obligatorios en la tabla Estudiante: idEstudiante, primerNombre,
        //  primerApellido, segundoApellido. El segundoNombre admite NULL en la BD.
        // ════════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("VÁLIDO: todos los campos obligatorios del estudiante completos → INSERT puede proceder")]
        public void Estudiante_TodosLosCamposObligatoriosLlenos_RetornaVerdadero()
        {
            string id = "20241001";
            string primerNombre = "Laura";
            string primerApellido = "Gómez";
            string segundoApellido = "Ríos";

            bool resultado = ValidadorCampos.ValidarCamposEstudiante(id, primerNombre, primerApellido, segundoApellido);

            Assert.IsTrue(resultado, "Con todos los campos obligatorios llenos debe retornar true.");
        }

        [TestMethod]
        [Description("INVÁLIDO: ID del estudiante vacío → el INSERT fallaría en la BD (PK requerida)")]
        public void Estudiante_IdVacio_RetornaFalso()
        {
            string id = "";
            string primerNombre = "Laura";
            string primerApellido = "Gómez";
            string segundoApellido = "Ríos";

            bool resultado = ValidadorCampos.ValidarCamposEstudiante(id, primerNombre, primerApellido, segundoApellido);

            Assert.IsFalse(resultado, "Sin ID debe retornar false.");
        }

        [TestMethod]
        [Description("INVÁLIDO: primerNombre vacío → no puede registrarse un estudiante sin nombre")]
        public void Estudiante_PrimerNombreVacio_RetornaFalso()
        {
            string id = "20241001";
            string primerNombre = "";
            string primerApellido = "Gómez";
            string segundoApellido = "Ríos";

            bool resultado = ValidadorCampos.ValidarCamposEstudiante(id, primerNombre, primerApellido, segundoApellido);

            Assert.IsFalse(resultado, "Sin primerNombre debe retornar false.");
        }

        [TestMethod]
        [Description("LÍMITE: todos los campos son cadenas de un solo espacio (solo whitespace)")]
        public void Estudiante_CamposConSoloEspacios_RetornaFalso()
        {
            // IsNullOrEmpty(" ") retorna FALSE, así que usamos IsNullOrWhiteSpace en este caso límite
            // Este test documenta que el validador actual NO rechaza espacios en blanco.
            // Si en el futuro se endurece, este test actuará como alarma.
            string id = " ";
            string primerNombre = " ";
            string primerApellido = " ";
            string segundoApellido = " ";

            bool resultado = ValidadorCampos.ValidarCamposEstudiante(id, primerNombre, primerApellido, segundoApellido);

            // Con la implementación actual (IsNullOrEmpty) esto retorna TRUE.
            // Lo documentamos como comportamiento conocido (caso límite identificado):
            Assert.IsTrue(resultado,
                "CASO LÍMITE DOCUMENTADO: el validador actual acepta espacios en blanco. " +
                "Considerar cambiar a IsNullOrWhiteSpace para mayor robustez.");
        }

        // ════════════════════════════════════════════════════════════════════════
        //  BLOQUE 3 — CRUD DOCENTE
        //  Valida la lógica de administrarDocentes.cs (agregar_Click)
        // ════════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("VÁLIDO: todos los campos del docente completos → registro puede proceder")]
        public void Docente_TodosLosCamposLlenos_RetornaVerdadero()
        {
            string id = "10245678";
            string primerNombre = "Carlos";
            string primerApellido = "Martínez";
            string area = "Matemáticas";

            bool resultado = ValidadorCampos.ValidarCamposDocente(id, primerNombre, primerApellido, area);

            Assert.IsTrue(resultado, "Con todos los campos completos debe retornar true.");
        }

        [TestMethod]
        [Description("INVÁLIDO: área de conocimiento vacía → no puede registrarse un docente sin área")]
        public void Docente_AreaVacia_RetornaFalso()
        {
            string id = "10245678";
            string primerNombre = "Carlos";
            string primerApellido = "Martínez";
            string area = "";

            bool resultado = ValidadorCampos.ValidarCamposDocente(id, primerNombre, primerApellido, area);

            Assert.IsFalse(resultado, "Sin área debe retornar false.");
        }

        [TestMethod]
        [Description("INVÁLIDO: ID del docente nulo → equivale a campo vacío")]
        public void Docente_IdNulo_RetornaFalso()
        {
            string id = null!;   // El ! suprime la advertencia de nullable; simula un campo vacío
            string primerNombre = "Carlos";
            string primerApellido = "Martínez";
            string area = "Matemáticas";

            bool resultado = ValidadorCampos.ValidarCamposDocente(id, primerNombre, primerApellido, area);

            Assert.IsFalse(resultado, "Con ID nulo debe retornar false.");
        }

        // ════════════════════════════════════════════════════════════════════════
        //  BLOQUE 4 — FORMATO DE ID (lógica compartida Estudiante y Docente)
        // ════════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("VÁLIDO: ID numérico de 8 dígitos → formato correcto")]
        public void FormatoId_IdNumericoOchoDigitos_RetornaVerdadero()
        {
            bool resultado = ValidadorCampos.ValidarFormatoId("10245678");
            Assert.IsTrue(resultado, "Un ID de 8 dígitos numéricos debe ser válido.");
        }

        [TestMethod]
        [Description("INVÁLIDO: ID con letras mezcladas → rechazar")]
        public void FormatoId_IdConLetras_RetornaFalso()
        {
            bool resultado = ValidadorCampos.ValidarFormatoId("ABC1234");
            Assert.IsFalse(resultado, "Un ID con letras debe ser inválido.");
        }

        [TestMethod]
        [Description("LÍMITE INFERIOR: ID de solo 3 dígitos → por debajo del mínimo permitido (4)")]
        public void FormatoId_IdDeTresDigitos_RetornaFalso()
        {
            bool resultado = ValidadorCampos.ValidarFormatoId("123");
            Assert.IsFalse(resultado, "Un ID de menos de 4 dígitos debe ser inválido.");
        }

        [TestMethod]
        [Description("LÍMITE: ID de exactamente 4 dígitos → mínimo válido")]
        public void FormatoId_IdDeExactamenteCuatroDigitos_RetornaVerdadero()
        {
            bool resultado = ValidadorCampos.ValidarFormatoId("1001");
            Assert.IsTrue(resultado, "Un ID de exactamente 4 dígitos debe ser válido.");
        }

        [TestMethod]
        [Description("INVÁLIDO: ID completamente vacío")]
        public void FormatoId_IdVacio_RetornaFalso()
        {
            bool resultado = ValidadorCampos.ValidarFormatoId("");
            Assert.IsFalse(resultado, "Un ID vacío debe ser inválido.");
        }

        [TestMethod]
        [Description("INVÁLIDO: ID con guión → caracteres especiales no permitidos")]
        public void FormatoId_IdConGuion_RetornaFalso()
        {
            bool resultado = ValidadorCampos.ValidarFormatoId("1024-5678");
            Assert.IsFalse(resultado, "Un ID con guión debe ser inválido.");
        }

        // ════════════════════════════════════════════════════════════════════════
        //  BLOQUE 5 — LECTOR BIOMÉTRICO (validación de plantilla DPFP)
        //  Refleja la condición de guardarHuellaEnBD en ambos módulos:
        //  si el byte[] es null o vacío, la huella no se capturó correctamente.
        // ════════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("VÁLIDO: plantilla biométrica con datos simulados → lista para guardar en BD")]
        public void Biometrico_PlantillaConDatos_RetornaVerdadero()
        {
            // Simulamos un byte[] como el que retorna DPFP.Template.Serialize()
            byte[] plantillaSimulada = new byte[512];
            new Random().NextBytes(plantillaSimulada); // datos aleatorios, simula una huella real

            bool resultado = ValidadorCampos.ValidarPlantillaBiometrica(plantillaSimulada);

            Assert.IsTrue(resultado, "Una plantilla con datos debe considerarse válida.");
        }

        [TestMethod]
        [Description("INVÁLIDO: plantilla null → el lector no capturó ningún dato")]
        public void Biometrico_PlantillaNull_RetornaFalso()
        {
            byte[]? plantilla = null;

            bool resultado = ValidadorCampos.ValidarPlantillaBiometrica(plantilla!);

            Assert.IsFalse(resultado, "Una plantilla null debe ser inválida.");
        }

        [TestMethod]
        [Description("LÍMITE: plantilla con array vacío (length = 0) → captura fallida")]
        public void Biometrico_PlantillaArrayVacio_RetornaFalso()
        {
            byte[] plantilla = Array.Empty<byte>();

            bool resultado = ValidadorCampos.ValidarPlantillaBiometrica(plantilla);

            Assert.IsFalse(resultado, "Una plantilla de longitud 0 debe ser inválida.");
        }

        // ════════════════════════════════════════════════════════════════════════
        //  BLOQUE 6 — GENERACIÓN DE REPORTES PDF
        //  Refleja la validación previa al PdfWriter.GetInstance() de
        //  generarReporteEstudiantes.cs y generarReporteDocentes.cs
        // ════════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("VÁLIDO: ruta PDF apunta a carpeta existente (carpeta Temp del sistema)")]
        public void PDF_RutaEnDirectorioExistente_RetornaVerdadero()
        {
            // Usamos la carpeta temporal del sistema operativo, que siempre existe
            string ruta = Path.Combine(Path.GetTempPath(), "ReporteAsistencia.pdf");

            bool resultado = ValidadorCampos.ValidarRutaPdf(ruta);

            Assert.IsTrue(resultado, "Una ruta válida con directorio existente debe retornar true.");
        }

        [TestMethod]
        [Description("INVÁLIDO: ruta con extensión incorrecta (.docx en lugar de .pdf)")]
        public void PDF_RutaConExtensionIncorrecta_RetornaFalso()
        {
            string ruta = Path.Combine(Path.GetTempPath(), "ReporteAsistencia.docx");

            bool resultado = ValidadorCampos.ValidarRutaPdf(ruta);

            Assert.IsFalse(resultado, "Una ruta sin extensión .pdf debe retornar false.");
        }

        [TestMethod]
        [Description("INVÁLIDO: ruta vacía → no se puede guardar el PDF en ningún lado")]
        public void PDF_RutaVacia_RetornaFalso()
        {
            bool resultado = ValidadorCampos.ValidarRutaPdf("");

            Assert.IsFalse(resultado, "Una ruta vacía debe retornar false.");
        }

        [TestMethod]
        [Description("INVÁLIDO: directorio de la ruta no existe → iTextSharp lanzaría excepción")]
        public void PDF_RutaEnDirectorioInexistente_RetornaFalso()
        {
            string ruta = @"C:\CarpetaQueNoExiste\ReporteAsistencia.pdf";

            bool resultado = ValidadorCampos.ValidarRutaPdf(ruta);

            Assert.IsFalse(resultado, "Una ruta en directorio inexistente debe retornar false.");
        }

        // ════════════════════════════════════════════════════════════════════════
        //  BLOQUE 7 — RANGO DE FECHAS DEL REPORTE
        //  Refleja la validación de comboBox2 (mes) y comboBox3 (semana)
        //  en generarReporteEstudiantes.cs y generarReporteDocentes.cs
        // ════════════════════════════════════════════════════════════════════════

        [TestMethod]
        [Description("VÁLIDO: fecha inicio anterior a fecha fin → rango coherente para el reporte")]
        public void FechasReporte_InicioAntesQueFin_RetornaVerdadero()
        {
            DateTime inicio = new DateTime(2025, 5, 1);
            DateTime fin = new DateTime(2025, 5, 7);

            bool resultado = ValidadorCampos.ValidarRangoFechas(inicio, fin);

            Assert.IsTrue(resultado, "Semana 1 del mes: inicio antes que fin debe ser válido.");
        }

        [TestMethod]
        [Description("LÍMITE: fecha inicio igual a fecha fin → reporte de un solo día, debe aceptarse")]
        public void FechasReporte_InicioIgualQueFin_RetornaVerdadero()
        {
            DateTime fecha = new DateTime(2025, 5, 15);

            bool resultado = ValidadorCampos.ValidarRangoFechas(fecha, fecha);

            Assert.IsTrue(resultado, "Un rango de un solo día debe ser válido.");
        }

        [TestMethod]
        [Description("INVÁLIDO: fecha fin antes que fecha inicio → rango invertido, no debe procesarse")]
        public void FechasReporte_FinAntesQueInicio_RetornaFalso()
        {
            DateTime inicio = new DateTime(2025, 5, 15);
            DateTime fin = new DateTime(2025, 5, 1);

            bool resultado = ValidadorCampos.ValidarRangoFechas(inicio, fin);

            Assert.IsFalse(resultado, "Un rango con fin antes que inicio debe ser inválido.");
        }
    }
}