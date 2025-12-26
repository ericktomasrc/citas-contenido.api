using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.PasswordReset.Commands
{
    public class SolicitarRecuperacionPasswordCommand : IRequest<Result<string>>
    {
        public string Email { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
