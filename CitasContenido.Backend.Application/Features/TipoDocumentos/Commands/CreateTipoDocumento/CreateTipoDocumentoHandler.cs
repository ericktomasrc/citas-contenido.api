using AutoMapper;
using CitasContenido.Backend.Domain.DTOs;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Shared.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CitasContenido.Backend.Application.Features.TipoDocumentos.Commands.CreateTipoDocumento
{
    public class CreateTipoDocumentoHandler : IRequestHandler<CreateTipoDocumentoCommand, Result<TipoDocumentoDto>>
    {
        private readonly ITipoDocumentoRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateTipoDocumentoHandler> _logger;

        public CreateTipoDocumentoHandler(
            ITipoDocumentoRepository repository,
            IMapper mapper,
            ILogger<CreateTipoDocumentoHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<TipoDocumentoDto>> Handle(
            CreateTipoDocumentoCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Validar que no exista
                if (await _repository.ExistsByNameAsync(request.Name))
                {
                    _logger.LogWarning("Tipo de documento ya existe: {Name}", request.Name);
                    return Result<TipoDocumentoDto>.Failure($"Ya existe un tipo de documento con el nombre '{request.Name}'");
                }

                // Crear entidad usando lógica de dominio
                var tipoDocumento = TipoDocumento.Create(
                    request.Name,
                    request.Descripcion ?? string.Empty,
                    request.UsuarioCreacion
                );

                // Persistir
                var id = await _repository.CreateAsync(tipoDocumento);

                _logger.LogInformation("Tipo de documento creado: {Id} - {Name}", id, request.Name);

                // Recuperar
                var tipoDocumentoCreado = await _repository.GetByIdAsync(id);

                if (tipoDocumentoCreado == null)
                    return Result<TipoDocumentoDto>.Failure("Error al recuperar el tipo de documento creado");

                // Mapear a DTO
                var dto = _mapper.Map<TipoDocumentoDto>(tipoDocumentoCreado);

                return Result<TipoDocumentoDto>.Success(dto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error de validación al crear tipo de documento");
                return Result<TipoDocumentoDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear tipo de documento");
                return Result<TipoDocumentoDto>.Failure("Ocurrió un error al crear el tipo de documento");
            }
        }
    }
}
