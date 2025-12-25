using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;

namespace CitasContenido.Backend.Infraestructure.Services
{
    public class SubirArchivoDomainService : ISubirArchivoDomainService
    {
        private readonly ITipoContenidoRepository _tipoContenidoRepository;
        private readonly IContenidoArchivoRepository _contenidoArchivoRepository;
        private readonly IAzureStorageService _azureStorageService;

        public SubirArchivoDomainService(
            ITipoContenidoRepository tipoContenidoRepository,
            IContenidoArchivoRepository contenidoArchivoRepository,
            IAzureStorageService azureStorageService)
        {
            _tipoContenidoRepository = tipoContenidoRepository;
            _contenidoArchivoRepository = contenidoArchivoRepository;
            _azureStorageService = azureStorageService;
        }

        public async Task<Result<string>> SubirArchivoAsync(
            long usuarioId,
            int tipoContenidoId,
            Stream archivoStream,
            string nombreArchivo,
            string extension)
        {
            // 1. Validar tipo de contenido existe
            var tipoContenido = await _tipoContenidoRepository.ObtenerPorIdAsync(tipoContenidoId);
            if (tipoContenido == null)
            {
                return Result<string>.Failure("Tipo de contenido no válido");
            }

            // 2. Validar límite de archivos
            var cantidadActual = await _contenidoArchivoRepository
                .ContarPorUsuarioYTipoAsync(usuarioId, tipoContenidoId);

            if (tipoContenido.LimiteArchivos.HasValue &&
                cantidadActual >= tipoContenido.LimiteArchivos.Value)
            {
                return Result<string>.Failure(
                    $"Ya alcanzaste el límite de {tipoContenido.LimiteArchivos} archivos para {tipoContenido.Nombre}");
            }

            // 3. Validar tamaño del archivo
            if (tipoContenido.TamañoMaximoMB.HasValue)
            {
                var tamañoMB = archivoStream.Length / (1024.0 * 1024.0);
                if (tamañoMB > tipoContenido.TamañoMaximoMB.Value)
                {
                    return Result<string>.Failure(
                        $"El archivo excede el tamaño máximo de {tipoContenido.TamañoMaximoMB}MB");
                }
            }

            // 4. Subir a Azure Storage
            var blobUrl = await _azureStorageService.SubirArchivoAsync(
                archivoStream,
                nombreArchivo,
                extension);

            return Result<string>.Success(blobUrl);
        }
    }
}
