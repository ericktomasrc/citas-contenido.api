using CitasContenido.Backend.Domain.DTOs;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.TipoDocumentos.Queries.GetTipoDocumentos
{
    public record GetTipoDocumentosQuery(
        string? Filtro,
        bool? SoloHabilitados
    ) : IRequest<Result<List<TipoDocumentoDto>>>;
}
