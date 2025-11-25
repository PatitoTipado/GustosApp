using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.UsuariosMapper
{
    public class UsuarioResumenProfile : Profile
    {
        public UsuarioResumenProfile()
        {
            CreateMap<Usuario, UsuarioResumenResponse>()
                .ForMember(dest => dest.Restricciones, opt => opt.MapFrom(src => src.Restricciones))
                .ForMember(dest => dest.CondicionesMedicas, opt => opt.MapFrom(src => src.CondicionesMedicas))
                .ForMember(dest => dest.Gustos, opt => opt.MapFrom(src => src.Gustos));
        }
    }
}