using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.DTOs.Auth;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;

namespace CitasContenido.Backend.Infraestructure.Services
{
    public class RefreshTokenDomainService : IRefreshTokenDomainService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenDomainService(
            IRefreshTokenRepository refreshTokenRepository,
            IUsuarioRepository usuarioRepository,
            IJwtService jwtService,
            IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _usuarioRepository = usuarioRepository;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Buscar refresh token
                var token = await _refreshTokenRepository.ObtenerPorTokenAsync(refreshToken);
                if (token == null || !token.EstaActivo())
                {
                    return Result<AuthResponseDto>.Failure("Refresh token inválido o expirado");
                }

                // Obtener usuario
                var usuario = await _usuarioRepository.ObtenerPorIdAsync(token.UsuarioId);
                if (usuario == null)
                {
                    return Result<AuthResponseDto>.Failure("Usuario no encontrado");
                }

                // Generar nuevos tokens
                var nuevoToken = _jwtService.GenerarToken(usuario.NGuid, usuario.Email);
                var nuevoRefreshToken = _jwtService.GenerarRefreshToken();

                // Revocar el refresh token anterior
                token.Revocar(nuevoRefreshToken);
                await _refreshTokenRepository.ActualizarAsync(token, _unitOfWork);

                // Guardar nuevo refresh token
                var nuevoRefreshTokenEntity = RefrescarToken.Crear(usuario.Id, nuevoRefreshToken, diasValidez: 7);
                await _refreshTokenRepository.CrearAsync(nuevoRefreshTokenEntity, _unitOfWork);

                // Actualizar última actividad
                usuario.ActualizarActividad();
                await _usuarioRepository.ActualizarAsync(usuario, _unitOfWork);

                await _unitOfWork.CommitAsync();

                // Construir respuesta
                var response = new AuthResponseDto
                {
                    Token = nuevoToken,
                    RefreshToken = nuevoRefreshToken,
                    User = new UsuarioDto
                    {
                        Id = usuario.Id,
                        Nombre = usuario.Nombre ?? string.Empty,
                        Apellidos = usuario.Apellidos ?? string.Empty,
                        Email = usuario.Email,
                        Edad = usuario.Edad,
                        GeneroId = usuario.GeneroId,
                        TipoDocumentoId = usuario.TipoDocumentoId,
                        NumeroDocumento = usuario.NumeroDocumento,
                        Nacionalidad = usuario.Nacionalidad,
                        EmailVerificado = usuario.EmailVerificado,
                        IdentidadVerificada = usuario.IdentidadVerificada,
                        Latitud = usuario.Latitud,
                        Longitud = usuario.Longitud,
                        RangoDistanciaKm = usuario.RangoDistanciaKm,
                        FotoPerfil = usuario.FotoPerfil,
                        FotoEnVivo = usuario.FotoEnVivo,
                        IsPremium = usuario.IsPremium,
                        UltimaActividad = usuario.UltimaActividad
                    }
                };

                return Result<AuthResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception($"Error al refrescar token: {ex.Message}", ex);
            }
        }
    }
}
