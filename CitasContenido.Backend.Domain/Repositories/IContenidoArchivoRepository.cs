using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;

namespace CitasContenido.Backend.Domain.Repositories
{
    public interface IContenidoArchivoRepository
    {
        Task<long> CrearAsync(ContenidoArchivo contenido, IUnitOfWork unitOfWork);
        Task<ContenidoArchivo?> ObtenerPorIdAsync(long id);
        Task<IEnumerable<ContenidoArchivo>> ObtenerPorUsuarioAsync(long usuarioId);
        Task<IEnumerable<ContenidoArchivo>> ObtenerPorUsuarioYTipoAsync(long usuarioId, int tipoContenidoId);
        Task<ContenidoArchivo?> ObtenerFotoPrincipalAsync(long usuarioId);
        Task<int> ContarPorUsuarioYTipoAsync(long usuarioId, int tipoContenidoId);
        Task<bool> ActualizarAsync(ContenidoArchivo contenido);
        Task<bool> EliminarAsync(long id);
        Task DesmarcarTodasComoPrincipalAsync(long usuarioId);
    }
}
