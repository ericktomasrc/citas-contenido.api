using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace CitasContenido.Backend.Infraestructure.Services
{
    public class VerificarIdentidadDomainService : IVerificarIdentidadDomainService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IVerificacionIdentidadRepository _verificacionIdentidadRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IUnitOfWork _unitOfWork;

        public VerificarIdentidadDomainService(
            IUsuarioRepository usuarioRepository,
            IVerificacionIdentidadRepository verificacionIdentidadRepository,
            IBlobStorageService blobStorageService,
            IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _verificacionIdentidadRepository = verificacionIdentidadRepository;
            _blobStorageService = blobStorageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> VerificarIdentidadAsync(
            long usuarioId,
            IFormFile? fotoPerfil,
            IFormFile fotoDocumento,
            decimal latitud,
            decimal longitud)
        {
            try
            {
                // Obtener usuario
                var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);
                if (usuario == null)
                {
                    return Result<string>.Failure("Usuario no encontrado");
                }

                // Verificar que el email esté verificado
                if (!usuario.EmailVerificado)
                {
                    return Result<string>.Failure("Debes verificar tu email primero");
                }

                // Subir foto de perfil (opcional)
                string? fotoPerfilUrl = null;
                if (fotoPerfil != null)
                {
                    fotoPerfilUrl = await _blobStorageService.SubirFotoAsync(
                        fotoPerfil,
                        $"usuarios/{usuarioId}/perfil"
                    );
                }

                // Subir foto de documento (obligatorio)
                var fotoDocumentoUrl = await _blobStorageService.SubirFotoAsync(
                    fotoDocumento,
                    $"usuarios/{usuarioId}/documentos"
                );

                await _unitOfWork.BeginTransactionAsync();

                // Establecer ubicación
                usuario.EstablecerUbicacion(latitud, longitud);

                // Establecer fotos
                usuario.EstablecerFotos(fotoPerfilUrl, fotoDocumentoUrl);
                await _usuarioRepository.ActualizarAsync(usuario, _unitOfWork);

                // Crear registro de verificación de identidad
                var verificacion = VerificacionIdentidad.Crear(
                    usuario.Id,
                    fotoPerfilUrl,
                    fotoDocumentoUrl
                );
                await _verificacionIdentidadRepository.CrearAsync(verificacion, _unitOfWork);

                await _unitOfWork.CommitAsync();

                return Result<string>.Success("Documentos enviados para verificación. Te notificaremos cuando sean revisados.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception($"Error al verificar identidad: {ex.Message}", ex);
            }
        }
    }
}
