using System.Text.Json.Serialization;

namespace CitasContenido.Backend.Domain.Common
{
    public class NominatimResponse
    {
        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("address")]
        public NominatimAddress? Address { get; set; }
    }
}
