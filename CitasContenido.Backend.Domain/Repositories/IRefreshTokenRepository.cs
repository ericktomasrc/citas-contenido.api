using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;

namespace CitasContenido.Backend.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefrescarToken?> ObtenerPorTokenAsync(string token);
        Task<Guid> CrearAsync(RefrescarToken refreshToken, IUnitOfWork? unitOfWork = null);
        Task ActualizarAsync(RefrescarToken refreshToken, IUnitOfWork? unitOfWork = null);
        Task RevocarTodosPorUsuarioAsync(long usuarioId);
    }
}
