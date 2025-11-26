using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;

namespace GustosApp.API.Mapping.SolicitudesMapper
{
    public class SolicitudRestauranteProfile : Profile
    {
        public SolicitudRestauranteProfile()
        {
            CreateMap<SolicitudRestaurante, SolicitudRestaurantePendienteDto>()
                .ForMember(dest => dest.NombreRestaurante, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.Direccion))
                .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Nombre : ""))
                .ForMember(dest => dest.UsuarioEmail, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Email : ""))
                .ForMember(dest => dest.FechaCreacionUtc, opt => opt.MapFrom(src => src.FechaCreacion))
                .ForMember(dest => dest.imgLogo, opt => opt.MapFrom(src =>
                    src.Imagenes
                        .Where(i => i.Tipo == TipoImagenSolicitud.Logo)
                        .Select(i => i.Url)
                        .FirstOrDefault()))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado));
        }
    }
}
