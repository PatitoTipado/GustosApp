using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Services
{
    public class RecomendadorRestaurantes 
    {
        private readonly IEmbeddingService _embeddingService;
        private const double UmbralMinimo = 0.1;
        private const double FactorPenalizacion = 0.1;

        public RecomendadorRestaurantes(IEmbeddingService embeddingService)
        {
            _embeddingService = embeddingService;
        }

        public List<Restaurante> GenerarRecomendaciones(
            List<string> gustos,
            List<string> restricciones,
            List<Restaurante> restaurantesDisponibles,
            int maxResults = 10,
            CancellationToken ct = default)
        {
            var restaurantes = FiltrarRestaurantes(gustos, restricciones, restaurantesDisponibles);
            var resultados = new List<(Restaurante restaurante, double score)>();

            var userEmb = _embeddingService.GetEmbedding(string.Join(" ", gustos));

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

                double penalizacion = gustos
                    .Count(g => !rest.GustosQueSirve.Any(e =>
                        e.Nombre != null &&
                        e.Nombre.ToLower().Contains(g.ToLower())))
                    * FactorPenalizacion;

                double scoreFinal = maxScoreBase * (1 - penalizacion);
                if (scoreFinal >= UmbralMinimo)
                {
                    rest.Score = scoreFinal; // propiedad temporal
                    resultados.Add((rest, scoreFinal));
                }
            }

            return resultados
                .OrderByDescending(x => x.score)
                .Take(maxResults)
                .Select(x => x.restaurante)
                .ToList();
        }

        private List<Restaurante> FiltrarRestaurantes(
            List<string> gustos,
            List<string> restricciones,
            List<Restaurante> restaurantes)
        {
            if (restaurantes == null || !restaurantes.Any())
                return new();

            var gustosNorm = gustos.Select(g => g.ToLower()).ToList();
            var restriccionesNorm = restricciones.Select(r => r.ToLower()).ToList();

            var puntuaciones = new Dictionary<Restaurante, int>();

            foreach (var r in restaurantes)
            {
                bool esRestringido = restriccionesNorm.Any() &&
                                     r.RestriccionesQueRespeta.Any(res =>
                                         restriccionesNorm.Contains(res.Nombre.ToLower()));

                if (esRestringido) continue;

                int puntuacion = r.GustosQueSirve.Count(g => gustosNorm.Contains(g.Nombre.ToLower()));

                if (puntuacion > 0)
                    puntuaciones.Add(r, puntuacion);
            }

            return puntuaciones
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToList();
        }
    }
}
