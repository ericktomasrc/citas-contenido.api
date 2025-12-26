using FluentValidation;

namespace CitasContenido.Backend.Application.Features.Auth.CompletarRegistro
{
    public class CompletarRegistroValidator : AbstractValidator<CompletarRegistroCommand>
    {
        public CompletarRegistroValidator()
        {
            // Información Personal
            RuleFor(x => x.UsuarioId)
                .GreaterThan(0).WithMessage("UsuarioId inválido");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username es requerido")
                .Matches("^[a-z0-9_]+$").WithMessage("Username solo puede contener letras minúsculas, números y guión bajo")
                .MinimumLength(3).WithMessage("Username debe tener al menos 3 caracteres")
                .MaximumLength(20).WithMessage("Username no puede exceder 20 caracteres");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("Nombre es requerido")
                .MaximumLength(100).WithMessage("Nombre no puede exceder 100 caracteres");

            RuleFor(x => x.Apellidos)
                .NotEmpty().WithMessage("Apellidos son requeridos")
                .MaximumLength(100).WithMessage("Apellidos no pueden exceder 100 caracteres"); 

            // Contraseña
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Contraseña es requerida")
                .MinimumLength(8).WithMessage("Contraseña debe tener al menos 8 caracteres")
                .Matches(@"[A-Z]").WithMessage("Contraseña debe contener al menos una mayúscula")
                .Matches(@"[0-9]").WithMessage("Contraseña debe contener al menos un número")
                .Matches(@"[\W_]").WithMessage("Contraseña debe contener al menos un carácter especial");

            RuleFor(x => x.ConfirmarPassword)
                .Equal(x => x.Password).WithMessage("Las contraseñas no coinciden");

            // Ubicación
            RuleFor(x => x.Latitud)
                .InclusiveBetween(-90, 90).WithMessage("Latitud inválida");

            RuleFor(x => x.Longitud)
                .InclusiveBetween(-180, 180).WithMessage("Longitud inválida");

            // Fotos
            RuleFor(x => x.FotoDocumento)
                .NotNull().WithMessage("Foto de documento es requerida");

            RuleFor(x => x.FotoEnVivo)
                .NotNull().WithMessage("Foto en vivo es requerida");

            // Validaciones condicionales para Creadores
            When(x => x.TipoUsuarioId == 2, () =>
            {
                RuleFor(x => x.FechaNacimiento)
                      .NotEmpty().WithMessage("Fecha de nacimiento es requerida")
                      .Must(BeAtLeast18YearsOld).WithMessage("Debes ser mayor de 18 años");
                
                RuleFor(x => x.GeneroId)
                 .GreaterThan(0).WithMessage("Seleccionar genero");

                RuleFor(x => x.TipoDocumentoId)
                    .NotEmpty().WithMessage("Tipo de documento es requerido para creadores");

                RuleFor(x => x.NumeroDocumento)
                    .NotEmpty().WithMessage("Número de documento es requerido para creadores");

                RuleFor(x => x.Nacionalidad)
                    .NotEmpty().WithMessage("Nacionalidad es requerida para creadores");

                RuleFor(x => x.WhatsApp)
                    .NotEmpty().WithMessage("WhatsApp es requerido para creadores")
                    .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Formato de WhatsApp inválido");

                When(x => x.NumeroYape?.Trim() !="", () =>
                {
                    RuleFor(x => x.NumeroYape) 
                    .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Formato de NumeroYape inválido");
                });

                When(x => x.NumeroPlin?.Trim() != "", () =>
                {
                    RuleFor(x => x.NumeroPlin)
                    .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Formato de NumeroPlin inválido");
                });
            });
        }

        private bool BeAtLeast18YearsOld(DateTime fechaNacimiento)
        {
            var edad = DateTime.Today.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;
            return edad >= 18;
        }
    }
}
