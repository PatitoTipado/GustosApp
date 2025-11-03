
using GustosApp.Application.DTO;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class SugerirGustosSobreUnRadioUseCase
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IRestauranteRepository _restaurantRepo;

        private const double UmbralMinimo = 0.1;
        private const double FactorPenalizacion = 0.1;

        public SugerirGustosSobreUnRadioUseCase(IEmbeddingService embeddingService, IRestauranteRepository restaurantRepo)
        {
            _embeddingService = embeddingService;
            _restaurantRepo = restaurantRepo;
        }

        public List<Restaurante> Handle( UsuarioPreferencias preferencias,List<Restaurante> restaurantesFiltrados,
            int maxResults = 10, CancellationToken ct = default)
        {
            List<Restaurante> restaurantes = filtrarRestaurante(
                preferencias.Gustos,
                preferencias.Restricciones,
                restaurantesFiltrados,
                ct);

            var resultados = new List<(Restaurante restaurante, double score)>();
            var userEmb = _embeddingService.GetEmbedding(string.Join(" ", preferencias.Gustos));

            foreach (var rest in restaurantes)
            {
                double maxScoreBase = 0;

                foreach (var especialidad in rest.GustosQueSirve)
                {
                    var baseEmb = _embeddingService.GetEmbedding(especialidad.Nombre);
                    double scoreSimilitud = CosineSimilarity.Coseno(userEmb, baseEmb);

                    if (scoreSimilitud > maxScoreBase)
                        maxScoreBase = scoreSimilitud;
                }

                double penalizacion = 0;
                foreach (var gusto in preferencias.Gustos)
                {
                    if (!rest.GustosQueSirve.Any(e =>
                        e.Nombre != null &&
                        e.Nombre.ToLower().Contains(gusto.ToLower())))
                    {
                        penalizacion += FactorPenalizacion;
                    }
                }

                double scoreFinal = maxScoreBase * (1 - penalizacion);
                if (scoreFinal >= UmbralMinimo)
                    resultados.Add((rest, scoreFinal));
            }

            // Ordenar, limitar y devolver entidades del dominio
            var recomendacionesOrdenadas = resultados
                .OrderByDescending(x => x.score)
                .Take(maxResults)
                .Select(x =>
                {
                    x.restaurante.Score = x.score; // si agregás Score como propiedad temporal en la entidad
                    return x.restaurante;
                })
                .ToList();

            return recomendacionesOrdenadas;
        }
        private List<Restaurante> filtrarRestaurante( List<string> gustos, List<string> restricciones,
                List<Restaurante> restaurantes,CancellationToken ct)
        {
            if (restaurantes == null || !restaurantes.Any())
                return new List<Restaurante>();

            var gustosNormalizados = gustos?.Select(g => g.ToLower()).ToList() ?? new();
            var restriccionesNormalizadas = restricciones?.Select(r => r.ToLower()).ToList() ?? new();

            var puntuaciones = new Dictionary<Restaurante, int>();

            foreach (var r in restaurantes)
            {
                bool esRestringido = restriccionesNormalizadas.Any() &&
                                     r.RestriccionesQueRespeta.Any(res =>
                                         restriccionesNormalizadas.Contains(res.Nombre.ToLower()));

                if (esRestringido)
                    continue;

                int puntuacion = 0;

                if (gustosNormalizados.Any())
                {
                    puntuacion = r.GustosQueSirve.Count(g =>
                        gustosNormalizados.Contains(g.Nombre.ToLower()));

                    if (puntuacion > 0)
                        puntuaciones.Add(r, puntuacion);
                }
                else
                {
                    puntuaciones.Add(r, 0);
                }
            }

            return puntuaciones
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToList();
        }
    }
    }
