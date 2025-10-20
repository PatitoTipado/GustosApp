using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping
{
    public class ApiMapeoPerfil : Profile
    {
        public ApiMapeoPerfil()
        {

            // CreateMap<Origen, Destino>();
            CreateMap<RegistrarUsuarioRequest, Usuario>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FotoPerfilUrl, opt => opt.MapFrom(src => src.FotoPerfilUrl))
                .ForMember(dest => dest.IdUsuario , opt => opt.MapFrom(src => src.Username));

            CreateMap<Usuario, UsuarioResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirebaseUid, opt => opt.MapFrom(src => src.FirebaseUid))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.IdUsuario))
                .ForMember(dest => dest.FotoPerfilUrl, opt => opt.MapFrom(src => src.FotoPerfilUrl));
                
        }
    }
}
