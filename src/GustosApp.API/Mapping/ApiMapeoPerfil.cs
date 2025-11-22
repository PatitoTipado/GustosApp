using System.Text.Json;
using AutoMapper;

using GustosApp.API.DTO;

using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;

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
                .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.Username));

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
                .ForMember(dest => dest.Restricciones, opt => opt.MapFrom(src => src.Restricciones))
                .ForMember(dest => dest.CondicionesMedicas, opt => opt.MapFrom(src => src.CondicionesMedicas))
                .ForMember(dest => dest.Gustos, opt => opt.MapFrom(src => src.Gustos));

            CreateMap<Restriccion, ItemResumen>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre));

            CreateMap<CondicionMedica, ItemResumen>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre));

            CreateMap<Gusto, ItemResumen>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre));

            CreateMap<OpinionRestaurante, CrearOpinionRestauranteResponse>()
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
                .ForMember(dest => dest.afectarRecomendacion, opt => opt.MapFrom(src => src.afectarRecomendacion))
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


            CreateMap<Restaurante, RestauranteDTO>()
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

            CreateMap<SolicitudRestaurante, SolicitudRestaurantePendienteDto>()
                .ForMember(dest => dest.NombreRestaurante, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.Direccion))
                .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Nombre : ""))
                .ForMember(dest => dest.UsuarioEmail, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Email : ""))
                .ForMember(dest => dest.FechaCreacionUtc, opt => opt.MapFrom(src => src.FechaCreacion))
                .ForMember(dest => dest.imgLogo,opt => opt.MapFrom(src =>
                src.Imagenes
            .Where(i => i.Tipo == TipoImagenSolicitud.Logo)
            .Select(i => i.Url)
            .FirstOrDefault() 

             ))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado));
            CreateMap<SolicitudRestaurante, SolicitudRestauranteDetalleDto>()

                .ForMember(dest => dest.UsuarioId,
                    opt => opt.MapFrom(src => src.Usuario.Id))
                .ForMember(dest => dest.UsuarioNombre,
                    opt => opt.MapFrom(src => $"{src.Usuario.Nombre} {src.Usuario.Apellido}".Trim()))
                .ForMember(dest => dest.UsuarioEmail,
                    opt => opt.MapFrom(src => src.Usuario.Email))

                .ForMember(dest => dest.NombreRestaurante,
                    opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Direccion,
                    opt => opt.MapFrom(src => src.Direccion))
                .ForMember(dest => dest.Latitud,
                    opt => opt.MapFrom(src => src.Latitud))
                .ForMember(dest => dest.Longitud,
                    opt => opt.MapFrom(src => src.Longitud))

                .ForMember(dest => dest.HorariosJson,
                    opt => opt.MapFrom(src => src.HorariosJson))

                .ForMember(dest => dest.Gustos,
                    opt => opt.MapFrom(src =>
                        src.Gustos.Select(g => new ItemSimpleDto
                        {
                            Id = g.Id,
                            Nombre = g.Nombre
                        }).ToList()
                    ))

                .ForMember(dest => dest.Restricciones,
                    opt => opt.MapFrom(src =>
                        src.Restricciones.Select(r => new ItemSimpleDto
                        {
                            Id = r.Id,
                            Nombre = r.Nombre
                        }).ToList()
                    ))

                .ForMember(dest => dest.ImagenesDestacadas,
                    opt => opt.MapFrom(src =>
                        src.Imagenes
                            .Where(i => i.Tipo == TipoImagenSolicitud.Destacada)
                            .Select(i => i.Url)
                            .FirstOrDefault() ?? string.Empty))

                .ForMember(dest => dest.ImagenesInterior,
                    opt => opt.MapFrom(src =>
                        src.Imagenes
                            .Where(i => i.Tipo == TipoImagenSolicitud.Interior)
                            .Select(i => i.Url)
                            .ToList()))

                .ForMember(dest => dest.ImagenesComida,
                    opt => opt.MapFrom(src =>
                        src.Imagenes
                            .Where(i => i.Tipo == TipoImagenSolicitud.Comida)
                            .Select(i => i.Url)
                            .ToList()))

                .ForMember(dest => dest.ImagenMenu,
                    opt => opt.MapFrom(src =>
                        src.Imagenes
                            .Where(i => i.Tipo == TipoImagenSolicitud.Menu)
                            .Select(i => i.Url)
                            .FirstOrDefault()))

                .ForMember(dest => dest.Logo,
                    opt => opt.MapFrom(src =>
                        src.Imagenes
                            .Where(i => i.Tipo == TipoImagenSolicitud.Logo)
                            .Select(i => i.Url)
                            .FirstOrDefault()))

                .ForMember(dest => dest.FechaCreacionUtc,
                    opt => opt.MapFrom(src => src.FechaCreacion))

                // Conversión final de horarios JSON a objeto
                .ForMember(dest => dest.Horarios,
                    opt => opt.ConvertUsing(new HorariosJsonConverter(), src => src.HorariosJson));

            CreateMap<Restaurante, RestauranteDetalleDto>()

                  .ForMember(dest => dest.Latitud,
                 opt => opt.MapFrom(src =>  src.Latitud))

                   .ForMember(dest => dest.Longitud,
                 opt => opt.MapFrom(src => src.Longitud))

                    .ForMember(dest => dest.Longitud,
                 opt => opt.MapFrom(src => src.Longitud))

                .ForMember(dest => dest.EsDeLaApp,
                 opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.PlaceId)))

           .ForMember(dest => dest.ImagenDestacada,
               opt => opt.MapFrom(src => src.ImagenUrl)) // Tipo 0
           .ForMember(dest => dest.LogoUrl,
               opt => opt.MapFrom(src => src.LogoUrl)) // Tipo 4
           .ForMember(dest => dest.ImagenesInterior,
               opt => opt.MapFrom(src =>
                   src.Imagenes
                       .Where(i => i.Tipo == TipoImagenRestaurante.Interior)
                       .OrderBy(i => i.Orden)
                       .Select(i => i.Url)
                       .ToList()
               ))
           .ForMember(dest => dest.ImagenesComida,
               opt => opt.MapFrom(src =>
                   src.Imagenes
                       .Where(i => i.Tipo == TipoImagenRestaurante.Comida)
                       .OrderBy(i => i.Orden)
                       .Select(i => i.Url)
                       .ToList()
               ))
       .ForMember(dest => dest.Menu,
    opt => opt.MapFrom(src =>
        src.Menu != null &&
        !string.IsNullOrWhiteSpace(src.Menu.Json)
            ? DeserializeMenu(src)
            : null
    ))


           // Reviews Locales
           .ForMember(dest => dest.ReviewsLocales,
               opt => opt.MapFrom(src =>
                   src.Reviews
                       .Where(r => !r.EsImportada)
                       .OrderByDescending(r => r.FechaCreacion)
               ))
           // Reviews Google
           .ForMember(dest => dest.ReviewsGoogle,
               opt => opt.MapFrom(src =>
                   src.Reviews
                       .Where(r => r.EsImportada)
                       .OrderByDescending(r => r.FechaCreacion)
               ))
           ;

          
            CreateMap<OpinionRestaurante, OpinionRestauranteDto>()

                .ForMember(dest => dest.Autor,
                    opt => opt.MapFrom(src =>
                        src.AutorExterno ??
                        (src.Usuario != null ? $"{src.Usuario.Nombre} {src.Usuario.Apellido}" : "Usuario")
                    ))
                .ForMember(dest => dest.Fecha,
                    opt => opt.MapFrom(src => src.FechaCreacion))
                .ForMember(dest => dest.ImagenAutor,
                    opt => opt.MapFrom(src => src.ImagenAutorExterno))

                .ForMember(dest => dest.Fotos,
                 opt => opt.MapFrom(src =>
            src.Fotos != null
                ? src.Fotos.Select(f => f.Url).ToList()
                : new List<string>()
        ));


        }

        private static RestauranteMenuDto? DeserializeMenu(Restaurante r)
        {
            try
            {
                // Si no existe el menú → null
                if (r.Menu == null)
                    return null;

                if (string.IsNullOrWhiteSpace(r.Menu.Json))
                    return null;

                // Deserialize sencillo
                return JsonSerializer.Deserialize<RestauranteMenuDto>(r.Menu.Json);
            }
            catch
            {
                return null;
            }
        }

    }

}

    




    
    


