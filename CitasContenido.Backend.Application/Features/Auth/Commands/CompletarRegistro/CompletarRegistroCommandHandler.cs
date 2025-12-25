using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using MediatR;

namespace CitasContenido.Backend.Application.Features.Auth.Commands.CompletarRegistro
{
    public class CompletarRegistroHandler : IRequestHandler<CompletarRegistroCommand, Result<CompletarRegistroResponse>>
    {
        private readonly ICompletarRegistroDomainService _completarRegistroService;

        public CompletarRegistroHandler(ICompletarRegistroDomainService completarRegistroService)
        {
            _completarRegistroService = completarRegistroService;
        }

        public async Task<Result<CompletarRegistroResponse>> Handle(
            CompletarRegistroCommand request,
            CancellationToken cancellationToken)
        {
            // Convertir IFormFile a Stream para fotos
            Stream? fotoPerfilStream = null;
            Stream? fotoEnVivoStream = null;

            try
            {
                if (request.FotoPerfil != null)
                {
                    fotoPerfilStream = request.FotoPerfil.OpenReadStream();
                }

                if (request.FotoEnVivo != null)
                {
                    fotoEnVivoStream = request.FotoEnVivo.OpenReadStream();
                }

                // Llamar al Domain Service
                var resultado = await _completarRegistroService.CompletarRegistroAsync(
                    usuarioId: request.UsuarioId,
                    tipoUsuarioId: request.TipoUsuarioId,
                    username: request.Username,
                    nombre: request.Nombre,
                    apellidos: request.Apellidos,
                    fechaNacimiento: request.FechaNacimiento,
                    generoId: request.GeneroId,
                    password: request.Password,
                    latitud: request.Latitud,
                    longitud: request.Longitud,
                    fotoPerfilStream: fotoPerfilStream,
                    fotoPerfilNombre: request.FotoPerfil?.FileName ?? "perfil.jpg",
                    fotoEnVivoStream: fotoEnVivoStream,
                    fotoEnVivoNombre: request.FotoEnVivo?.FileName ?? "verificacion.jpg",
                    // Datos de Creador
                    tipoDocumentoId: request.TipoDocumentoId,
                    numeroDocumento: request.NumeroDocumento,
                    nacionalidad: request.Nacionalidad,
                    whatsapp: request.WhatsApp,
                    numeroYape: request.NumeroYape,
                    numeroPlin: request.NumeroPlin,
                    bancoNombre: request.BancoNombre,
                    numeroCuenta: request.NumeroCuenta,
                    bio: request.Bio
                );

                if (!resultado.IsSuccess)
                {
                    return Result<CompletarRegistroResponse>.Failure(resultado.Error);
                }

                return Result<CompletarRegistroResponse>.Success(new CompletarRegistroResponse
                {
                    UsuarioId = resultado.Value.UsuarioId,
                    Token = resultado.Value.Token,
                    RefreshToken = resultado.Value.RefreshToken,
                    Mensaje = "Registro completado exitosamente"
                });
            }
            finally
            {
                // Limpiar streams
                fotoPerfilStream?.Dispose();
                fotoEnVivoStream?.Dispose();
            }
        }
    }
}
