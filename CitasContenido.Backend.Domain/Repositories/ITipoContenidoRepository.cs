using CitasContenido.Backend.Domain.Entities;

namespace CitasContenido.Backend.Domain.Repositories
{
    public interface ITipoContenidoRepository
    {
        Task<TipoContenido?> ObtenerPorIdAsync(int id);
        Task<TipoContenido?> ObtenerPorNombreAsync(string nombre);
        Task<IEnumerable<TipoContenido>> ObtenerTodosAsync();
        Task<IEnumerable<TipoContenido>> ObtenerPorTipoUsuarioAsync(bool esCreador);
    }
}
