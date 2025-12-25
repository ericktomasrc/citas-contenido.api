using CitasContenido.Backend.Domain.Entities;

namespace CitasContenido.Backend.Domain.Repositories
{
    public interface IVerificacionIdentidadRepository
    {
        Task<long> CrearAsync(VerificacionIdentidad verificacion, Common.IUnitOfWork unitOfWork);
        Task<VerificacionIdentidad?> ObtenerPorUsuarioIdAsync(Guid usuarioId);
        Task ActualizarAsync(VerificacionIdentidad verificacion);
    }
}
