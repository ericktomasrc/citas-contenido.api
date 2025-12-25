using CitasContenido.Shared.Results;

namespace CitasContenido.Backend.Domain.Services
{
    public interface ISubirArchivoDomainService
    {
        Task<Result<string>> SubirArchivoAsync(
            long usuarioId,
            int tipoContenidoId,
            Stream archivoStream,
            string nombreArchivo,
            string extension);
    }
}
