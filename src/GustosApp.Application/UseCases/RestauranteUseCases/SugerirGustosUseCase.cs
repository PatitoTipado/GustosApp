using GustosApp.Application.DTO;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.Extensions.Logging;

namespace GustosApp.Application.UseCases.RestauranteUseCases
{
    public class SugerirGustosUseCase
    {
        private readonly IRestauranteRepository _restaurantRepo;
        private readonly IEmbeddingService _embeddingService;
        private readonly ILogger<SugerirGustosUseCase> _logger;

        private const double FactorPenalizacion = 0.2;
        private const double UmbralMinimo = 0.05;

        public SugerirGustosUseCase(
            IRestauranteRepository restaurantRepo,
            IEmbeddingService embeddingService,
            ILogger<SugerirGustosUseCase> logger)
        {
            _restaurantRepo = restaurantRepo;
            _embeddingService = embeddingService;
            _logger = logger;
        }

        public virtual async Task<List<Restaurante>> Handle(UsuarioPreferencias usuario, int maxResults = 10, CancellationToken ct = default)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            if (usuario.Gustos == null || !usuario.Gustos.Any())
                return new List<Restaurante>();

            //  Obtener restaurantes candidatos filtrados por gustos y restricciones
            var restaurantes = await _restaurantRepo.buscarRestauranteParaUsuariosConGustosYRestricciones(
                usuario.Gustos,
                usuario.Restricciones,
                ct);

            if (restaurantes == null || !restaurantes.Any())
                return new List<Restaurante>();

            //  Cache de embeddings locales para evitar recomputar
            var embCache = new Dictionary<string, float[]>();

            float[] GetEmbeddingSafe(string texto)
            {
                if (string.IsNullOrWhiteSpace(texto))
                    return new float[768]; // vector neutro

                if (embCache.TryGetValue(texto, out var cached))
                    return cached;

                try
                {
                    var emb = _embeddingService.GetEmbedding(texto);
                    embCache[texto] = emb;
                    return emb;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error generando embedding para '{texto}'. Se usará vector neutro.", texto);
                    var neutral = new float[768];
                    embCache[texto] = neutral;
                    return neutral;
                }
            }

            //  Crear embedding base del usuario
            var userEmb = GetEmbeddingSafe(string.Join(" ", usuario.Gustos));

            var resultados = new List<(Restaurante restaurante, double score)>();

            foreach (var rest in restaurantes)
            {
                double maxScoreBase = 0;

                // Evitar null reference en gustos del restaurante
                var especialidades = rest.GustosQueSirve ?? new List<Gusto>();

                foreach (var especialidad in especialidades)
                {
                    if (string.IsNullOrWhiteSpace(especialidad.Nombre))
                        continue;

                    var baseEmb = GetEmbeddingSafe(especialidad.Nombre);
                    double similitud = CosineSimilarity.Coseno(userEmb, baseEmb);

                    if (similitud > maxScoreBase)
                        maxScoreBase = similitud;
                }

                // Penalización proporcional por gustos no coincidentes
                double penalizacion = 0;
                if (usuario.Gustos.Any())
                {
                    int gustosNoCoincidentes = usuario.Gustos.Count(g =>
                        !especialidades.Any(e =>
                            e.Nombre != null &&
                            e.Nombre.Contains(g, StringComparison.OrdinalIgnoreCase)));

                    penalizacion = (gustosNoCoincidentes * FactorPenalizacion) / usuario.Gustos.Count;
                }

                double scoreFinal = maxScoreBase * (1 - penalizacion);

                if (scoreFinal >= UmbralMinimo)
                {
                    rest.Score = scoreFinal;
                    resultados.Add((rest, scoreFinal));
                }
            }

            //  Eliminar duplicados (por Id)
            var unicos = resultados
                .GroupBy(r => r.restaurante.Id)
                .Select(g => g.First())
                .ToList();

            // Ordenar por score y limitar resultados
            var recomendaciones = unicos
                .OrderByDescending(r => r.score)
                .Take(maxResults)
                .Select(r =>
                {
                    r.restaurante.Score = r.score;
                    return r.restaurante;
                })
                .ToList();

            _logger.LogInformation("Se generaron {count} recomendaciones para usuario con {gustos} gustos y {restricciones} restricciones.",
                recomendaciones.Count, usuario.Gustos.Count, usuario.Restricciones.Count);

            return recomendaciones;
        }
    }

}



