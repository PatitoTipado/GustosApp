using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.UsuariosMapper
{
    public class UsuarioSimpleProfile : Profile
    {
        public UsuarioSimpleProfile()
        {
            CreateMap<Usuario, UsuarioSimpleResponse>()
                .ForMember(dest => dest.Username,
                    opt => opt.MapFrom(src => src.IdUsuario))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Nombre,
                    opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.FotoPerfilUrl,
                    opt => opt.MapFrom(src => src.FotoPerfilUrl));
        }
    }
}
