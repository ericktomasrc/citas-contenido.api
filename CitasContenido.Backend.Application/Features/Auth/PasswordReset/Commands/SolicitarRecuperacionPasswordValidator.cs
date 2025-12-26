using FluentValidation;

namespace CitasContenido.Backend.Application.Features.Auth.PasswordReset.Commands
{
    public class SolicitarRecuperacionPasswordValidator
           : AbstractValidator<SolicitarRecuperacionPasswordCommand>
    {
        public SolicitarRecuperacionPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio")
                .EmailAddress().WithMessage("El email no es válido");
        }
    }
}
