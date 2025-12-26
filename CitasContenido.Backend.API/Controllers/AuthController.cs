using CitasContenido.Backend.Application.Features.Auth.Login;
using CitasContenido.Backend.Application.Features.Auth.Logout;
using CitasContenido.Backend.Application.Features.Auth.ReenviarEmailVerificacion;
using CitasContenido.Backend.Application.Features.Auth.RefreshToken;
using CitasContenido.Backend.Application.Features.Auth.RegistrarEmail;
using CitasContenido.Backend.Application.Features.Auth.VerificarEmail;
using CitasContenido.Backend.Application.Features.Auth.VerificarIdentidad;
using CitasContenido.Backend.Application.Features.Auth.CompletarRegistro;
using CitasContenido.Backend.Domain.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CitasContenido.Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Paso 1: Registrar email - Envía correo de verificación
        /// </summary>
        [HttpPost("registrar-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarEmail([FromBody] RegistroEmailDto dto)
        {
            var command = new RegistrarEmailCommand(dto.Email);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(new { message = result.Error });

            return Ok(new { message = result.Value });
        }

        /// <summary>
        /// Paso 2: Verificar email - Click en el link del correo
        /// </summary>
        [HttpPost("verificar-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerificarEmail([FromBody] VerificarEmailDto dto)
        {
            var command = new VerificarEmailCommand(dto.Token);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(new { message = result.Error });

            return Ok(new
            {
                message = "Email verificado exitosamente",
                usuarioId = result.Value
            });
        }
        
        /// <summary>
        /// Paso 4: Verificar identidad - Fotos + ubicación GPS
        /// </summary>
        [HttpPost("verificar-identidad")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerificarIdentidad([FromForm] VerificacionIdentidadDto dto)
        {
            var usuarioId = ObtenerUsuarioIdDeClaims();

            if (usuarioId ==0)
                return Unauthorized(new { message = "Usuario no autenticado" });

            var command = new VerificarIdentidadCommand(
                usuarioId,
                dto.FotoEnVivo,
                dto.FotoDocumento,
                dto.Latitud,
                dto.Longitud
            );

            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(new { message = result.Error });

            return Ok(new { message = result.Value });
        }

        /// <summary>
        /// Login - Autenticación con email y password
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var command = new LoginCommand(dto.Email, dto.Password);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(new { message = result.Error });

            return Ok(result.Value);
        }

        /// <summary>
        /// Refresh Token - Renovar token de acceso
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {
            var command = new RefreshTokenCommand(dto.RefreshToken);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(new { message = result.Error });

            return Ok(result.Value);
        }

        /// <summary>
        /// Logout - Cerrar sesión
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            var usuarioId = ObtenerUsuarioIdDeClaims();

            if (usuarioId == 0)
                return Unauthorized(new { message = "Usuario no autenticado" });

            var command = new LogoutCommand(usuarioId);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(new { message = result.Error });

            return Ok(new { message = result.Value });
        }

        /// <summary>
        /// Obtener perfil del usuario autenticado
        /// </summary>
        [HttpGet("perfil")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerPerfil()
        {
            var usuarioId = ObtenerUsuarioIdDeClaims();

            if (usuarioId == 0)
                return Unauthorized(new { message = "Usuario no autenticado" });

            // Aquí deberías crear un Query para obtener el perfil
            // Por ahora retornamos el userId
            return Ok(new { usuarioId });
        }

        /// <summary>
        /// Método helper para obtener el ID del usuario desde los claims
        /// </summary>
        private long ObtenerUsuarioIdDeClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return 0;

            return long.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        /// <summary>
        /// Reenviar email de verificación
        /// </summary>
        [HttpPost("reenviar-email-verificacion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReenviarEmailVerificacion([FromBody] RegistroEmailDto dto)
        {
            var command = new ReenviarEmailVerificacionCommand(dto.Email);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(new { message = result.Error });

            return Ok(new { message = result.Value });
        }

        /// <summary>
        /// Paso 3: Completar registro - Datos personales + password
        /// </summary>
        [HttpPost("completar-registro")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CompletarRegistro([FromForm] CompletarRegistroCommand command)
        {
            var resultado = await _mediator.Send(command);

            if (!resultado.IsSuccess)
                return BadRequest(new { error = resultado.Error });

            return Ok(resultado.Value);
        }
    }
}
