using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.VerificarEmail
{
    public class VerificarEmailCommandHandler : IRequestHandler<VerificarEmailCommand, Result<long>>
    {
        private readonly IVerificacionEmailDomainService _verificacionEmailDomainService;

        public VerificarEmailCommandHandler(IVerificacionEmailDomainService verificacionEmailDomainService)
        {
            _verificacionEmailDomainService = verificacionEmailDomainService;
        }

        public async Task<Result<long>> Handle(VerificarEmailCommand request, CancellationToken cancellationToken)
        {            
            return await _verificacionEmailDomainService.VerificarEmailAsync(request.Token);
        }
    }
}
