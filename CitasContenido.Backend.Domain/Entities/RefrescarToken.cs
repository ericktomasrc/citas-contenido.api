namespace CitasContenido.Backend.Domain.Entities
{
    public class RefrescarToken
    {
        public long Id { get; private set; }
        public long UsuarioId { get; private set; }
        public string Token { get; private set; }
        public DateTime FechaExpiracion { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public bool Revocado { get; private set; }
        public DateTime? FechaRevocacion { get; private set; }
        public string? ReemplazadoPor { get; private set; } 

        private RefrescarToken() { }

        public static RefrescarToken Crear(long usuarioId, string token, int diasValidez)
        {
            return new RefrescarToken
            { 
                UsuarioId = usuarioId,
                Token = token,
                FechaExpiracion = DateTime.UtcNow.AddDays(diasValidez),
                FechaCreacion = DateTime.UtcNow, 
                Revocado = false
            };
        }

        public void Revocar(string? reemplazadoPor = null)
        {
            Revocado = true;
            FechaRevocacion = DateTime.UtcNow;
            ReemplazadoPor = reemplazadoPor;
        }

        public bool EstaActivo()
        {
            return !Revocado && DateTime.UtcNow < FechaExpiracion;
        }
    }
}
