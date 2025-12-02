using System.Text.Json;
using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;

namespace GustosApp.API.Mapping.RestaurantesMapper
{
    public class RestauranteDetalleProfile : Profile
    {
        public RestauranteDetalleProfile()
        {
            CreateMap<Restaurante, RestauranteDetalleDto>()

                   .ForMember(dest => dest.RatingCalculado,
                           opt => opt.MapFrom(src =>
                          src.Reviews != null && src.Reviews.Any()
                        ? Math.Round(src.Reviews.Average(r => r.Valoracion), 1)
                        : (src.Rating ?? 0)
                ))

                .ForMember(dest => dest.Latitud, opt => opt.MapFrom(src => src.Latitud))
                .ForMember(dest => dest.Longitud, opt => opt.MapFrom(src => src.Longitud))

                .ForMember(dest => dest.EsDeLaApp,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.PlaceId)))

                .ForMember(dest => dest.ImagenDestacada,
                    opt => opt.MapFrom(src => src.ImagenUrl))

                .ForMember(dest => dest.LogoUrl,
                    opt => opt.MapFrom(src => src.LogoUrl))

                .ForMember(dest => dest.ImagenesInterior,
                    opt => opt.MapFrom(src =>
                        src.Imagenes
                            .Where(i => i.Tipo == TipoImagenRestaurante.Interior)
                            .OrderBy(i => i.Orden)
                            .Select(i => i.Url)
                            .ToList()))

                .ForMember(dest => dest.ImagenesComida,
                    opt => opt.MapFrom(src =>
                        src.Imagenes
                            .Where(i => i.Tipo == TipoImagenRestaurante.Comida)
                            .OrderBy(i => i.Orden)
                            .Select(i => i.Url)
                            .ToList()))

                .ForMember(dest => dest.Menu,
                    opt => opt.MapFrom(src =>
                        src.Menu != null &&
                        !string.IsNullOrWhiteSpace(src.Menu.Json)
                            ? DeserializeMenu(src)
                            : null))

                .ForMember(dest => dest.ReviewsLocales,
                    opt => opt.MapFrom(src =>
                        src.Reviews
                            .Where(r => !r.EsImportada)
                            .OrderByDescending(r => r.FechaCreacion)))

                .ForMember(dest => dest.ReviewsGoogle,
                    opt => opt.MapFrom(src =>
                        src.Reviews
                            .Where(r => r.EsImportada)
                            .OrderByDescending(r => r.FechaCreacion)))
                .ForMember(dest => dest.GustosQueSirve,
                    opt => opt.MapFrom(src =>
                        src.GustosQueSirve != null
                            ? src.GustosQueSirve.Select(g => new GustoDto
                            {
                                Id = g.Id,
                                Nombre = g.Nombre
                            }).ToList()
                    : new List<GustoDto>()
                    ))

                .ForMember(dest => dest.RestriccionesQueRespeta, opt => opt.MapFrom(src =>
                    src.RestriccionesQueRespeta.Select(r => new RestriccionResponse(r.Id, r.Nombre))
                    ));
        }

        private static RestauranteMenuDto? DeserializeMenu(Restaurante r)
        {
            try
            {
                if (r.Menu == null) return null;
                if (string.IsNullOrWhiteSpace(r.Menu.Json)) return null;

                return JsonSerializer.Deserialize<RestauranteMenuDto>(r.Menu.Json);
            }
            catch { return null; }
        }
    }
}
