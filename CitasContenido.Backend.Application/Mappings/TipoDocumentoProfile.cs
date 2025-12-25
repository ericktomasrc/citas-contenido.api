using AutoMapper;
using CitasContenido.Backend.Domain.DTOs;
using CitasContenido.Backend.Domain.Entities;

namespace CitasContenido.Backend.Application.Mappings
{
    public class TipoDocumentoProfile : Profile
    {
        public TipoDocumentoProfile()
        {
            CreateMap<TipoDocumento, TipoDocumentoDto>();
        }
    }
}
