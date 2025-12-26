using CitasContenido.Shared.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CitasContenido.Backend.Application.Features.Auth.VerificarIdentidad
{
    public record VerificarIdentidadCommand(
         long UsuarioId,
         IFormFile? FotoEnVivo,
         IFormFile FotoDocumento,
         decimal Latitud,
         decimal Longitud
     ) : IRequest<Result<string>>;
}
