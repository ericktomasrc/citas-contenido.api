using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.TipoDocumentos.Commands.DeleteTipoDocumento
{
    public record DeleteTipoDocumentoCommand(int Id) : IRequest<Result>;
}
