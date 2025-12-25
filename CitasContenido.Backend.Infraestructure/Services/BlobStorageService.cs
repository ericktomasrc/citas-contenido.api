using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CitasContenido.Backend.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CitasContenido.Backend.Infraestructure.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException("Azure Storage connection string no configurada");

            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = configuration["AzureStorage:ContainerName"] ?? "citascontenido";
        }

        public async Task<string> SubirFotoAsync(IFormFile archivo, string carpeta)
        {
            if (archivo == null || archivo.Length == 0)
                throw new ArgumentException("Archivo inválido");

            // Validar tipo de archivo
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

            if (!extensionesPermitidas.Contains(extension))
                throw new ArgumentException("Tipo de archivo no permitido");

            // Validar tamaño (5MB máximo)
            if (archivo.Length > 5 * 1024 * 1024)
                throw new ArgumentException("El archivo excede el tamaño máximo de 5MB");

            // Generar nombre único
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var blobName = $"{carpeta}/{nombreArchivo}";

            // Obtener container
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Subir archivo
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = archivo.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders
                {
                    ContentType = archivo.ContentType
                });
            }

            return blobClient.Uri.ToString();
        }

        public async Task<bool> EliminarFotoAsync(string urlBlob)
        {
            try
            {
                var uri = new Uri(urlBlob);
                var blobName = uri.AbsolutePath.TrimStart('/').Substring(_containerName.Length + 1);

                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                return await blobClient.DeleteIfExistsAsync();
            }
            catch
            {
                return false;
            }
        }
         
    }
}
