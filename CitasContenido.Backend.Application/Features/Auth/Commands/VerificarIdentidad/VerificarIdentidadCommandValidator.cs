using FluentValidation;

namespace CitasContenido.Backend.Application.Features.Auth.Commands.VerificarIdentidad
{
    public class VerificarIdentidadCommandValidator : AbstractValidator<VerificarIdentidadCommand>
    {
        public VerificarIdentidadCommandValidator()
        {
            RuleFor(x => x.UsuarioId)
                .NotEmpty().WithMessage("El ID de usuario es requerido");

            RuleFor(x => x.FotoDocumento)
                .NotNull().WithMessage("La foto del documento es requerida");

            RuleFor(x => x.Latitud)
                .InclusiveBetween(-90, 90).WithMessage("La latitud debe estar entre -90 y 90");

            RuleFor(x => x.Longitud)
                .InclusiveBetween(-180, 180).WithMessage("La longitud debe estar entre -180 y 180");
        }
    }
}
