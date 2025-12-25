using System.ComponentModel.DataAnnotations;

namespace CitasContenido.Backend.Domain.DTOs.Auth
{
    public class VerificarEmailDto
    {
        [Required(ErrorMessage = "El token es requerido")]
        public string Token { get; set; } = string.Empty;
    }
}
