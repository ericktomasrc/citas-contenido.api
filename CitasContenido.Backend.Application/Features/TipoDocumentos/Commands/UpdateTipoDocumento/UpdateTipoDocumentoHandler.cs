using AutoMapper;
using CitasContenido.Backend.Domain.DTOs;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Shared.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CitasContenido.Backend.Application.Features.TipoDocumentos.Commands.UpdateTipoDocumento
{
    public class UpdateTipoDocumentoHandler : IRequestHandler<UpdateTipoDocumentoCommand, Result<TipoDocumentoDto>>
    {
        private readonly ITipoDocumentoRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateTipoDocumentoHandler> _logger;

        public UpdateTipoDocumentoHandler(
            ITipoDocumentoRepository repository,
            IMapper mapper,
            ILogger<UpdateTipoDocumentoHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<TipoDocumentoDto>> Handle(
            UpdateTipoDocumentoCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var tipoDocumento = await _repository.GetByIdAsync(request.Id);

                if (tipoDocumento == null)
                {
                    _logger.LogWarning("Tipo de documento no encontrado: {Id}", request.Id);
                    return Result<TipoDocumentoDto>.Failure($"Tipo de documento con ID '{request.Id}' no fue encontrado");
                }

                // Actualizar usando lógica de dominio
                tipoDocumento.Actualizar(
                    request.Name,
                    request.Descripcion ?? string.Empty,
                    request.UsuarioModificacion
                );

                var updated = await _repository.UpdateAsync(tipoDocumento);

                if (!updated)
                    return Result<TipoDocumentoDto>.Failure("Error al actualizar el tipo de documento");

                _logger.LogInformation("Tipo de documento actualizado: {Id}", request.Id);

                var dto = _mapper.Map<TipoDocumentoDto>(tipoDocumento);

                return Result<TipoDocumentoDto>.Success(dto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error de validación al actualizar");
                return Result<TipoDocumentoDto>.Failure(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error de lógica de negocio");
                return Result<TipoDocumentoDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar");
                return Result<TipoDocumentoDto>.Failure("Ocurrió un error al actualizar el tipo de documento");
            }
        }
    }
}
