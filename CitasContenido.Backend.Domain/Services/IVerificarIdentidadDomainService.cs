using CitasContenido.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace CitasContenido.Backend.Domain.Services
{
    public interface IVerificarIdentidadDomainService
    {
        Task<Result<string>> VerificarIdentidadAsync(
            long usuarioId,
            IFormFile? fotoPerfil,
            IFormFile fotoDocumento,
            decimal latitud,
            decimal longitud);
    }
}
