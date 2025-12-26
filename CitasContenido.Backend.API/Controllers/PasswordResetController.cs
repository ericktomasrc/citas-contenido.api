using CitasContenido.Backend.Application.Features.Auth.PasswordReset.Commands;
using CitasContenido.Backend.Application.Features.Auth.PasswordReset.DTOs;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CitasContenido.Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PasswordResetController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Solicitar código de recuperación de contraseña
        /// </summary>
        [HttpPost("solicitar")]
        public async Task<IActionResult> SolicitarRecuperacion(
            [FromBody] SolicitarRecuperacionPasswordRequest request)
        {
            var command = new SolicitarRecuperacionPasswordCommand
            {
                Email = request.Email,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Error });
            }

            return Ok(new { message = result.Value });
        }

        /// <summary>
        /// Verificar código de recuperación
        /// </summary>
        [HttpPost("verificar-codigo")]
        public async Task<IActionResult> VerificarCodigo(
            [FromBody] VerificarCodigoRequest request)
        {
            var command = new VerificarCodigoCommand
            {
                Email = request.Email,
                Codigo = request.Codigo
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Error });
            }

            return Ok(new { message = "Código válido", isValid = result.Value });
        }

        /// <summary>
        /// Restablecer contraseña con código
        /// </summary>
        [HttpPost("restablecer")]
        public async Task<IActionResult> RestablecerPassword(
            [FromBody] RestablecerPasswordRequest request)
        {
            try
            {
                var command = new RestablecerPasswordCommand
                {
                    Email = request.Email,
                    Codigo = request.Codigo,
                    NuevaPassword = request.NuevaPassword,
                    ConfirmarPassword = request.ConfirmarPassword
                };

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { message = result.Error });
                }

                return Ok(new { message = result.Value });
            }
            catch (ValidationException ex) // ✅ CAPTURAR VALIDACIÓN
            {
                var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = string.Join(", ", errors) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }
    }
}
