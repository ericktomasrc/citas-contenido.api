using FluentValidation;

namespace CitasContenido.Backend.Application.Features.Auth.VerificarEmail
{
    public class VerificarEmailCommandValidator : AbstractValidator<VerificarEmailCommand>
    {
        public VerificarEmailCommandValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("El token es requerido");
        }
    }
}
