using CitasContenido.Backend.Application.Features.TipoDocumentos.Commands.CreateTipoDocumento;
using CitasContenido.Backend.Application.Features.TipoDocumentos.Commands.DeleteTipoDocumento;
using CitasContenido.Backend.Application.Features.TipoDocumentos.Commands.UpdateTipoDocumento;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CitasContenido.Backend.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TipoDocumentoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TipoDocumentoController> _logger;

        public TipoDocumentoController(
            IMediator mediator,
            ILogger<TipoDocumentoController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("buscar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Buscar([FromQuery] string? filtro, [FromQuery] bool? soloHabilitados)
        {
            var query = new GetTipoDocumentosQuery(filtro, soloHabilitados);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return StatusCode(500, new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTipoDocumentoCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return CreatedAtAction(nameof(Buscar), new { id = result.Value!.Id }, result.Value);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTipoDocumentoCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { error = "El ID de la ruta no coincide con el ID del body" });

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                if (result.Error.Contains("no fue encontrado"))
                    return NotFound(new { error = result.Error });

                return BadRequest(new { error = result.Error });
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteTipoDocumentoCommand(id);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                if (result.Error.Contains("no fue encontrado"))
                    return NotFound(new { error = result.Error });

                return StatusCode(500, new { error = result.Error });
            }

            return NoContent();
        }
    }
}
