using System.ComponentModel.DataAnnotations;

namespace CitasContenido.Backend.Domain.DTOs.Auth
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "El refresh token es requerido")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
