using AutoMapper;
using CitasContenido.Backend.Domain.DTOs;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Shared.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CitasContenido.Backend.Application.Features.TipoDocumentos.Queries.GetTipoDocumentos
{
    public class GetTipoDocumentosHandler : IRequestHandler<GetTipoDocumentosQuery, Result<List<TipoDocumentoDto>>>
    {
        private readonly ITipoDocumentoRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTipoDocumentosHandler> _logger;

        public GetTipoDocumentosHandler(
            ITipoDocumentoRepository repository,
            IMapper mapper,
            ILogger<GetTipoDocumentosHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<List<TipoDocumentoDto>>> Handle(
            GetTipoDocumentosQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var tiposDocumento = await _repository.BuscarAsync(request.Filtro, request.SoloHabilitados);

                var dtos = _mapper.Map<List<TipoDocumentoDto>>(tiposDocumento);

                _logger.LogInformation("Consulta de tipos de documento exitosa. Total: {Count}", dtos.Count);

                return Result<List<TipoDocumentoDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar tipos de documento");
                return Result<List<TipoDocumentoDto>>.Failure("Ocurrió un error al consultar los tipos de documento");
            }
        }
    }
}
