using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using Microsoft.Extensions.Configuration;

namespace CitasContenido.Backend.Infraestructure.Services
{
    public class RegistrarEmailDomainService : IRegistrarEmailDomainService
    {
        private readonly IConfiguration _configuration;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IVerificacionEmailRepository _verificacionEmailRepository;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _diasmMaximo;

        public RegistrarEmailDomainService(
            IUsuarioRepository usuarioRepository,
            IVerificacionEmailRepository verificacionEmailRepository,
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            IConfiguration configuration
            )
        {          
            _configuration = configuration;
            _diasmMaximo = int.Parse(configuration["Configuracion:DiasMaximo"] ?? "24");
            _usuarioRepository = usuarioRepository;
            _verificacionEmailRepository = verificacionEmailRepository;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> RegistrarEmailAsync(string email)
        {
            try
            {
                // Verificar si el email ya existe
                var usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);

                if (usuario != null)
                {
                    //   CASO 1: Email existe pero NO verificado
                    if (!usuario.EmailVerificado)
                    {
                        // Verificar si tiene token válido
                        var tieneTokenValido = await _verificacionEmailRepository.ExisteTokenValidoAsync(usuario.Id);

                        if (tieneTokenValido)
                        {
                            return Result<string>.Failure(
                                "Ya existe un email de verificación pendiente. Revisa tu bandeja de entrada y spam. " +
                                "Si no lo encuentras, solicita un reenvío."
                            );
                        }

                        // No tiene token válido, crear uno nuevo y reenviar
                        await _unitOfWork.BeginTransactionAsync();

                        var nuevoToken = Guid.NewGuid().ToString("N");
                        var verificacion = VerificacionEmail.Crear(usuario.Id, nuevoToken, horasValidez: _diasmMaximo);
                        await _verificacionEmailRepository.CrearAsync(verificacion, _unitOfWork);

                        await _unitOfWork.CommitAsync();

                        try
                        {
                            await _emailService.EnviarEmailVerificacionAsync(email, nuevoToken, email);
                            return Result<string>.Success("Email de verificación reenviado. Por favor revisa tu correo.");
                        }
                        catch (Exception emailEx)
                        {
                            Console.WriteLine($"Error al enviar email: {emailEx.Message}");
                            return Result<string>.Failure("Error al enviar el email. Intenta más tarde.");
                        }
                    }

                    //   CASO 2: Email verificado pero registro NO completado
                    if (string.IsNullOrEmpty(usuario.Nombre) || string.IsNullOrEmpty(usuario.PasswordHash))
                    {
                        return Result<string>.Success($"CONTINUAR_REGISTRO|{usuario.Id}");
                    }

                    //   CASO 3: Registro completado
                    return Result<string>.Failure("El email ya está registrado. Por favor inicia sesión.");
                }

                //   CASO 4: Email NO existe - Registro nuevo
                await _unitOfWork.BeginTransactionAsync();

                var nuevoUsuario = Usuario.CrearConEmail(email);
                var id = await _usuarioRepository.CrearAsync(nuevoUsuario, _unitOfWork);
                nuevoUsuario.ActualizarId(id);

                var token = Guid.NewGuid().ToString("N");
                var nuevaVerificacion = VerificacionEmail.Crear(nuevoUsuario.Id, token, horasValidez: _diasmMaximo);
                await _verificacionEmailRepository.CrearAsync(nuevaVerificacion, _unitOfWork);

                await _unitOfWork.CommitAsync();

                // Enviar email fuera de transacción
                try
                {
                    await _emailService.EnviarEmailVerificacionAsync(email, token, email);
                }
                catch (Exception emailEx)
                {
                    Console.WriteLine($"Error al enviar email: {emailEx.Message}");
                    return Result<string>.Success(
                        "Registro exitoso, pero hubo un problema al enviar el email. " +
                        "Por favor, solicita un reenvío del email de verificación."
                    );
                }

                return Result<string>.Success("Email de verificación enviado. Por favor revisa tu correo.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception($"Error al registrar email: {ex.Message}", ex);
            }
        }

        public async Task<Result<string>> ReenviarEmailVerificacionAsync(string email)
        {
            try
            {
                // Buscar usuario por email
                var usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);
                if (usuario == null)
                {
                    return Result<string>.Failure("Email no registrado");
                }

                // Verificar si ya está verificado
                if (usuario.EmailVerificado)
                {
                    return Result<string>.Failure("Este email ya está verificado");
                }

                // Verificar si ya existe un token válido
                var tieneTokenValido = await _verificacionEmailRepository.ExisteTokenValidoAsync(usuario.Id);

                string token;

                if (!tieneTokenValido)
                {
                    // Crear nuevo token si no existe uno válido
                    await _unitOfWork.BeginTransactionAsync();

                    token = Guid.NewGuid().ToString("N");
                    var verificacion = VerificacionEmail.Crear(usuario.Id, token, horasValidez: _diasmMaximo);
                    await _verificacionEmailRepository.CrearAsync(verificacion, _unitOfWork);

                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    // Obtener el token existente
                    // Nota: Necesitarás agregar este método al repositorio
                    return Result<string>.Failure("Ya existe un email de verificación válido. Revisa tu bandeja de entrada y spam.");
                }

                // Enviar email
                try
                {
                    await _emailService.EnviarEmailVerificacionAsync(email, token, email);
                    return Result<string>.Success("Email de verificación reenviado. Por favor revisa tu correo.");
                }
                catch (Exception emailEx)
                {
                    return Result<string>.Failure($"Error al enviar el email: {emailEx.Message}");
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception($"Error al reenviar email de verificación: {ex.Message}", ex);
            }
        }
    }
}
