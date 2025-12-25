using CitasContenido.Backend.Domain.Entities;

namespace CitasContenido.Backend.Domain.Repositories
{
    public interface IVerificacionEmailRepository
    {
        Task<VerificacionEmail?> ObtenerPorTokenAsync(string token);
        Task<long> CrearAsync(VerificacionEmail verificacion, Common.IUnitOfWork unitOfWork);
        Task ActualizarAsync(VerificacionEmail verificacion, Common.IUnitOfWork unitOfWork);
        Task<bool> ExisteTokenValidoAsync(long usuarioId);
    }
}
