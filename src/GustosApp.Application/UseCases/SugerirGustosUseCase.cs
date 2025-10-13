using GustosApp.Application.DTO;
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

        public async Task<List<RecomendacionDTO>> Handle(List<string> gustosUsuario, int maxResults = 10, CancellationToken ct = default)
        {
            var restaurantes = await _restaurantRepo.GetAllAsync(ct);
            var resultados = new List<(Restaurante restaurante, double score)>();
            var userEmb = _embeddingService.GetEmbedding(string.Join(" ", gustosUsuario));

            foreach (var rest in restaurantes)
            {
                double maxScoreBase = 0;
                foreach (var especialidad in rest.Especialidad)
                {
                    var baseEmb = _embeddingService.GetEmbedding(especialidad.Nombre);
                    double scoreSimilitud = CosineSimilarity.Coseno(userEmb, baseEmb);

                    if (scoreSimilitud > maxScoreBase)
                    {
                        maxScoreBase = scoreSimilitud;
                    }
                }
                double scoreBase = maxScoreBase;
                double penalizacion = 0;

                foreach (var gusto in gustosUsuario)
                {
                    if (!rest.Especialidad.Any(e => e.Nombre != null && e.Nombre.ToLower().Contains(gusto.ToLower())))
                    {
                        penalizacion += FactorPenalizacion;
                    }
                }

                double scoreFinal = scoreBase * (1 - penalizacion);

                if (scoreFinal >= UmbralMinimo)
                {
                    resultados.Add((rest, scoreFinal));
                }
            }
            return resultados
                .OrderByDescending(x => x.score)
                .Take(maxResults)
                .Select(x => new RecomendacionDTO { RestaurantId =  x.restaurante.Id ,Score = x.score })
                .ToList();

        }
    }
}
    

    