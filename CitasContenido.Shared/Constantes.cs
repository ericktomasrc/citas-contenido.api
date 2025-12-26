using System.Text.RegularExpressions;

namespace Smr.Backend.Shared
{
    public static class Constantes
    {
        public static readonly int DURACION_TOKEN = 7;
        public static readonly string MINUTOS_EXPIRACION_PASSWORD = "30";
        public static readonly string BLOB_STORAGE_CONTAINERNAME = "citascontenido";
        public static readonly string SYSTEM = "SYSTEM";
        public static readonly string DESCONOCIDO_PAIS = "DESCONOCIDO PAIS";
        public static readonly string DESCONOCIDO_DEPARTAMENTO = "DESCONOCIDO DEPARTAMENTO";
        public static readonly string DESCONOCIDO_PROVINCIA = "DESCONOCIDO PROVINCIA";
        public static readonly string DESCONOCIDO_DISTRITO = "DESCONOCIDO DISTINTO";
        public static readonly string DESCONOCIDO_CIUDAD = "DESCONOCIDO CIUDAD";
        public static readonly string DESCONOCIDO_DIRECCION_COMPLETA = "DESCONOCIDO DIRECCION COMPLETA";
        public static readonly string MENSAJE_ERROR_USUARIO = "Error al actualizar usuario: ";


        public static readonly string MENSAJE_NO_CONTROLADO_SERVICES = "Sucedio un error inesperado, contacte con el responsable de la aplicacion";
        public static readonly string DanioYMermaCliente = "Daño y merma";
        public static readonly string Subarriendo = "Subarriendo";
        public static readonly string[] CodigosHijosOS = { "ALQL", "SUBA", "DYM" };
        public static readonly string DateFormatYyyyMMdd = "yyyyMMdd";
        public static readonly string DateFormatYyyy_MM_dd = "yyyy-MM-dd";
        public static readonly string DateFormatYyyy_S_MM_S_dd = "dd/MM/yyyy";
        public static readonly string FechaInvalida = "Fecha inválida.";
        public static readonly string NumeroInvalidoMayorOIgualCero = "Debe ser un número correcto mayor o igual a 0.";
        public static readonly string Param_Modelo = "Modelo"; 
        public static readonly string Param_Serie = "Serie";
        public static readonly string Param_Horometro = "Horometro";
        public static readonly string Param_Fecha = "Fecha";
        public static readonly string Param_CreatedAt = "CreatedAt";
        public static readonly string Param_CreatedBy = "CreatedBy";

        public static readonly Regex MesCobroRegex = new(
                       @"^(0[1-9]|1[0-2])\s*-\s*(Enero|Febrero|Marzo|Abril|Mayo|Junio|Julio|Agosto|Septiembre|Setiembre|Octubre|Noviembre|Diciembre)$",
                       RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled
                         );

        public static readonly string[] Meses = {
                                "", "Enero","Febrero","Marzo","Abril","Mayo","Junio",
                                "Julio","Agosto","Septiembre","Octubre","Noviembre","Diciembre"
                                 };

        //Validacion de cabeceras de formatos
        public static readonly string[] CABECERAS_INGRESO_RENTA = new string[]
        {
            "Codigo Cliente",
            "Cliente",
            "Frente (Proyecto)",
            "Serie SAP",
            "Material SAP",
            "Año",
            "Mes",
            "Moneda",
            "Ingreso de renta"
        };

        public static readonly string[] CABECERAS_VENTA_EQUIPO = new string[]
        {
            "gl_sirid",
            "Doc. Referencia",
            "Cod. Cliente",
            "Razon Social",
            "Lote",
            "Material",
            "Moneda Sociedad",
        };
        public static readonly string CONFIGURACION_CRITERIO_MODELO = "Modelo";
        public static readonly string CONFIGURACION_CRITERIO_CLIENTE = "Cliente";
        public const string MensajeProcesadoCorrectamente = "Se ha procesado correctamente la información";
    }
}
