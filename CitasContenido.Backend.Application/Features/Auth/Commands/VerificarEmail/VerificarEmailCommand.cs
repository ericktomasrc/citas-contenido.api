using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.Commands.VerificarEmail
{
    public record VerificarEmailCommand(string Token) : IRequest<Result<long>>;
}
