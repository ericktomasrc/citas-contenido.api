using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<string>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<Result<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            // Revocar todos los refresh tokens del usuario
            await _refreshTokenRepository.RevocarTodosPorUsuarioAsync(request.UsuarioId);

            return Result<string>.Success("Sesión cerrada exitosamente");
        }
    }
}
