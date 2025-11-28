using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.RestaurantesMapper
{
    public class RestauranteAleatorioProfile : Profile
    {
        public RestauranteAleatorioProfile()
        {
            // Mapping normal (Restaurante → DTO)
            CreateMap<Restaurante, RestauranteAleatorioResponse>()
                .ForMember(dest => dest.Gustos,
                    opt => opt.MapFrom(src =>
                        src.GustosQueSirve != null
                            ? src.GustosQueSirve.Select(g => g.Nombre).ToList()
                            : new List<string>()))
                .ForMember(dest => dest.Restricciones,
                    opt => opt.MapFrom(src =>
                        src.RestriccionesQueRespeta != null
                            ? src.RestriccionesQueRespeta.Select(r => r.Nombre).ToList()
                            : new List<string>()));

            // Reverse (DTO → Restaurante)
            CreateMap<RestauranteAleatorioResponse, Restaurante>()
                .ForMember(dest => dest.GustosQueSirve,
                    opt => opt.MapFrom(src =>
                        src.Gustos != null
                            ? src.Gustos.Select(nombre => new Gusto { Nombre = nombre }).ToList()
                            : new List<Gusto>()))
                .ForMember(dest => dest.RestriccionesQueRespeta,
                    opt => opt.MapFrom(src =>
                        src.Restricciones != null
                            ? src.Restricciones.Select(nombre => new Restriccion { Nombre = nombre }).ToList()
                            : new List<Restriccion>()))
                // Campos que no están en el DTO los ignoro
                .ForMember(dest => dest.NombreNormalizado, opt => opt.Ignore())
                .ForMember(dest => dest.CreadoUtc, opt => opt.Ignore())
                .ForMember(dest => dest.ActualizadoUtc, opt => opt.Ignore())
                .ForMember(dest => dest.Dueno, opt => opt.Ignore())
                .ForMember(dest => dest.DuenoId, opt => opt.Ignore())
                .ForMember(dest => dest.PropietarioUid, opt => opt.Ignore())
                .ForMember(dest => dest.HorariosJson, opt => opt.Ignore())
                .ForMember(dest => dest.UltimaActualizacion, opt => opt.Ignore())
                .ForMember(dest => dest.EmbeddingVector, opt => opt.Ignore())
                .ForMember(dest => dest.MenuProcesado, opt => opt.Ignore())
                .ForMember(dest => dest.MenuError, opt => opt.Ignore())
                .ForMember(dest => dest.Estadisticas, opt => opt.Ignore())
                .ForMember(dest => dest.Menu, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.Platos, opt => opt.Ignore())
                .ForMember(dest => dest.Imagenes, opt => opt.Ignore())
                .ForMember(dest => dest.PrimaryType, opt => opt.Ignore())
                .ForMember(dest => dest.TypesJson, opt => opt.Ignore())
                .ForMember(dest => dest.LogoUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Score, opt => opt.Ignore());
        }
    }
}
