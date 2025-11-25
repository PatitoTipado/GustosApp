using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.RestaurantesMapper
{
    public class RestauranteFavoritoProfile : Profile
    {
        public RestauranteFavoritoProfile()
        {
            CreateMap<Restaurante, RestauranteFavoritoDto>()
                .ForMember(dest => dest.RestauranteId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));
        }
    }
    }
