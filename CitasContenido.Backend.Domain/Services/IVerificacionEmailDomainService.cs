using CitasContenido.Shared.Results;

namespace CitasContenido.Backend.Domain.Services
{
    public interface IVerificacionEmailDomainService
    {
        Task<Result<long>> VerificarEmailAsync(string token);
    }
}
