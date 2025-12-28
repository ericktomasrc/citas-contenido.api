using CitasContenido.Backend.Domain.Services;
using System.Security.Cryptography;

namespace CitasContenido.Backend.Infraestructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100000; // Cambiado de 10000 a 100000

        public string HashPassword(string password)
        {
            using var algorithm = new Rfc2898DeriveBytes(
                password,
                SaltSize,
                Iterations,
                HashAlgorithmName.SHA256);

            var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
            var salt = Convert.ToBase64String(algorithm.Salt);

            return $"{Iterations}.{salt}.{key}";
        }

        public bool VerifyPassword(string password, string hash)
        {
            // Detectar si es un hash BCrypt (formato legacy)
            if (hash.StartsWith("$2a$") || hash.StartsWith("$2b$") || hash.StartsWith("$2y$"))
            {
                return VerifyBCryptPassword(password, hash);
            }

            // Verificación PBKDF2 (formato actual)
            var parts = hash.Split('.', 3);

            if (parts.Length != 3)
                return false;

            if (!int.TryParse(parts[0], out var iterations))
                return false;

            try
            {
                var salt = Convert.FromBase64String(parts[1]);
                var key = Convert.FromBase64String(parts[2]);

                using var algorithm = new Rfc2898DeriveBytes(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256);

                var keyToCheck = algorithm.GetBytes(KeySize);

                return keyToCheck.SequenceEqual(key);
            }
            catch
            {
                return false;
            }
        }

        private bool VerifyBCryptPassword(string password, string hash)
        {
            try
            {
                // Usar BCrypt.Net-Next (instalar: Install-Package BCrypt.Net-Next)
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}