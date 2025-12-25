using CitasContenido.Shared.Results;

namespace CitasContenido.Backend.Domain.Services
{
    public interface IRegistrarEmailDomainService
    {
        Task<Result<string>> RegistrarEmailAsync(string email);
        Task<Result<string>> ReenviarEmailVerificacionAsync(string email);
    }
}
