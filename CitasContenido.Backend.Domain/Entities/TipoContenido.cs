namespace CitasContenido.Backend.Domain.Entities
{
    public class TipoContenido
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
        public string? Descripcion { get; private set; }
        public bool SoloCreadores { get; private set; }
        public bool RequiereVerificacion { get; private set; }
        public int? LimiteArchivos { get; private set; }
        public int? TamañoMaximoMB { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public string UsuarioCreacion { get; private set; } = string.Empty;
        public DateTime? FechaModificacion { get; private set; }
        public string? UsuarioModificacion { get; private set; }
        public bool Habilitado { get; private set; }

        private TipoContenido() { }

        public static TipoContenido Crear(
            string nombre,
            string? descripcion,
            bool soloCreadores,
            bool requiereVerificacion,
            int? limiteArchivos,
            int? tamañoMaximoMB,
            string usuarioCreacion)
        {
            return new TipoContenido
            {
                Nombre = nombre,
                Descripcion = descripcion,
                SoloCreadores = soloCreadores,
                RequiereVerificacion = requiereVerificacion,
                LimiteArchivos = limiteArchivos,
                TamañoMaximoMB = tamañoMaximoMB,
                UsuarioCreacion = usuarioCreacion,
                FechaCreacion = DateTime.UtcNow,
                Habilitado = true
            };
        }
    }
}
