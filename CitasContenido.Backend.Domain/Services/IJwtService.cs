using System.Security.Claims;

namespace CitasContenido.Backend.Domain.Services
{
    public interface IJwtService
    {
        string GenerarToken(Guid nGuidusuario, string email);
        string GenerarRefreshToken();
        ClaimsPrincipal? ValidarToken(string token);
    }
}
