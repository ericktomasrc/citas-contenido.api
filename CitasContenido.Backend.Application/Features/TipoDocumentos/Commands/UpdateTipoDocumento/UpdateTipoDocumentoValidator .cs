using FluentValidation;

namespace CitasContenido.Backend.Application.Features.TipoDocumentos.Commands.UpdateTipoDocumento
{
    public class UpdateTipoDocumentoValidator : AbstractValidator<UpdateTipoDocumentoCommand>
    {
        public UpdateTipoDocumentoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID debe ser mayor a 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido")
                .MinimumLength(2).WithMessage("El nombre debe tener al menos 2 caracteres")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

            RuleFor(x => x.UsuarioModificacion)
                .NotEmpty().WithMessage("El usuario de modificación es requerido")
                .MaximumLength(50).WithMessage("El usuario de modificación no puede exceder 50 caracteres");
        }
    }
}
