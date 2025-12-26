using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.PasswordReset.Commands
{
    public class RestablecerPasswordHandler
           : IRequestHandler<RestablecerPasswordCommand, Result<string>>
    {
        private readonly IPasswordResetDomainService _passwordResetDomainService;

        public RestablecerPasswordHandler(IPasswordResetDomainService passwordResetDomainService)
        {
            _passwordResetDomainService = passwordResetDomainService;
        }

        public async Task<Result<string>> Handle(
            RestablecerPasswordCommand request,
            CancellationToken cancellationToken)
        {
            return await _passwordResetDomainService.RestablecerPasswordAsync(
                request.Email,
                request.Codigo,
                request.NuevaPassword,
                request.ConfirmarPassword
            );
        }
    }
} 
