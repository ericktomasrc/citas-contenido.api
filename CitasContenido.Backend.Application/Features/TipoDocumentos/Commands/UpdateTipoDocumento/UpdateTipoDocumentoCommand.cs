using CitasContenido.Backend.Domain.DTOs;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.TipoDocumentos.Commands.UpdateTipoDocumento
{
    public record UpdateTipoDocumentoCommand(
        int Id,
        string Name,
        string? Descripcion,
        string UsuarioModificacion
    ) : IRequest<Result<TipoDocumentoDto>>;
}
