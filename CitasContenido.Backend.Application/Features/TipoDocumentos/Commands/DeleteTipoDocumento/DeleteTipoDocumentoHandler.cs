using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Shared.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CitasContenido.Backend.Application.Features.TipoDocumentos.Commands.DeleteTipoDocumento
{
    public class DeleteTipoDocumentoHandler : IRequestHandler<DeleteTipoDocumentoCommand, Result>
    {
        private readonly ITipoDocumentoRepository _repository;
        private readonly ILogger<DeleteTipoDocumentoHandler> _logger;

        public DeleteTipoDocumentoHandler(
            ITipoDocumentoRepository repository,
            ILogger<DeleteTipoDocumentoHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result> Handle(
            DeleteTipoDocumentoCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var tipoDocumento = await _repository.GetByIdAsync(request.Id);

                if (tipoDocumento == null)
                {
                    _logger.LogWarning("Tipo de documento no encontrado: {Id}", request.Id);
                    return Result.Failure($"Tipo de documento con ID '{request.Id}' no fue encontrado");
                }

                // Validar usando lógica de dominio
                if (!tipoDocumento.PuedeSerEliminado())
                {
                    _logger.LogWarning("Tipo de documento no puede eliminarse (está habilitado): {Id}", request.Id);
                    return Result.Failure("No se puede eliminar un tipo de documento habilitado. Primero deshabilítelo.");
                }

                var deleted = await _repository.DeleteAsync(request.Id);

                if (!deleted)
                    return Result.Failure("Error al eliminar el tipo de documento");

                _logger.LogInformation("Tipo de documento eliminado: {Id}", request.Id);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar tipo de documento: {Id}", request.Id);
                return Result.Failure("Ocurrió un error al eliminar el tipo de documento");
            }
        }
    }
}
