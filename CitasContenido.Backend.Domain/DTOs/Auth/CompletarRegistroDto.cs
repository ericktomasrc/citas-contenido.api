using System.ComponentModel.DataAnnotations;

namespace CitasContenido.Backend.Domain.DTOs.Auth
{
    public class CompletarRegistroDto
    {
        [Required(ErrorMessage = "El ID de usuario es requerido")]
        public long UsuarioId { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder 100 caracteres")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "La edad es requerida")]
        [Range(18, 100, ErrorMessage = "La edad debe estar entre 18 y 100 años")]
        public int Edad { get; set; }

        [Required(ErrorMessage = "El género es requerido")]
        [RegularExpression("^[MF]$", ErrorMessage = "El género debe ser M o F")]
        public int GeneroId { get; set; }

        [Required(ErrorMessage = "El tipo de documento es requerido")]
        public int TipoDocumentoId { get; set; }

        [Required(ErrorMessage = "El número de documento es requerido")]
        [StringLength(20, ErrorMessage = "El número de documento no puede exceder 20 caracteres")]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nacionalidad es requerida")]
        [StringLength(100, ErrorMessage = "La nacionalidad no puede exceder 100 caracteres")]
        public string Nacionalidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]",
            ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
