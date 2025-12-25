using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CitasContenido.Backend.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace CitasContenido.Backend.Infraestructure.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public AzureStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureStorage:ConnectionString"]
                ?? throw new ArgumentNullException("AzureStorage:ConnectionString no configurado");

            _containerName = configuration["AzureStorage:ContainerName"]
                ?? throw new ArgumentNullException("AzureStorage:ContainerName no configurado");

            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> SubirArchivoAsync(Stream archivoStream, string nombreArchivo, string extension)
        {
            try
            {
                // Crear container si no existe
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

                // Generar nombre único
                var blobName = $"{Guid.NewGuid()}{extension}";
                var blobClient = containerClient.GetBlobClient(blobName);

                // Determinar content type
                var contentType = ObtenerContentType(extension);

                // Configurar headers
                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                };

                // Subir archivo
                archivoStream.Position = 0;
                await blobClient.UploadAsync(archivoStream, new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al subir archivo a Azure Storage: {ex.Message}", ex);
            }
        }

        public async Task<string> SubirArchivoAsync(byte[] archivoBytes, string nombreArchivo, string extension)
        {
            using var stream = new MemoryStream(archivoBytes);
            return await SubirArchivoAsync(stream, nombreArchivo, extension);
        }

        public async Task<bool> EliminarArchivoAsync(string blobUrl)
        {
            try
            {
                var blobClient = new BlobClient(new Uri(blobUrl));
                var response = await blobClient.DeleteIfExistsAsync();
                return response.Value;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar archivo de Azure Storage: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExisteArchivoAsync(string blobUrl)
        {
            try
            {
                var blobClient = new BlobClient(new Uri(blobUrl));
                return await blobClient.ExistsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar existencia de archivo: {ex.Message}", ex);
            }
        }

        public async Task<Stream> DescargarArchivoAsync(string blobUrl)
        {
            try
            {
                var blobClient = new BlobClient(new Uri(blobUrl));
                var download = await blobClient.DownloadAsync();
                return download.Value.Content;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al descargar archivo de Azure Storage: {ex.Message}", ex);
            }
        }

        private static string ObtenerContentType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".mp4" => "video/mp4",
                ".mov" => "video/quicktime",
                ".avi" => "video/x-msvideo",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }
    }
}
