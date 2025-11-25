using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.PreferenciasMapper
{
    public class CondicionMedicaProfile : Profile
    {
        public CondicionMedicaProfile()
        {
            CreateMap<CondicionMedica, ItemResumen>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre));

            CreateMap<CondicionMedica, CondicionMedicaResponse>()
                .ForMember(dest => dest.Seleccionado, opt => opt.Ignore());
        }
    }
}
