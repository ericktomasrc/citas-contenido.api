using CitasContenido.Backend.Domain.DTOs.Auth;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponseDto>>;
}
