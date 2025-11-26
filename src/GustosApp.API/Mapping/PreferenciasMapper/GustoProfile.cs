using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.PreferenciasMapper
{
    public class GustoProfile : Profile
    {
        public GustoProfile()
        {
            CreateMap<Gusto, ItemResumen>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre));

            CreateMap<Gusto, GustoDto>()
                .ForMember(dest => dest.Seleccionado, opt => opt.Ignore());
        }
    }
    }
