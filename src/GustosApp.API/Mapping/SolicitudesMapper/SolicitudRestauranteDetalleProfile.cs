using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;

namespace GustosApp.API.Mapping.SolicitudesMapper
{
    public class SolicitudRestauranteDetalleProfile : Profile
    {
        public SolicitudRestauranteDetalleProfile()
        {
            CreateMap<SolicitudRestaurante, SolicitudRestauranteDetalleDto>()
                .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.Usuario.Id))
                .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => $"{src.Usuario.Nombre} {src.Usuario.Apellido}".Trim()))
                .ForMember(dest => dest.UsuarioEmail, opt => opt.MapFrom(src => src.Usuario.Email))
                .ForMember(dest => dest.NombreRestaurante, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.Direccion))
                .ForMember(dest => dest.Latitud, opt => opt.MapFrom(src => src.Latitud))
                .ForMember(dest => dest.Longitud, opt => opt.MapFrom(src => src.Longitud))
                .ForMember(dest => dest.HorariosJson, opt => opt.MapFrom(src => src.HorariosJson))

                .ForMember(dest => dest.Gustos, opt => opt.MapFrom(src =>
                    src.Gustos.Select(g => new ItemSimpleDto
                    {
                        Id = g.Id,
                        Nombre = g.Nombre
                    }).ToList()))

                .ForMember(dest => dest.Restricciones, opt => opt.MapFrom(src =>
                    src.Restricciones.Select(r => new ItemSimpleDto
                    {
                        Id = r.Id,
                        Nombre = r.Nombre
                    }).ToList()))

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

                .ForMember(dest => dest.FechaCreacionUtc, opt => opt.MapFrom(src => src.FechaCreacion))
                .ForMember(dest => dest.Horarios, opt => opt.ConvertUsing(new HorariosJsonConverter(), src => src.HorariosJson));
        }
    }
}
