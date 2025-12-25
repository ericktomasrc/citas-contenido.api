using CitasContenido.Backend.Domain.Entities;

namespace CitasContenido.Backend.Domain.Repositories
{
    public interface ITipoDocumentoRepository
    {
        Task<TipoDocumento?> GetByIdAsync(int id);
        Task<IEnumerable<TipoDocumento>> GetAllAsync();
        Task<IEnumerable<TipoDocumento>> GetHabilitadosAsync();
        Task<IEnumerable<TipoDocumento>> BuscarAsync(string? filtro, bool? soloHabilitados);
        Task<int> CreateAsync(TipoDocumento tipoDocumento);
        Task<bool> UpdateAsync(TipoDocumento tipoDocumento);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
    }
}
