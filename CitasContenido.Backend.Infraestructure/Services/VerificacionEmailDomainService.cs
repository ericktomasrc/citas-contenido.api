using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;

namespace CitasContenido.Backend.Infraestructure.Services
{
    public class VerificacionEmailDomainService : IVerificacionEmailDomainService
    {
        private readonly IVerificacionEmailRepository _verificacionEmailRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VerificacionEmailDomainService(
            IVerificacionEmailRepository verificacionEmailRepository,
            IUsuarioRepository usuarioRepository,
            IUnitOfWork unitOfWork)
        {
            _verificacionEmailRepository = verificacionEmailRepository;
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<long>> VerificarEmailAsync(string token)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                //   LÓGICA DE NEGOCIO: Buscar verificación por token
                var verificacion = await _verificacionEmailRepository.ObtenerPorTokenAsync(token);
                if (verificacion == null)
                {
                    return Result<long>.Failure("Token de verificación inválido");
                }

                //   REGLA DE NEGOCIO: No permitir verificación duplicada
                if (verificacion.Verificado)
                {
                    return Result<long>.Failure("Este email ya ha sido verificado");
                }

                //   REGLA DE NEGOCIO: Token debe estar vigente
                if (verificacion.EstaExpirado())
                {
                    return Result<long>.Failure("El token de verificación ha expirado");
                }

                //   LÓGICA DE NEGOCIO: Obtener usuario
                var usuario = await _usuarioRepository.ObtenerPorIdAsync(verificacion.UsuarioId);
                if (usuario == null)
                {
                    return Result<long>.Failure("Usuario no encontrado");
                }

                //   REGLA DE NEGOCIO: Verificar email requiere actualizar ambas entidades
                verificacion.Verificar();
                await _verificacionEmailRepository.ActualizarAsync(verificacion, _unitOfWork);

                usuario.VerificarEmail();
                await _usuarioRepository.ActualizarAsync(usuario, _unitOfWork);

                //   Commit de la transacción
                await _unitOfWork.CommitAsync();

                return Result<long>.Success(usuario.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception($"Error al verificar email: {ex.Message}", ex);
            }
        }
    }
}
