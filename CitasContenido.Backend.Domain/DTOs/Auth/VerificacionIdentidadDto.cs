using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CitasContenido.Backend.Domain.DTOs.Auth
{
    public class VerificacionIdentidadDto
    {
        [Required(ErrorMessage = "La foto en vivo es requerida")]
        public IFormFile? FotoEnVivo { get; set; }

        [Required(ErrorMessage = "La foto del documento es requerida")]
        public IFormFile FotoDocumento { get; set; } = null!;

        [Required(ErrorMessage = "La latitud es requerida")]
        [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90")]
        public decimal Latitud { get; set; }

        [Required(ErrorMessage = "La longitud es requerida")]
        [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180")]
        public decimal Longitud { get; set; }
    }
}
