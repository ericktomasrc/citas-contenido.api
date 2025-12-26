namespace CitasContenido.Backend.Application.Features.Auth.PasswordReset.DTOs
{
    public class RestablecerPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string NuevaPassword { get; set; } = string.Empty;
        public string ConfirmarPassword { get; set; } = string.Empty;
    }
}
