namespace CitasContenido.Backend.Domain.Entities
{
    public class PasswordHistory
    {
        public long Id { get; private set; }
        public long UsuarioId { get; private set; }
        public string PasswordHash { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        private PasswordHistory() { }

        public static PasswordHistory Crear(long usuarioId, string passwordHash)
        {
            return new PasswordHistory
            {
                UsuarioId = usuarioId,
                PasswordHash = passwordHash,
                FechaCreacion = DateTime.UtcNow
            };
        }
    }
}
