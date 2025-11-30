using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.UsuariosMapper
{
    public class UsuarioGustosProfile: Profile
    {

        public UsuarioGustosProfile()
        {
            CreateMap<GustosFiltradosUsuarioDTO, UsuarioGustos>()
                .ForMember(dest => dest.GustosFiltrados, opt => opt.MapFrom(src => src.GustosFiltrados))
                .ForMember(dest => dest.GustosSeleccionados, opt => opt.MapFrom(src => src.GustosSeleccionados));
        }

    }
}
