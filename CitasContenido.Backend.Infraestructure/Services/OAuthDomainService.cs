using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.DTOs.Auth;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using Google.Apis.Auth;
using System.Net.Http.Json;

namespace CitasContenido.Backend.Infraestructure.Services
{
    public class OAuthDomainService : IOAuthDomainService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;

        public OAuthDomainService(
            IUsuarioRepository usuarioRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtService jwtService,
            IUnitOfWork unitOfWork,
            IHttpClientFactory httpClientFactory)
        {
            _usuarioRepository = usuarioRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Result<AuthResponseDto>> LoginConGoogleAsync(string googleToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                //   VALIDAR TOKEN DE GOOGLE
                GoogleJsonWebSignature.Payload payload;
                try
                {
                    payload = await GoogleJsonWebSignature.ValidateAsync(googleToken);
                }
                catch (Exception)
                {
                    return Result<AuthResponseDto>.Failure("Token de Google inválido");
                }

                var googleId = payload.Subject;
                var email = payload.Email;
                var nombre = payload.GivenName;
                var apellidos = payload.FamilyName;

                //   BUSCAR SI YA EXISTE USUARIO CON ESTE GOOGLE ID
                var usuario = await _usuarioRepository.ObtenerPorGoogleIdAsync(googleId);

                //   SI NO EXISTE, BUSCAR POR EMAIL
                if (usuario == null)
                {
                    usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);

                    if (usuario != null)
                    {
                        //   CASO 1: Email existe Y registro está completo
                        if (usuario.RegistroCompletado)
                        {
                            return Result<AuthResponseDto>.Failure("Este email ya está registrado. Por favor inicia sesión con tu contraseña");
                        }

                        //   CASO 2: Email verificado pero registro NO completado
                        // Vincular Google y enviar señal para completar registro
                        usuario.VincularGoogleId(googleId); // Vincular Google ID
                        await _usuarioRepository.ActualizarAsync(usuario, _unitOfWork);
                        await _unitOfWork.CommitAsync();

                        return Result<AuthResponseDto>.Success(new AuthResponseDto
                        {
                            Token = null, //   Sin token aún
                            RefreshToken = null,
                            User = new UsuarioDto
                            {
                                Id = usuario.Id,
                                Email = usuario.Email
                            },
                            Message = $"CONTINUAR_REGISTRO|{usuario.Id}" //   Señal especial
                        });
                    }
                }

                //   SI NO EXISTE NINGÚN USUARIO, CREAR UNO NUEVO (solo con email verificado)
                if (usuario == null)
                {
                    usuario = Usuario.CrearConGoogle(email, googleId, nombre, apellidos);

                    // El método CrearConGoogle se encarga de inicializar NGuid y otras propiedades internas.
                    // No es necesario asignar NGuid manualmente.

                    long id =await _usuarioRepository.CrearAsync(usuario, _unitOfWork);
                    await _unitOfWork.CommitAsync();

                    return Result<AuthResponseDto>.Success(new AuthResponseDto
                    {
                        Token = null,
                        RefreshToken = null,
                        User = new UsuarioDto
                        {
                            Id = id,
                            Email = usuario.Email
                        },
                        Message = $"CONTINUAR_REGISTRO|{usuario.Id}"
                    });
                }

                //   CASO 3: Usuario existe y registro YA está completo → Login normal
                usuario.ActualizarActividad();
                await _usuarioRepository.ActualizarAsync(usuario, _unitOfWork);

                var token = _jwtService.GenerarToken(usuario.NGuid, usuario.Email);
                var refreshToken = _jwtService.GenerarRefreshToken();

                var refreshTokenEntity = RefrescarToken.Crear(usuario.Id, refreshToken, diasValidez: 7);
                await _refreshTokenRepository.CrearAsync(refreshTokenEntity, _unitOfWork);

                await _unitOfWork.CommitAsync();

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
                throw new Exception($"Error al iniciar sesión con Google: {ex.Message}", ex);
            }
        }

        public async Task<Result<AuthResponseDto>> LoginConFacebookAsync(string facebookToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                //    VALIDAR TOKEN DE FACEBOOK
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(
                    $"https://graph.facebook.com/me?fields=id,email,first_name,last_name&access_token={facebookToken}");

                if (!response.IsSuccessStatusCode)
                {
                    return Result<AuthResponseDto>.Failure("Token de Facebook inválido");
                }

                var fbUserData = await response.Content.ReadFromJsonAsync<FacebookUserData>();
                if (fbUserData == null || string.IsNullOrEmpty(fbUserData.Id))
                {
                    return Result<AuthResponseDto>.Failure("No se pudo obtener información de Facebook");
                }

                var facebookId = fbUserData.Id;
                var email = fbUserData.Email ?? $"{facebookId}@facebook.com"; // Facebook puede no dar email
                var nombre = fbUserData.FirstName;
                var apellidos = fbUserData.LastName;

                //    BUSCAR SI YA EXISTE USUARIO CON ESTE FACEBOOK ID
                var usuario = await _usuarioRepository.ObtenerPorFacebookIdAsync(facebookId);

                //    SI NO EXISTE, BUSCAR POR EMAIL
                if (usuario == null && !string.IsNullOrEmpty(fbUserData.Email))
                {
                    usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);

                    if (usuario != null)
                    {
                        return Result<AuthResponseDto>.Failure("Este email ya está registrado. Por favor inicia sesión con tu contraseña");
                    }
                }

                //    SI NO EXISTE, CREAR NUEVO USUARIO
                if (usuario == null)
                {
                    usuario = Usuario.CrearConFacebook(email, facebookId, nombre, apellidos);
                    await _usuarioRepository.CrearAsync(usuario, _unitOfWork);
                }

                //    ACTUALIZAR ÚLTIMA ACTIVIDAD
                usuario.ActualizarActividad();
                await _usuarioRepository.ActualizarAsync(usuario, _unitOfWork);

                //    GENERAR TOKENS
                var token = _jwtService.GenerarToken(usuario.NGuid, usuario.Email);
                var refreshToken = _jwtService.GenerarRefreshToken();

                //    GUARDAR REFRESH TOKEN
                var refreshTokenEntity = RefrescarToken.Crear(usuario.Id, refreshToken, diasValidez: 7);
                await _refreshTokenRepository.CrearAsync(refreshTokenEntity, _unitOfWork);

                await _unitOfWork.CommitAsync();

                //    CONSTRUIR RESPUESTA
                var response2 = new AuthResponseDto
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

                return Result<AuthResponseDto>.Success(response2);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception($"Error al iniciar sesión con Facebook: {ex.Message}", ex);
            }
        }

        //    CLASE AUXILIAR PARA DESERIALIZAR RESPUESTA DE FACEBOOK
        private class FacebookUserData
        {
            public string Id { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
        }
    }
}
