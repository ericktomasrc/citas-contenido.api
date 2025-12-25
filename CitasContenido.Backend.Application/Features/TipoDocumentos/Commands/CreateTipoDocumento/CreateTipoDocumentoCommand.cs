using CitasContenido.Backend.Domain.DTOs;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.TipoDocumentos.Commands.CreateTipoDocumento
{
    public record CreateTipoDocumentoCommand(
        string Name,
        string? Descripcion,
        string UsuarioCreacion
    ) : IRequest<Result<TipoDocumentoDto>>;
}
