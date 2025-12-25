using CitasContenido.Backend.Domain.DTOs;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.TipoDocumentos.Commands.DeleteTipoDocumento
{

    public record GetTipoDocumentosQuery(
        string? Filtro,
        bool? SoloHabilitados
    ) : IRequest<Result<List<TipoDocumentoDto>>>;
}
