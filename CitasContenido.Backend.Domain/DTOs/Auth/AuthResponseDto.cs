namespace CitasContenido.Backend.Domain.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UsuarioDto User { get; set; } = null!;
    }
}
