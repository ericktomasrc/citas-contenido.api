using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;

namespace CitasContenido.Backend.Domain.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorIdAsync(long id);
        Task<Usuario?> ObtenerPorEmailAsync(string email);
        Task<long> CrearAsync(Usuario usuario, IUnitOfWork unitOfWork);
        Task ActualizarAsync(Usuario usuario,  IUnitOfWork unitOfWork);
        Task<bool> ExisteEmailAsync(string email);
        Task ActualizarUltimaActividadAsync(Guid usuarioId);
    }
}
