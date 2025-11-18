using GustosApp.Application.DTO;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.Extensions.Logging;

namespace GustosApp.Application.UseCases.RestauranteUseCases
{
    public class SugerirGustosSobreUnRadioUseCase
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly ILogger<SugerirGustosSobreUnRadioUseCase> _logger;

        private const double UmbralMinimo = 0.05;
        private const double FactorPenalizacion = 0.15;

        public SugerirGustosSobreUnRadioUseCase(
            IEmbeddingService embeddingService,
            ILogger<SugerirGustosSobreUnRadioUseCase> logger)
        {
            _embeddingService = embeddingService;
            _logger = logger;
        }

        public async Task<List<Restaurante>> Handle(
            UsuarioPreferencias usuario,
            List<Restaurante> restaurantesCercanos,
            int maxResults = 10,
            CancellationToken ct = default)
        {
            if (usuario == null || usuario.Gustos == null || !usuario.Gustos.Any())
                return new List<Restaurante>();

            if (restaurantesCercanos == null || !restaurantesCercanos.Any())
                return new List<Restaurante>();

            var embCache = new Dictionary<string, float[]>();

            float[] GetEmbedding(string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                    return new float[768];

                if (embCache.TryGetValue(text, out var cached))
                    return cached;

                try
                {
                    var emb = _embeddingService.GetEmbedding(text);
                    embCache[text] = emb;
                    return emb;
                }
                catch
                {
                    var empty = new float[768];
                    embCache[text] = empty;
                    return empty;
                }
            }

            // embedding único del usuario
            var userEmbedding = GetEmbedding(string.Join(" ", usuario.Gustos));

            var resultados = new List<(Restaurante rest, double score)>();

            foreach (var rest in restaurantesCercanos)
            {
                double maxSimilitud = 0;

                var especialidades = rest.GustosQueSirve ?? new List<Gusto>();

                // 1similitud por embeddings
                foreach (var esp in especialidades)
                {
                    if (string.IsNullOrWhiteSpace(esp.Nombre)) continue;

                    var embEsp = GetEmbedding(esp.Nombre);
                    var similitud = CosineSimilarity.Coseno(userEmbedding, embEsp);

                    if (similitud > maxSimilitud)
                        maxSimilitud = similitud;
                }

                // penalización por gustos faltantes
                int gustosNoCoinciden = usuario.Gustos.Count(g =>
                    !especialidades.Any(e =>
                        e.Nombre != null &&
                        e.Nombre.Contains(g, StringComparison.OrdinalIgnoreCase)));

                double penalizacion = (gustosNoCoinciden * FactorPenalizacion) / usuario.Gustos.Count;

                double scoreFinal = maxSimilitud * (1 - penalizacion);

                if (scoreFinal >= UmbralMinimo)
                {
                    rest.Score = scoreFinal;
                    resultados.Add((rest, scoreFinal));
                }
            }

            return resultados
                .GroupBy(x => x.rest.Id)
                .Select(g => g.First())
                .OrderByDescending(x => x.score)
                .Take(maxResults)
                .Select(x => x.rest)
                .ToList();
        }
    }
}
