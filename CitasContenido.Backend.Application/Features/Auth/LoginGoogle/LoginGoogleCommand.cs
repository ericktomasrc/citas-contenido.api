using CitasContenido.Backend.Domain.DTOs.Auth;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.LoginGoogle
{
    public record LoginGoogleCommand(string GoogleToken) : IRequest<Result<AuthResponseDto>>;
}
