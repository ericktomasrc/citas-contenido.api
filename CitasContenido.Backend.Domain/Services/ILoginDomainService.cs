using CitasContenido.Backend.Domain.DTOs.Auth;
using CitasContenido.Shared.Results;

namespace CitasContenido.Backend.Domain.Services
{
    public interface ILoginDomainService
    {
        Task<Result<AuthResponseDto>> LoginAsync(string email, string password);
    }
}
