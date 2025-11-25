using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.GruposMapper
{
    public class InvitacionGrupoProfile : Profile
    {
        public InvitacionGrupoProfile()
        {
            CreateMap<InvitacionGrupo, InvitacionGrupoResponse>()
                .ForMember(dest => dest.GrupoNombre, opt => opt.MapFrom(src => src.Grupo.Nombre))
                .ForMember(dest => dest.UsuarioInvitadoNombre,
                    opt => opt.MapFrom(src => $"{src.UsuarioInvitado.Nombre} {src.UsuarioInvitado.Apellido}"))
                .ForMember(dest => dest.UsuarioInvitadoEmail, opt => opt.MapFrom(src => src.UsuarioInvitado.Email))
                .ForMember(dest => dest.UsuarioInvitadorNombre,
                    opt => opt.MapFrom(src => $"{src.UsuarioInvitador.Nombre} {src.UsuarioInvitador.Apellido}"))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()));
        }
    }
    }
