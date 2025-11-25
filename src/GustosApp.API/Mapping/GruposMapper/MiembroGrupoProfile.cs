using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.GruposMapper
{
    public class MiembroGrupoProfile : Profile
    {
        public MiembroGrupoProfile()
        {
            CreateMap<MiembroGrupo, MiembroGrupoResponse>()
                .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId))
                .ForMember(dest => dest.UsuarioFirebaseUid, opt => opt.MapFrom(src => src.Usuario.FirebaseUid))
                .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => $"{src.Usuario.Nombre} {src.Usuario.Apellido}"))
                .ForMember(dest => dest.UsuarioEmail, opt => opt.MapFrom(src => src.Usuario.Email))
                .ForMember(dest => dest.UsuarioUsername, opt => opt.MapFrom(src => src.Usuario.IdUsuario))
                .ForMember(dest => dest.FechaUnion, opt => opt.MapFrom(src => src.FechaUnion))
                .ForMember(dest => dest.afectarRecomendacion, opt => opt.MapFrom(src => src.afectarRecomendacion))
                .ForMember(dest => dest.EsAdministrador, opt => opt.MapFrom(src => src.EsAdministrador));
        }
    }
}
