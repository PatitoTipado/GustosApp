using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.RestaurantesMapper
{
    public class OpinionRestauranteProfile : Profile
    {
        public OpinionRestauranteProfile()
        {
            CreateMap<OpinionRestaurante, OpinionRestauranteDto>()
                .ForMember(dest => dest.Autor,
                    opt => opt.MapFrom(src =>
                        src.AutorExterno ??
                        (src.Usuario != null ? $"{src.Usuario.Nombre} {src.Usuario.Apellido}" : "Usuario")
                    ))
                .ForMember(dest => dest.Fecha, opt => opt.MapFrom(src => src.FechaCreacion))
                .ForMember(dest => dest.ImagenAutor, opt => opt.MapFrom(src => src.ImagenAutorExterno))
                .ForMember(dest => dest.Fotos,
                    opt => opt.MapFrom(src =>
                        src.Fotos != null
                            ? src.Fotos.Select(f => f.Url).ToList()
                            : new List<string>()));
        }
    }
}
