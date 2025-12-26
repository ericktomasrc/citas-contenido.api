using CitasContenido.Backend.Application.Features.Auth.PasswordReset.Commands;
using FluentValidation;

namespace CitasContenido.Backend.Application.Features.Auth.PasswordReset.Validators
{
    public class RestablecerPasswordValidator : AbstractValidator<RestablecerPasswordCommand>
    {
        public RestablecerPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio")
                .EmailAddress().WithMessage("El email no es válido");

            RuleFor(x => x.Codigo)
                .NotEmpty().WithMessage("El código es obligatorio")
                .Length(6).WithMessage("El código debe tener 6 dígitos")
                .Matches(@"^\d{6}$").WithMessage("El código solo puede contener números");

            RuleFor(x => x.NuevaPassword)
                .NotEmpty().WithMessage("La contraseña es obligatoria")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
                .Matches(@"[A-Z]").WithMessage("La contraseña debe tener al menos una mayúscula")
                //.Matches(@"[a-z]").WithMessage("La contraseña debe tener al menos una minúscula")
                .Matches(@"[0-9]").WithMessage("La contraseña debe tener al menos un número")
                .Matches(@"[\W_]").WithMessage("La contraseña debe tener al menos un carácter especial");

            RuleFor(x => x.ConfirmarPassword)
                .NotEmpty().WithMessage("Debes confirmar la contraseña")
                .Equal(x => x.NuevaPassword).WithMessage("Las contraseñas no coinciden");
        }
    }
}
