using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.DTOs.Auth;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;

namespace CitasContenido.Backend.Infraestructure.Services
{
    public class LoginDomainService : ILoginDomainService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;

        public LoginDomainService(
            IUsuarioRepository usuarioRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(string email, string password)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Buscar usuario por email
                var usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);
                if (usuario == null)
                {
                    return Result<AuthResponseDto>.Failure("Email o contraseña incorrectos");
                }

                // Verificar password
                var passwordValido = _passwordHasher.VerifyPassword(password, usuario.PasswordHash);
                if (!passwordValido)
                {
                    return Result<AuthResponseDto>.Failure("Email o contraseña incorrectos");
                }

                // Verificar que el email esté verificado
                if (!usuario.EmailVerificado)
                {
                    return Result<AuthResponseDto>.Failure("Debes verificar tu email primero");
                }

                // Actualizar última actividad
                usuario.ActualizarActividad();
                await _usuarioRepository.ActualizarAsync(usuario, _unitOfWork);

                // Generar tokens
                var token = _jwtService.GenerarToken(usuario.NGuid, usuario.Email);
                var refreshToken = _jwtService.GenerarRefreshToken();

                // Guardar refresh token
                var refreshTokenEntity = RefrescarToken.Crear(usuario.Id, refreshToken, diasValidez: 7);
                await _refreshTokenRepository.CrearAsync(refreshTokenEntity, _unitOfWork);

                await _unitOfWork.CommitAsync();

                // Construir respuesta
                var response = new AuthResponseDto
                {
                    Token = token,
                    RefreshToken = refreshToken,
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
                        FotoDocumento = usuario.FotoDocumento,
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
                throw new Exception($"Error al iniciar sesión: {ex.Message}", ex);
            }
        }
    }
}
