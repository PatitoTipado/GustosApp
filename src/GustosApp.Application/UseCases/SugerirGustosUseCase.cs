using GustosApp.Application.DTO;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class SugerirGustosUseCase
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IRestauranteRepository _restaurantRepo;

        private const double UmbralMinimo = 0.1;
        private const double FactorPenalizacion = 0.1;

        public SugerirGustosUseCase(IEmbeddingService embeddingService, IRestauranteRepository restaurantRepo)
        {
            _embeddingService = embeddingService;
            _restaurantRepo = restaurantRepo;
        }

        public async Task<List<RestauranteDto>> Handle(UsuarioPreferenciasDTO usuario, int maxResults = 10, CancellationToken ct = default)
        {
            var restaurantes = await _restaurantRepo.buscarRestauranteParaUsuariosConGustosYRestricciones(
                usuario.Gustos, usuario.Restricciones, ct);

            var resultados = new List<(Restaurante restaurante, double score)>();
            var userEmb = _embeddingService.GetEmbedding(string.Join(" ", usuario.Gustos));

            foreach (var rest in restaurantes)
            {
                double maxScoreBase = 0;
                foreach (var especialidad in rest.GustosQueSirve)
                {
                    var baseEmb = _embeddingService.GetEmbedding(especialidad.Nombre);
                    var scoreSimilitud = CosineSimilarity.Coseno(userEmb, baseEmb);
                    if (scoreSimilitud > maxScoreBase) maxScoreBase = scoreSimilitud;
                }

                double penalizacion = 0;
                foreach (var gusto in usuario.Gustos)
                {
                    if (!rest.GustosQueSirve.Any(e => e.Nombre != null && e.Nombre.ToLower().Contains(gusto.ToLower())))
                        penalizacion += FactorPenalizacion;
                }

                var scoreFinal = maxScoreBase * (1 - penalizacion);
                if (scoreFinal >= UmbralMinimo)
                    resultados.Add((rest, scoreFinal));
            }

            // helper local
            static T? SafeDeserialize<T>(string? json)
            {
                if (string.IsNullOrWhiteSpace(json)) return default;
                try { return System.Text.Json.JsonSerializer.Deserialize<T>(json); }
                catch { return default; }
            }

            var recomendacionesOrdenadas = resultados
                .OrderByDescending(x => x.score)
                .Take(maxResults)
                .Select(x =>
                {
                    var horariosObj = SafeDeserialize<object>(x.restaurante.HorariosJson);
                    var typesList = SafeDeserialize<List<string>>(x.restaurante.TypesJson) ?? new List<string>();

                    return new RestauranteDto(
                        id: x.restaurante.Id,
                        propietarioUid: x.restaurante.PropietarioUid,
                        nombre: x.restaurante.Nombre,
                        direccion: x.restaurante.Direccion,
                        latitud: x.restaurante.Latitud,
                        longitud: x.restaurante.Longitud,
                        horarios: horariosObj,                     
                        creadoUtc: x.restaurante.CreadoUtc,
                        actualizadoUtc: x.restaurante.ActualizadoUtc,
                        primaryType: x.restaurante.PrimaryType,    
                        types: typesList,                          
                        imagenUrl: x.restaurante.ImagenUrl,
                        valoracion: x.restaurante.Valoracion,
                        platos: new List<string>(),                
                        gustosQueSirve: x.restaurante.GustosQueSirve
                            .Select(g => new GustoDto(g.Id, g.Nombre, g.ImagenUrl))
                            .ToList(),
                        restriccionesQueRespeta: x.restaurante.RestriccionesQueRespeta
                            .Select(r => new RestriccionResponse(r.Id, r.Nombre))
                            .ToList(),
                        score: x.score
                    );
                })
                .ToList();

            return recomendacionesOrdenadas;
        }

    }
}


