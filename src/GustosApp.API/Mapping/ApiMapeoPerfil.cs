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


            CreateMap<Restriccion, RestriccionDto>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
           .ForMember(dest => dest.Seleccionado, opt => opt.Ignore());

            CreateMap<Usuario, UsuarioResumenResponse>()
                .ForMember(dest => dest.Gustos, opt => opt.MapFrom
                (src => src.Gustos.Select(g => g.Nombre).ToList()))
                .ForMember(dest => dest.Restricciones, opt => opt.MapFrom
                (src => src.Restricciones.Select(r => r.Nombre).ToList()))
                .ForMember(dest => dest.CondicionesMedicas, opt => opt.MapFrom
                (src => src.CondicionesMedicas.Select(c => c.Nombre).ToList()));


            CreateMap<Notificacion, NotificacionDTO>()
           .ForMember(dest => dest.Tipo,
               opt => opt.MapFrom(src => src.Tipo.ToString()));

            CreateMap<Usuario, UsuarioSimpleResponse>()
                .ForMember(dest => dest.Username,
                opt => opt.MapFrom(src => src.IdUsuario));

            
            CreateMap<SolicitudAmistad, SolicitudAmistadResponse>()
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()))
                .ForMember(dest => dest.Remitente, opt => opt.MapFrom(src => src.Remitente))
                .ForMember(dest => dest.Destinatario, opt => opt.MapFrom(src => src.Destinatario));



            CreateMap<CondicionMedica, CondicionMedicaResponse>()
           .ForMember(dest => dest.Seleccionado, opt => opt.Ignore());

        }
    }
}

