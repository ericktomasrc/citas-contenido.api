using FluentValidation;

namespace CitasContenido.Backend.Application.Features.TipoDocumentos.Commands.CreateTipoDocumento
{
    public class CreateTipoDocumentoValidator : AbstractValidator<CreateTipoDocumentoCommand>
    {
        public CreateTipoDocumentoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido")
                .MinimumLength(2).WithMessage("El nombre debe tener al menos 2 caracteres")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

            RuleFor(x => x.UsuarioCreacion)
                .NotEmpty().WithMessage("El usuario de creación es requerido")
                .MaximumLength(50).WithMessage("El usuario de creación no puede exceder 50 caracteres");
        }
    }
}
