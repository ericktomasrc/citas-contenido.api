using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.RegistrarEmail
{
    public class RegistrarEmailCommandHandler : IRequestHandler<RegistrarEmailCommand, Result<string>>
    {
        private readonly IRegistrarEmailDomainService _registrarEmailDomainService;

        public RegistrarEmailCommandHandler(IRegistrarEmailDomainService registrarEmailDomainService)
        {
            _registrarEmailDomainService = registrarEmailDomainService;
        }

        public async Task<Result<string>> Handle(RegistrarEmailCommand request, CancellationToken cancellationToken)
        {
            return await _registrarEmailDomainService.RegistrarEmailAsync(request.Email);
        }
    }
}
