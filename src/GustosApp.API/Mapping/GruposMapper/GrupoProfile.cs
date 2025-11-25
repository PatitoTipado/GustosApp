using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.GruposMapper
{
    public class GrupoProfile : Profile
    {
        public GrupoProfile()
        {
            CreateMap<Grupo, GrupoResponse>()
                .ForMember(dest => dest.AdministradorFirebaseUid,
                    opt => opt.MapFrom(src => src.Administrador != null ? src.Administrador.FirebaseUid : null))
                .ForMember(dest => dest.AdministradorNombre,
                    opt => opt.MapFrom(src => src.Administrador != null
                        ? $"{src.Administrador.Nombre} {src.Administrador.Apellido}"
                        : string.Empty))
                .ForMember(dest => dest.CantidadMiembros,
                    opt => opt.MapFrom(src => src.Miembros.Count(m => m.Activo)))
                .ForMember(dest => dest.Miembros,
                    opt => opt.MapFrom(src => src.Miembros.Where(m => m.Activo)));
        }
    }
}
