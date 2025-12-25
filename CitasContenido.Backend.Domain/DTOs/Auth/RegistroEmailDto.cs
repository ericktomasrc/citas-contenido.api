using System.ComponentModel.DataAnnotations;

namespace CitasContenido.Backend.Domain.DTOs.Auth
{
    public class RegistroEmailDto
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; } = string.Empty;
    }
}
