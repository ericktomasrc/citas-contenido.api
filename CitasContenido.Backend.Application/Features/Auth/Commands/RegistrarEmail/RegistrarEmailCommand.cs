using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.Commands.RegistrarEmail
{
    public record RegistrarEmailCommand(string Email) : IRequest<Result<string>>;
}
