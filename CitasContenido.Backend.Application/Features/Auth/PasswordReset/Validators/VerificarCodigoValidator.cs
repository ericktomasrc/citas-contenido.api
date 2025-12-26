using CitasContenido.Backend.Application.Features.Auth.PasswordReset.Commands;
using FluentValidation;

namespace CitasContenido.Backend.Application.Features.Auth.PasswordReset.Validators
{
    public class VerificarCodigoValidator : AbstractValidator<VerificarCodigoCommand>
    {
        public VerificarCodigoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio")
                .EmailAddress().WithMessage("El email no es válido");

            RuleFor(x => x.Codigo)
                .NotEmpty().WithMessage("El código es obligatorio")
                .Length(6).WithMessage("El código debe tener 6 dígitos")
                .Matches(@"^\d{6}$").WithMessage("El código solo puede contener números");
        }
    }
}
