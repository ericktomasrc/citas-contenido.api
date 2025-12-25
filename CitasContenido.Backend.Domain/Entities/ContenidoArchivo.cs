namespace CitasContenido.Backend.Domain.Entities
{
    public class ContenidoArchivo
    {
        public long Id { get; private set; }
        public long UsuarioId { get; private set; }
        public int TipoContenidoId { get; private set; }
        public int TipoArchivoId { get; private set; }
        public bool EsPrincipal { get; private set; }
        public bool EstadoVerificacion { get; private set; }
        public bool EsPublico { get; private set; }
        public int Orden { get; private set; }
        public string NombreArchivo { get; private set; } = string.Empty;
        public long TamanioArchivoMB { get; private set; }
        public string Extension { get; private set; } = string.Empty;
        public string BlobKey { get; private set; } = string.Empty;
        public string BlobUrl { get; private set; } = string.Empty;
        public string ContainerName { get; private set; } = string.Empty;
        public DateTime FechaCreacion { get; private set; }
        public string UsuarioCreacion { get; private set; } = string.Empty;

        private ContenidoArchivo() { }

        public static ContenidoArchivo Crear(
            long usuarioId,
            int tipoContenidoId,
            int tipoArchivoId,
            string nombreArchivo,
            long TamanioArchivoMB,
            string extension,
            string blobKey,
            string blobUrl,
            string containerName,
            bool esPrincipal,
            bool esPublico,
            int orden,
            string usuarioCreacion)
        {
            return new ContenidoArchivo
            {
                UsuarioId = usuarioId,
                TipoContenidoId = tipoContenidoId,
                TipoArchivoId = tipoArchivoId,
                NombreArchivo = nombreArchivo,
                TamanioArchivoMB = TamanioArchivoMB,
                Extension = extension,
                BlobKey = blobKey,
                BlobUrl = blobUrl,
                ContainerName = containerName,
                EsPrincipal = esPrincipal,
                EstadoVerificacion = false, // Por defecto no verificado
                EsPublico = esPublico,
                Orden = orden,
                UsuarioCreacion = usuarioCreacion,
                FechaCreacion = DateTime.UtcNow
            };
        }

        public void MarcarComoPrincipal()
        {
            EsPrincipal = true;
        }

        public void QuitarDePrincipal()
        {
            EsPrincipal = false;
        }

        public void AprobarVerificacion()
        {
            EstadoVerificacion = true;
        }
    }
}
