using CitasContenido.Backend.Domain.DTOs.Auth;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
    {
        private readonly IRefreshTokenDomainService _refreshTokenDomainService;

        public RefreshTokenCommandHandler(IRefreshTokenDomainService refreshTokenDomainService)
        {
            _refreshTokenDomainService = refreshTokenDomainService;
        }

        public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _refreshTokenDomainService.RefreshTokenAsync(request.RefreshToken);
        }
    }
}
