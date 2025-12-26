using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.PasswordReset.Commands
{
    public class SolicitarRecuperacionPasswordHandler
            : IRequestHandler<SolicitarRecuperacionPasswordCommand, Result<string>>
    {
        private readonly IPasswordResetDomainService _passwordResetDomainService;

        public SolicitarRecuperacionPasswordHandler(IPasswordResetDomainService passwordResetDomainService)
        {
            _passwordResetDomainService = passwordResetDomainService;
        }

        public async Task<Result<string>> Handle(
            SolicitarRecuperacionPasswordCommand request,
            CancellationToken cancellationToken)
        {
            return await _passwordResetDomainService.SolicitarRecuperacionPasswordAsync(
                request.Email,
                request.IpAddress,
                request.UserAgent
            );
        }
    }
}
