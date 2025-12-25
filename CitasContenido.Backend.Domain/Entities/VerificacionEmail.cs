namespace CitasContenido.Backend.Domain.Entities
{
    public class VerificacionEmail
    {
        public long Id { get; private set; }
        public long UsuarioId { get; private set; }
        public string Token { get; private set; }
        public DateTime FechaExpiracion { get; private set; }
        public bool Verificado { get; private set; }
        public DateTime? FechaVerificacion { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        private VerificacionEmail() { }

        public static VerificacionEmail Crear(long usuarioId, string token, int horasValidez = 24)
        {
            return new VerificacionEmail
            { 
                UsuarioId = usuarioId,
                Token = token,
                FechaExpiracion = DateTime.UtcNow.AddHours(horasValidez),
                Verificado = false,
                FechaCreacion = DateTime.UtcNow,
                FechaVerificacion = DateTime.UtcNow
            };
        }

        public void Verificar()
        {
            if (EstaExpirado())
                throw new InvalidOperationException("El token ha expirado");

            Verificado = true;
            FechaVerificacion = DateTime.UtcNow;
        }

        public bool EstaExpirado()
        {
            return DateTime.UtcNow > FechaExpiracion;
        }
    }
}
