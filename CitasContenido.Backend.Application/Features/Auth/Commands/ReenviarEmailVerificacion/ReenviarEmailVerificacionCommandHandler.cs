using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.Commands.ReenviarEmailVerificacion
{
    public class ReenviarEmailVerificacionCommandHandler : IRequestHandler<ReenviarEmailVerificacionCommand, Result<string>>
    {
        private readonly IRegistrarEmailDomainService _registrarEmailDomainService;

        public ReenviarEmailVerificacionCommandHandler(IRegistrarEmailDomainService registrarEmailDomainService)
        {
            _registrarEmailDomainService = registrarEmailDomainService;
        }

        public async Task<Result<string>> Handle(ReenviarEmailVerificacionCommand request, CancellationToken cancellationToken)
        {
            return await _registrarEmailDomainService.ReenviarEmailVerificacionAsync(request.Email);
        }
    }
}
