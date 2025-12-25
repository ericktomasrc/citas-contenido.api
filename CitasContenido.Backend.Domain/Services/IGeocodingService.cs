using CitasContenido.Backend.Domain.DTOs;

namespace CitasContenido.Backend.Domain.Services
{
    public interface IGeocodingService
    {
        Task<UbicacionDto> ObtenerUbicacionPorCoordenadas(decimal latitud, decimal longitud);
    }
}
