using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.PreferenciasMapper
{
    public class RestriccionProfile : Profile
    {
        public RestriccionProfile()
        {
            CreateMap<Restriccion, RestriccionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Seleccionado, opt => opt.Ignore());

            CreateMap<Restriccion, ItemResumen>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre));
        }
    }
    }
