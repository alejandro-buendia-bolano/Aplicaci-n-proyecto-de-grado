// Archivo: Aplicacion/ValidadorCampos.cs
// PEGA ESTE CÓDIGO COMPLETO — reemplaza el contenido que Visual Studio genera automáticamente

namespace Aplicacion
{
    /// <summary>
    /// Centraliza todas las reglas de validación del sistema.
    /// Al estar separadas del formulario, pueden probarse de forma aislada (unitariamente).
    /// </summary>
    public static class ValidadorCampos
    {
        // ────────────────────────────────────────────────────────────────────────
        //  LOGIN  (refleja la lógica de Login.cs → button4_Click_1)
        // ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Retorna true solo si usuario Y contraseña contienen texto.
        /// </summary>
        public static bool ValidarCamposLogin(string usuario, string contrasena)
        {
            return !string.IsNullOrEmpty(usuario) && !string.IsNullOrEmpty(contrasena);
        }

        // ────────────────────────────────────────────────────────────────────────
        //  ESTUDIANTE  (refleja administrarEstudiantes.cs → agregar_Click_1)
        // ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Retorna true si los cuatro campos obligatorios del Estudiante tienen contenido.
        /// segundoNombre NO se valida aquí porque la BD permite NULL en esa columna.
        /// </summary>
        public static bool ValidarCamposEstudiante(
            string idEstudiante,
            string primerNombre,
            string primerApellido,
            string segundoApellido)
        {
            return !string.IsNullOrEmpty(idEstudiante)
                && !string.IsNullOrEmpty(primerNombre)
                && !string.IsNullOrEmpty(primerApellido)
                && !string.IsNullOrEmpty(segundoApellido);
        }

        // ────────────────────────────────────────────────────────────────────────
        //  DOCENTE  (refleja administrarDocentes.cs → agregar_Click)
        // ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Retorna true si los cuatro campos obligatorios del Docente tienen contenido.
        /// </summary>
        public static bool ValidarCamposDocente(
            string idDocente,
            string primerNombre,
            string primerApellido,
            string area)
        {
            return !string.IsNullOrEmpty(idDocente)
                && !string.IsNullOrEmpty(primerNombre)
                && !string.IsNullOrEmpty(primerApellido)
                && !string.IsNullOrEmpty(area);
        }

        // ────────────────────────────────────────────────────────────────────────
        //  FORMATO DE ID  (regla compartida por Estudiante y Docente)
        // ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Retorna true si el ID es numérico puro y tiene al menos 4 dígitos.
        /// Cubre el caso límite de IDs cortos o con caracteres especiales.
        /// </summary>
        public static bool ValidarFormatoId(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Trim().Length < 4)
                return false;

            foreach (char c in id.Trim())
                if (!char.IsDigit(c))
                    return false;

            return true;
        }

        // ────────────────────────────────────────────────────────────────────────
        //  RANGO DE FECHAS  (refleja lógica de generarReporteEstudiantes.cs)
        // ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Retorna true si la fecha de inicio es anterior o igual a la de fin.
        /// Evita generar reportes con rangos invertidos.
        /// </summary>
        public static bool ValidarRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            return fechaInicio <= fechaFin;
        }

        // ────────────────────────────────────────────────────────────────────────
        //  HUELLA BIOMÉTRICA  (refleja lógica de GuardarHuellaEnBD en ambos módulos)
        // ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Retorna true si el byte[] de la plantilla biométrica tiene datos.
        /// Un array null o vacío indica que la huella no fue capturada correctamente.
        /// </summary>
        public static bool ValidarPlantillaBiometrica(byte[] plantilla)
        {
            return plantilla != null && plantilla.Length > 0;
        }

        // ────────────────────────────────────────────────────────────────────────
        //  RUTA DE REPORTE PDF  (refleja lógica de generarReporteEstudiantes.cs)
        // ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Retorna true si la ruta tiene extensión .pdf y el directorio padre existe.
        /// </summary>
        public static bool ValidarRutaPdf(string rutaArchivo)
        {
            if (string.IsNullOrEmpty(rutaArchivo))
                return false;

            if (!rutaArchivo.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return false;

            string? directorio = Path.GetDirectoryName(rutaArchivo);
            return !string.IsNullOrEmpty(directorio) && Directory.Exists(directorio);
        }
    }
}