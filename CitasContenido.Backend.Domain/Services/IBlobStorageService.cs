using Microsoft.AspNetCore.Http;

namespace CitasContenido.Backend.Domain.Services
{
    public interface IBlobStorageService
    {
        Task<string> SubirFotoAsync(IFormFile archivo, string carpeta);
        Task<bool> EliminarFotoAsync(string urlBlob);
    }
}
