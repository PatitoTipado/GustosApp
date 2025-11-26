
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.Extensions.Logging;

namespace GustosApp.Application.UseCases.RestauranteUseCases
{
    public class SugerirGustosSobreUnRadioUseCase :  IRecomendadorRestaurantes
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

        public async Task<List<Restaurante>> Handle(UsuarioPreferencias usuario,
            List<Restaurante> restaurantesCercanos,
            int maxResults = 10,
            CancellationToken ct = default)
        {
            if (validarUsuarioYRestaurantes(usuario, restaurantesCercanos))
            {
                return new List<Restaurante>();
            }

            var userEmbedding = obtenerEmbeddingDeUsuario(usuario);

            var resultados = new List<(Restaurante rest, double score)>();

            foreach (var rest in restaurantesCercanos)
            {
                var restEmbedding = obtenerRestauranteEmbedding(rest);

                if (restEmbedding == null) continue;

                var similitud = CosineSimilarity.Coseno(userEmbedding, restEmbedding);

                double penalizacion = obtenerPenalizacion(usuario, rest);

                double scoreFinal = similitud * (1 - penalizacion);

                if (scoreFinal >= UmbralMinimo)
                {
                    rest.Score = scoreFinal;
                    resultados.Add((rest, scoreFinal));
                }
            }

            return ordenarResultado(maxResults, resultados);
        }

        private static List<Restaurante> ordenarResultado(int maxResults, List<(Restaurante rest, double score)> resultados)
        {
            return resultados
                .GroupBy(x => x.rest.Id)
                .Select(g => g.First())
                .OrderByDescending(x => x.score)
                .Take(maxResults)
                .Select(x => x.rest)
                .ToList();
        }

        private static double obtenerPenalizacion(UsuarioPreferencias usuario, Restaurante rest)
        {
            int gustosFaltantes = usuario.Gustos.Count(g =>
                !rest.GustosQueSirve.Any(e =>
                    e.Nombre != null &&
                    e.Nombre.Contains(g, StringComparison.OrdinalIgnoreCase)));

            int restriccionesNoCumple = usuario.Restricciones.Count(r =>
                !rest.RestriccionesQueRespeta.Any(e =>
                    e.Nombre != null &&
                    e.Nombre.Contains(r, StringComparison.OrdinalIgnoreCase)));

            double penalizacion =
                (gustosFaltantes + restriccionesNoCumple)
                * FactorPenalizacion
                / (usuario.Gustos.Count + usuario.Restricciones.Count);

            return penalizacion;
        }

        private float[] obtenerRestauranteEmbedding(Restaurante rest)
        {
            var textoRestaurante = string.Join(" ",
                rest.GustosQueSirve.Select(g => g.Nombre)
                .Concat(rest.RestriccionesQueRespeta.Select(r => r.Nombre))
                .Where(s => !string.IsNullOrWhiteSpace(s))
            );

            if (string.IsNullOrWhiteSpace(textoRestaurante))
                return null;

            return _embeddingService.GetEmbedding(textoRestaurante);
        }

        private float[] obtenerEmbeddingDeUsuario(UsuarioPreferencias usuario)
        {
            var textoUsuario = string.Join(" ",
                usuario.Gustos.Concat(usuario.Restricciones)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
            );

            // embedding único del usuario
            return _embeddingService.GetEmbedding(textoUsuario);
        }

        private bool validarUsuarioYRestaurantes(UsuarioPreferencias usuario, List<Restaurante> restaurantesCercanos)
        {
            bool usuarioNoValido = usuario == null || usuario.Gustos == null || !usuario.Gustos.Any();

            bool restauranteNoValido = (restaurantesCercanos == null || !restaurantesCercanos.Any());

            return usuarioNoValido || restauranteNoValido;
        }
    }
}
