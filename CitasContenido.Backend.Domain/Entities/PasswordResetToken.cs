namespace CitasContenido.Backend.Domain.Entities
{
    public class PasswordResetToken
    {
        public long Id { get; private set; }
        public long UsuarioId { get; private set; }
        public string Email { get; private set; }
        public string Token { get; private set; } // Hash del código
        public string TokenPlainText { get; private set; } // Código en texto plano
        public DateTime FechaCreacion { get; private set; }
        public DateTime FechaExpiracion { get; private set; }
        public bool Usado { get; private set; }
        public DateTime? FechaUso { get; private set; }
        public string? IpAddress { get; private set; }
        public string? UserAgent { get; private set; }

        // Constructor privado para EF Core
        private PasswordResetToken() { }

        // Factory method
        public static PasswordResetToken Crear(
            long usuarioId,
            string email,
            string codigo, // Código de 6 dígitos en texto plano
            int minutosExpiracion = 30,
            string? ipAddress = null,
            string? userAgent = null)
        {
            if (string.IsNullOrWhiteSpace(codigo) || codigo.Length != 6)
                throw new ArgumentException("El código debe tener 6 dígitos", nameof(codigo));

            return new PasswordResetToken
            {
                UsuarioId = usuarioId,
                Email = email.ToLower().Trim(),
                Token = BCrypt.Net.BCrypt.HashPassword(codigo), // Hash del código
                TokenPlainText = codigo, // Texto plano (solo para enviar por email)
                FechaCreacion = DateTime.UtcNow,
                FechaExpiracion = DateTime.UtcNow.AddMinutes(minutosExpiracion),
                Usado = false,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };
        }

        // Verificar código
        public bool VerificarCodigo(string codigo)
        {
            return BCrypt.Net.BCrypt.Verify(codigo, Token);
        }

        // Marcar como usado
        public void MarcarComoUsado()
        {
            Usado = true;
            FechaUso = DateTime.UtcNow;
        }

        // Verificar si está expirado
        public bool EstaExpirado()
        {
            return DateTime.UtcNow > FechaExpiracion;
        }

        // Verificar si es válido
        public bool EsValido()
        {
            return !Usado && !EstaExpirado();
        }
    }
}
