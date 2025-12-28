using CitasContenido.Backend.Domain.DTOs.Auth;
using CitasContenido.Shared.Results;

namespace CitasContenido.Backend.Domain.Services
{
    public interface IOAuthDomainService
    {
        Task<Result<AuthResponseDto>> LoginConGoogleAsync(string googleToken);
        Task<Result<AuthResponseDto>> LoginConFacebookAsync(string facebookToken);
    }
}
