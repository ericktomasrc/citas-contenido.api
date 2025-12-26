using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.PasswordReset.Commands
{
    public class VerificarCodigoHandler
        : IRequestHandler<VerificarCodigoCommand, Result<bool>>
    {
        private readonly IPasswordResetDomainService _passwordResetDomainService;

        public VerificarCodigoHandler(IPasswordResetDomainService passwordResetDomainService)
        {
            _passwordResetDomainService = passwordResetDomainService;
        }

        public async Task<Result<bool>> Handle(
            VerificarCodigoCommand request,
            CancellationToken cancellationToken)
        {
            return await _passwordResetDomainService.VerificarCodigoAsync(
                request.Email,
                request.Codigo
            );
        }
    }
}
