using FluentValidation;

namespace CitasContenido.Backend.Application.Features.Auth.Commands.RegistrarEmail
{
    public class RegistrarEmailCommandValidator : AbstractValidator<RegistrarEmailCommand>
    {
        public RegistrarEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es requerido")
                .EmailAddress().WithMessage("El email no es válido");
        }
    }
}
