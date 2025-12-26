using CitasContenido.Shared.Results;

namespace CitasContenido.Backend.Domain.Services
{
    public interface IPasswordResetDomainService
    {
        Task<Result<string>> SolicitarRecuperacionPasswordAsync(
            string email,
            string? ipAddress = null,
            string? userAgent = null);

        Task<Result<bool>> VerificarCodigoAsync(string email, string codigo);

        Task<Result<string>> RestablecerPasswordAsync(
            string email,
            string codigo,
            string nuevaPassword,
            string confirmarPassword);
    }
}
