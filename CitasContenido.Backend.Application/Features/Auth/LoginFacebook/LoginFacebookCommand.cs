using CitasContenido.Backend.Domain.DTOs.Auth;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.LoginFacebook
{
    public record LoginFacebookCommand(string FacebookToken) : IRequest<Result<AuthResponseDto>>;
}
