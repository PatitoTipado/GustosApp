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
                .ForMember(dest => dest.FotoPerfilUrl, opt => opt.MapFrom(src => src.FotoPerfilUrl))
                .ForMember(dest => dest.Plan, opt => opt.MapFrom(src => src.Plan.ToString()));


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

            CreateMap<Valoracion, CrearValoracionResponse>()
                .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => src.Usuario!.Nombre))
                .ForMember(dest => dest.UsuarioApellido, opt => opt.MapFrom(src => src.Usuario!.Apellido))
                .ForMember(dest => dest.RestauranteNombre, opt => opt.MapFrom(src => src.Restaurante!.NombreNormalizado))
                .ForMember(dest => dest.RestauranteLatitud, opt => opt.MapFrom(src => src.Restaurante!.Latitud))
                .ForMember(dest => dest.RestauranteLongitud, opt => opt.MapFrom(src => src.Restaurante!.Longitud));

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


            
            CreateMap<MiembroGrupo, MiembroGrupoResponse>()
                .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId))
                .ForMember(dest => dest.UsuarioFirebaseUid, opt => opt.MapFrom(src => src.Usuario.FirebaseUid))
                .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => $"{src.Usuario.Nombre} {src.Usuario.Apellido}"))
                .ForMember(dest => dest.UsuarioEmail, opt => opt.MapFrom(src => src.Usuario.Email))
                .ForMember(dest => dest.UsuarioUsername, opt => opt.MapFrom(src => src.Usuario.IdUsuario))
                .ForMember(dest => dest.FechaUnion, opt => opt.MapFrom(src => src.FechaUnion))
                .ForMember(dest => dest.EsAdministrador, opt => opt.MapFrom(src => src.EsAdministrador));



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


            CreateMap<InvitacionGrupo, InvitacionGrupoResponse>()
            .ForMember(dest => dest.GrupoNombre, opt => opt.MapFrom(src => src.Grupo.Nombre))
            .ForMember(dest => dest.UsuarioInvitadoNombre,
                opt => opt.MapFrom(src => $"{src.UsuarioInvitado.Nombre} {src.UsuarioInvitado.Apellido}"))
             .ForMember(dest => dest.UsuarioInvitadoEmail, opt => opt.MapFrom(src => src.UsuarioInvitado.Email))
             .ForMember(dest => dest.UsuarioInvitadorNombre,
                   opt => opt.MapFrom(src => $"{src.UsuarioInvitador.Nombre} {src.UsuarioInvitador.Apellido}"))
             .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()));


            CreateMap<ChatMensaje, ChatMensajeResponse>();


            CreateMap<Restaurante, RestauranteDto>()
            .ForMember(dest => dest.Latitud, opt => opt.MapFrom(src => src.Latitud))
            .ForMember(dest => dest.Longitud, opt => opt.MapFrom(src => src.Longitud))
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score))
            .ForMember(dest => dest.GustosQueSirve, opt => opt.MapFrom(src =>
                 src.GustosQueSirve.Select(g => new GustoDto(g.Id, g.Nombre, g.ImagenUrl))))
            .ForMember(dest => dest.RestriccionesQueRespeta, opt => opt.MapFrom(src =>
                 src.RestriccionesQueRespeta.Select(r => new RestriccionResponse(r.Id, r.Nombre))));

            CreateMap<Restaurante, RestauranteListadoDto>()
           .ForMember(dest => dest.PrimaryType, opt => opt.MapFrom(src => src.Categoria));

            CreateMap<Gusto, GustoDto>()
            .ForMember(dest => dest.Seleccionado, opt => opt.Ignore());

        }
    }
}

