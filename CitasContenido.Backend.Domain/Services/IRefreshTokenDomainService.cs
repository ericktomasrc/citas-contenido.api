using CitasContenido.Backend.Domain.DTOs.Auth;
using CitasContenido.Shared.Results;

namespace CitasContenido.Backend.Domain.Services
{
    public interface IRefreshTokenDomainService
    {
        Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
    }
}
