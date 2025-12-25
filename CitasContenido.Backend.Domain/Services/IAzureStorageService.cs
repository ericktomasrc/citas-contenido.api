namespace CitasContenido.Backend.Domain.Services
{
    public interface IAzureStorageService
    {
        Task<string> SubirArchivoAsync(Stream archivoStream, string nombreArchivo, string extension);
        Task<string> SubirArchivoAsync(byte[] archivoBytes, string nombreArchivo, string extension);
        Task<bool> EliminarArchivoAsync(string blobUrl);
        Task<bool> ExisteArchivoAsync(string blobUrl);
        Task<Stream> DescargarArchivoAsync(string blobUrl);
    }
}
