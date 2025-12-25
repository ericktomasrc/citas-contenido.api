using CitasContenido.Shared.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CitasContenido.Backend.Application.Features.Auth.Commands.CompletarRegistro
{
    public class CompletarRegistroCommand : IRequest<Result<CompletarRegistroResponse>>
    {
        // Identificación
        public long UsuarioId { get; set; }
        public int TipoUsuarioId { get; set; }

        // Paso 1: Información Personal
        public string Username { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public int GeneroId { get; set; } // M o F

        // Paso 2: Documento de Identidad (Solo Creadores)
        public int? TipoDocumentoId { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? Nacionalidad { get; set; }

        // Paso 3: Contacto y Pagos (Solo Creadores)
        public string? WhatsApp { get; set; }
        public string? NumeroYape { get; set; }
        public string? NumeroPlin { get; set; }
        public string? BancoNombre { get; set; }
        public string? NumeroCuenta { get; set; }
        public string? Bio { get; set; }

        // Paso 4: Contraseña
        public string Password { get; set; } = string.Empty;
        public string ConfirmarPassword { get; set; } = string.Empty;

        // Paso 5: Verificación
        public IFormFile? FotoPerfil { get; set; }
        public IFormFile? FotoEnVivo { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
    }

    public class CompletarRegistroResponse
    {
        public long UsuarioId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }
}
