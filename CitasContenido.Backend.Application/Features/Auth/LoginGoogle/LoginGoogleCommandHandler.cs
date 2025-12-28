using CitasContenido.Backend.Domain.DTOs.Auth;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.LoginGoogle
{
    public class LoginGoogleCommandHandler : IRequestHandler<LoginGoogleCommand, Result<AuthResponseDto>>
    {
        private readonly IOAuthDomainService _oauthDomainService;

        public LoginGoogleCommandHandler(IOAuthDomainService oauthDomainService)
        {
            _oauthDomainService = oauthDomainService;
        }

        public async Task<Result<AuthResponseDto>> Handle(LoginGoogleCommand request, CancellationToken cancellationToken)
        {
            return await _oauthDomainService.LoginConGoogleAsync(request.GoogleToken);
        }
    }
}
