using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.Commands.ReenviarEmailVerificacion
{
    public record ReenviarEmailVerificacionCommand(string Email) : IRequest<Result<string>>;
}
