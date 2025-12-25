using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.DTOs;
using CitasContenido.Backend.Domain.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CitasContenido.Backend.Infraestructure.Services
{
    public class NominatimGeocodingService : IGeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NominatimGeocodingService> _logger;

        public NominatimGeocodingService(
            IHttpClientFactory httpClientFactory,
            ILogger<NominatimGeocodingService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "CitasContenido/1.0");
            _logger = logger;
        }

        public async Task<UbicacionDto> ObtenerUbicacionPorCoordenadas(
            decimal latitud,
            decimal longitud)
        {
            try
            {
                var url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitud}&lon={longitud}&accept-language=es";

                // Respetar rate limit de Nominatim (1 req/segundo)
                await Task.Delay(1100);

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<NominatimResponse>(json);

                if (data?.Address == null)
                    return new UbicacionDto();

                return new UbicacionDto
                {
                    Pais = Normalizar(data.Address.Country),
                    Departamento = Normalizar(data.Address.State ?? data.Address.Region),
                    Provincia = Normalizar(data.Address.County ?? data.Address.Province),
                    Distrito = Normalizar(data.Address.Municipality ?? data.Address.Suburb),
                    Ciudad = Normalizar(data.Address.City ?? data.Address.Town),
                    DireccionCompleta = data.DisplayName ?? ""
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en geocoding: {Lat}, {Lon}", latitud, longitud);
                return new UbicacionDto
                {
                    Pais = "DESCONOCIDO",
                    Departamento = "DESCONOCIDO"
                };
            }
        }

        private string Normalizar(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            // Normalizar: MAYÚSCULAS, sin tildes, sin espacios extra
            return texto.Trim().ToUpperInvariant()
                .Replace("Á", "A").Replace("É", "E")
                .Replace("Í", "I").Replace("Ó", "O")
                .Replace("Ú", "U").Replace("Ñ", "N");
        }
    }
}
