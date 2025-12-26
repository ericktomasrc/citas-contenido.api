namespace CitasContenido.Backend.Application.Features.Auth.PasswordReset.DTOs
{
    public class VerificarCodigoRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
    }
}
