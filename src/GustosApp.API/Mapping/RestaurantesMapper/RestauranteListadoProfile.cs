using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.RestaurantesMapper
{
    public class RestauranteListadoProfile : Profile
    {
        public RestauranteListadoProfile()
        {
            CreateMap<Restaurante, RestauranteListadoDto>()
                .ForMember(dest => dest.PrimaryType, opt => opt.MapFrom(src => src.Categoria));
        }
    }
}
