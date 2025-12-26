using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using Microsoft.Extensions.Configuration;
using Smr.Backend.Shared;

namespace CitasContenido.Backend.Infraestructure.Services
{
    public class PasswordResetDomainService : IPasswordResetDomainService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordResetTokenRepository _tokenRepository;
        private readonly IPasswordHistoryRepository _passwordHistoryRepository;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly int _minutosExpiracion;
        
        public PasswordResetDomainService(
            IUsuarioRepository usuarioRepository,
            IPasswordResetTokenRepository tokenRepository,
            IPasswordHistoryRepository passwordHistoryRepository,
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _minutosExpiracion = int.Parse(configuration["Configuracion:DuracionExpericacionPassword"] ?? Constantes.MINUTOS_EXPIRACION_PASSWORD);
            _usuarioRepository = usuarioRepository;
            _tokenRepository = tokenRepository;
            _passwordHistoryRepository = passwordHistoryRepository;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        // ==================== 1. SOLICITAR RECUPERACIÓN ====================
        public async Task<Result<string>> SolicitarRecuperacionPasswordAsync(
            string email,
            string? ipAddress = null,
            string? userAgent = null)
        {
            try
            {
                // 1. Verificar que el usuario existe
                var usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);
                if (usuario == null)
                {
                    // Por seguridad, no revelar si el email existe o no
                    return Result<string>.Success("Si el email existe, recibirás un código de recuperación");
                }

                // 2. Generar código aleatorio de 6 dígitos
                var random = new Random();
                var codigo = random.Next(100000, 999999).ToString();

                // 3. Crear token de recuperación
                var token = PasswordResetToken.Crear(
                    usuario.Id,
                    email,
                    codigo,
                    minutosExpiracion: _minutosExpiracion,
                    ipAddress: ipAddress,
                    userAgent: userAgent
                );

                // 4. Guardar en BD
                await _unitOfWork.BeginTransactionAsync();

                // Invalidar tokens anteriores
                await _tokenRepository.InvalidarTodosLosTokensDelUsuarioAsync(usuario.Id, _unitOfWork);

                // Crear nuevo token
                await _tokenRepository.CrearAsync(token, _unitOfWork);

                await _unitOfWork.CommitAsync();

                // 5. Enviar email con el código
                try
                {
                    await _emailService.EnviarCodigoRecuperacionPasswordAsync(
                        email,
                        usuario.Nombre ?? "Usuario",
                        codigo
                    );

                    return Result<string>.Success("Código enviado al email correctamente");
                }
                catch (Exception emailEx)
                {
                    Console.WriteLine($"Error al enviar email: {emailEx.Message}");
                    return Result<string>.Failure("Error al enviar el email. Intenta más tarde.");
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result<string>.Failure($"Error al solicitar recuperación: {ex.Message}");
            }
        }

        // ==================== 2. VERIFICAR CÓDIGO ====================
        public async Task<Result<bool>> VerificarCodigoAsync(string email, string codigo)
        {
            try
            {
                // 1. Verificar que el usuario existe
                var usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);
                if (usuario == null)
                {
                    return Result<bool>.Failure("Email no encontrado");
                }

                // 2. Buscar token válido con el código
                var token = await _tokenRepository.ObtenerPorEmailYCodigoAsync(email, codigo);

                if (token == null)
                {
                    return Result<bool>.Failure("Código inválido o expirado");
                }

                // 3. Verificar que no esté usado ni expirado
                if (!token.EsValido())
                {
                    return Result<bool>.Failure("El código ya fue usado o ha expirado");
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error al verificar código: {ex.Message}");
            }
        }

        // ==================== 3. RESTABLECER CONTRASEÑA ====================
        public async Task<Result<string>> RestablecerPasswordAsync(
            string email,
            string codigo,
            string nuevaPassword,
            string confirmarPassword)
        {
            try
            {
                // 1. Validar que las contraseñas coincidan
                if (nuevaPassword != confirmarPassword)
                {
                    return Result<string>.Failure("Las contraseñas no coinciden");
                }

                // 2. Verificar que el usuario existe
                var usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);
                if (usuario == null)
                {
                    return Result<string>.Failure("Email no encontrado");
                }

                // 3. Buscar token válido con el código
                var token = await _tokenRepository.ObtenerPorEmailYCodigoAsync(email, codigo);

                if (token == null)
                {
                    return Result<string>.Failure("Código inválido o expirado");
                }

                // 4. Verificar que el token sea válido
                if (!token.EsValido())
                {
                    return Result<string>.Failure("El código ya fue usado o ha expirado");
                }

                // 5. Hash de la nueva contraseña
                var nuevaPasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);

                // 6. Actualizar contraseña y regenerar SecurityStamp
                await _unitOfWork.BeginTransactionAsync();

                // Cambiar contraseña (esto regenera el SecurityStamp automáticamente)

                var passwordHistory = PasswordHistory.Crear(usuario.Id, usuario.PasswordHash);  
                await _passwordHistoryRepository.CrearAsync(passwordHistory, _unitOfWork);

                usuario.CambiarContrasena(nuevaPasswordHash);
                await _usuarioRepository.ActualizarAsync(usuario, _unitOfWork);               

                // Marcar token como usado
                token.MarcarComoUsado();
                await _tokenRepository.ActualizarAsync(token, _unitOfWork);

                // Invalidar todos los demás tokens del usuario
                await _tokenRepository.InvalidarTodosLosTokensDelUsuarioAsync(usuario.Id, _unitOfWork);

                await _unitOfWork.CommitAsync();

                return Result<string>.Success("Contraseña actualizada exitosamente");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result<string>.Failure($"Error al restablecer contraseña: {ex.Message}");
            }
        }
    }
}
