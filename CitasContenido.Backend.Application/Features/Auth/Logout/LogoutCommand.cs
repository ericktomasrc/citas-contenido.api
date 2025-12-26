using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.Logout
{
    public record LogoutCommand(long UsuarioId) : IRequest<Result<string>>;
}
