using FluentValidation;

namespace CitasContenido.Backend.Application.Features.Auth.ReenviarEmailVerificacion
{
    public class ReenviarEmailVerificacionCommandValidator : AbstractValidator<ReenviarEmailVerificacionCommand>
    {
        public ReenviarEmailVerificacionCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es requerido")
                .EmailAddress().WithMessage("El email no es válido");
        }
    }
}
