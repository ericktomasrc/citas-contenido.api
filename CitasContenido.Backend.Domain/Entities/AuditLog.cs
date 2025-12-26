using System.Text.Json;

namespace CitasContenido.Backend.Domain.Entities
{
    public class AuditLog
    {
        public long Id { get; private set; }
        public long? UsuarioId { get; private set; }
        public string Accion { get; private set; }
        public string? Entidad { get; private set; }
        public long? EntidadId { get; private set; }
        public string? DetallesAntes { get; private set; }
        public string? DetallesDespues { get; private set; }
        public string? IpAddress { get; private set; }
        public string? UserAgent { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        private AuditLog() { }

        public static AuditLog Crear(
            string accion,
            long? usuarioId = null,
            string? entidad = null,
            long? entidadId = null,
            object? detallesAntes = null,
            object? detallesDespues = null,
            string? ipAddress = null,
            string? userAgent = null)
        {
            return new AuditLog
            {
                Accion = accion,
                UsuarioId = usuarioId,
                Entidad = entidad,
                EntidadId = entidadId,
                DetallesAntes = detallesAntes != null ? JsonSerializer.Serialize(detallesAntes) : null,
                DetallesDespues = detallesDespues != null ? JsonSerializer.Serialize(detallesDespues) : null,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                FechaCreacion = DateTime.UtcNow
            };
        }
    }
}
