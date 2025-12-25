using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.Commands.VerificarIdentidad
{
    public class VerificarIdentidadCommandHandler : IRequestHandler<VerificarIdentidadCommand, Result<string>>
    {
        private readonly IVerificarIdentidadDomainService _verificarIdentidadDomainService;

        public VerificarIdentidadCommandHandler(IVerificarIdentidadDomainService verificarIdentidadDomainService)
        {
            _verificarIdentidadDomainService = verificarIdentidadDomainService;
        }

        public async Task<Result<string>> Handle(VerificarIdentidadCommand request, CancellationToken cancellationToken)
        {
            return await _verificarIdentidadDomainService.VerificarIdentidadAsync(
                request.UsuarioId,
                request.FotoPerfil,
                request.FotoDocumento,
                request.Latitud,
                request.Longitud
            );
        }
    }
}
