namespace CitasContenido.Backend.Domain.Entities
{
    public class VerificacionIdentidad
    {
        public long Id { get; private set; }
        public long UsuarioId { get; private set; }
        public string? FotoPerfilUrl { get; private set; }
        public string FotoDocumentoUrl { get; private set; }
        public int EstadoVerificacion { get; private set; } // 0: Pendiente, 1: Aprobado, 2: Rechazado
        public string? MotivoRechazo { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public DateTime? FechaVerificacion { get; private set; }
        public Guid? VerificadoPor { get; private set; }

        private VerificacionIdentidad() { }

        public static VerificacionIdentidad Crear(
            long usuarioId,
            string? fotoPerfilUrl,
            string fotoDocumentoUrl)
        {
            if (string.IsNullOrWhiteSpace(fotoDocumentoUrl))
                throw new ArgumentException("La foto del documento es requerida");

            return new VerificacionIdentidad
            { 
                UsuarioId = usuarioId,
                FotoPerfilUrl = fotoPerfilUrl,
                FotoDocumentoUrl = fotoDocumentoUrl,
                EstadoVerificacion = 0, // Pendiente
                FechaCreacion = DateTime.UtcNow
            };
        }

        public void Aprobar(Guid verificadoPor)
        {
            EstadoVerificacion = 1; // Aprobado
            FechaVerificacion = DateTime.UtcNow;
            VerificadoPor = verificadoPor;
            MotivoRechazo = null;
        }

        public void Rechazar(string motivo, Guid verificadoPor)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("El motivo de rechazo es requerido");

            EstadoVerificacion = 2; // Rechazado
            MotivoRechazo = motivo;
            FechaVerificacion = DateTime.UtcNow;
            VerificadoPor = verificadoPor;
        }

        public bool EstaPendiente() => EstadoVerificacion == 0;
        public bool EstaAprobado() => EstadoVerificacion == 1;
        public bool EstaRechazado() => EstadoVerificacion == 2;
    }
}
