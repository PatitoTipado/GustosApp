using GustosApp.Application.DTO;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.RestauranteUseCases
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

        public async Task<List<Restaurante>> Handle(UsuarioPreferencias usuario, int maxResults = 10, CancellationToken ct = default)
        {
            var restaurantes = await _restaurantRepo.buscarRestauranteParaUsuariosConGustosYRestricciones(
              usuario.Gustos,
              usuario.Restricciones,
              ct);

            var resultados = new List<(Restaurante restaurante, double score)>();

            //  2. Crear embedding de los gustos del usuario
            var userEmb = _embeddingService.GetEmbedding(string.Join(" ", usuario.Gustos));

            foreach (var rest in restaurantes)
            {
                double maxScoreBase = 0;

                // Comparar embeddings con cada gusto del restaurante
                foreach (var especialidad in rest.GustosQueSirve)
                {
                    var baseEmb = _embeddingService.GetEmbedding(especialidad.Nombre);
                    double scoreSimilitud = CosineSimilarity.Coseno(userEmb, baseEmb);

                    if (scoreSimilitud > maxScoreBase)
                        maxScoreBase = scoreSimilitud;
                }

                double penalizacion = 0;
                foreach (var gusto in usuario.Gustos)
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
                {

                    rest.Score = scoreFinal;
                    resultados.Add((rest, scoreFinal));
                }
            }

            //  3. Ordenar por score y limitar
            var recomendaciones = resultados
                .OrderByDescending(x => x.score)
                .Take(maxResults)
                .Select(x =>
                {
                    x.restaurante.Score = x.score;
                    return x.restaurante;
                })
                .ToList();

            return recomendaciones;
        }
    }
}



