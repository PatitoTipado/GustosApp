using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.RestaurantesMapper
{
    public class RestauranteProfile : Profile
    {
        public RestauranteProfile()
        {
            CreateMap<Restaurante, RestauranteDTO>()
                .ForMember(dest => dest.Latitud, opt => opt.MapFrom(src => src.Latitud))
                .ForMember(dest => dest.Longitud, opt => opt.MapFrom(src => src.Longitud))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score))
                .ForMember(dest => dest.GustosQueSirve, opt => opt.MapFrom(src =>
                    src.GustosQueSirve.Select(g => new GustoDto(g.Id, g.Nombre, g.ImagenUrl))))
                .ForMember(dest => dest.RestriccionesQueRespeta, opt => opt.MapFrom(src =>
                    src.RestriccionesQueRespeta.Select(r => new RestriccionResponse(r.Id, r.Nombre))))
                .ForMember(dest => dest.GooglePlaceId, opt => opt.MapFrom(src => src.PlaceId));
        }
    }
}
