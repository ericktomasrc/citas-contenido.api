using CitasContenido.Backend.Domain.DTOs.Auth;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly ILoginDomainService _loginDomainService;

        public LoginCommandHandler(ILoginDomainService loginDomainService)
        {
            _loginDomainService = loginDomainService;
        }

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _loginDomainService.LoginAsync(request.Email, request.Password);
        }
    }
}
