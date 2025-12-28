using CitasContenido.Backend.Domain.DTOs.Auth;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.LoginFacebook
{
    public class LoginFacebookCommandHandler : IRequestHandler<LoginFacebookCommand, Result<AuthResponseDto>>
    {
        private readonly IOAuthDomainService _oauthDomainService;

        public LoginFacebookCommandHandler(IOAuthDomainService oauthDomainService)
        {
            _oauthDomainService = oauthDomainService;
        }

        public async Task<Result<AuthResponseDto>> Handle(LoginFacebookCommand request, CancellationToken cancellationToken)
        {
            return await _oauthDomainService.LoginConFacebookAsync(request.FacebookToken);
        }
    }
}
