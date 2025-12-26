using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;

namespace CitasContenido.Backend.Domain.Repositories
{
    public interface IPasswordResetTokenRepository
    {
        Task<PasswordResetToken?> ObtenerPorEmailYCodigoAsync(string email, string codigo);
        Task<PasswordResetToken?> ObtenerUltimoTokenValidoAsync(string email);
        Task<long> CrearAsync(PasswordResetToken token, IUnitOfWork unitOfWork);
        Task ActualizarAsync(PasswordResetToken token, IUnitOfWork unitOfWork);
        Task InvalidarTodosLosTokensDelUsuarioAsync(long usuarioId, IUnitOfWork unitOfWork);
        Task LimpiarTokensExpiradosAsync();
    }
}
