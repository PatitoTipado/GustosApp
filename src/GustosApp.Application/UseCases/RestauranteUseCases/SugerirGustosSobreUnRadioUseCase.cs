
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
        private readonly IRestauranteRepository _restauranteRepository;

        private const double UmbralMinimo = 0.05;
        private const double UmbralRelevanciaResena = 0.60;
        private const double FactorPenalizacion = 0.15;

        // impacto final de reseñas
        private const double BoostPositivo = 1.02;   // +2%
        private const double PenalizacionNegativa = 0.95; // -5%

        public SugerirGustosSobreUnRadioUseCase(
            IEmbeddingService embeddingService,
            IRestauranteRepository restauranteRepository)
        {
            _embeddingService = embeddingService;
            _restauranteRepository = restauranteRepository;
        }

        public async Task<List<Restaurante>> Handle(UsuarioPreferencias usuario,
            List<Restaurante> restaurantesCercanos,
            int maxResults = 10,
            CancellationToken ct = default)
        {
            if (validarUsuarioYRestaurantes(usuario, restaurantesCercanos))
                return new List<Restaurante>();

            float[] userEmbedding = ObtenerEmbeddingDeUsuario(usuario);

            List<(Restaurante rest, double score)> resultados =
                similitudUsuarioRestauranteResultado(usuario, restaurantesCercanos, userEmbedding);

            if (!resultados.Any()) return new List<Restaurante>();

            List<Restaurante> restaurantesConResenias = await consultarRestaurantesConResenias(resultados);

            resultadosValoracionPorResenias(userEmbedding, resultados, restaurantesConResenias);

            return ordenarResultado(maxResults, resultados);
        }

        private void resultadosValoracionPorResenias(float[] userEmbedding, List<(Restaurante rest, double score)> resultados, List<Restaurante> restaurantesConResenias)
        {
            foreach (var restConRes in restaurantesConResenias)
            {
                var item = resultados.FirstOrDefault(x => x.rest.Id == restConRes.Id);
                if (item.rest == null) continue;

                double ajuste = calcularAjustePorResenas(restConRes, userEmbedding);

                double nuevoScore = item.score * ajuste;

                item.rest.Score = nuevoScore;

                resultados.RemoveAll(x => x.rest.Id == restConRes.Id);
                resultados.Add((item.rest, nuevoScore));
            }
        }

        private async Task<List<Restaurante>> consultarRestaurantesConResenias(List<(Restaurante rest, double score)> resultados)
        {
            var ids = resultados.Select(r => r.rest.Id).ToList();
            List<Restaurante> restaurantesConResenias =
                await _restauranteRepository.obtenerRestauranteConResenias(ids);
            return restaurantesConResenias;
        }

        private double calcularAjustePorResenas(Restaurante rest, float[] userEmbedding)
        {
            if (rest.Reviews == null || !rest.Reviews.Any())
                return 1.0;

            var ajustes = new List<double>();

            foreach (var resena in rest.Reviews)
            {
                if (string.IsNullOrWhiteSpace(resena.Opinion))
                    continue;

                float[] embeddingResena = _embeddingService.GetEmbedding(resena.Opinion);

                double relevancia = CosineSimilarity.Coseno(userEmbedding, embeddingResena);

                if (relevancia < UmbralRelevanciaResena)
                    continue; 

                if (resena.Valoracion >= 3)
                    ajustes.Add(BoostPositivo);
                else
                    ajustes.Add(PenalizacionNegativa);
            }

            if (!ajustes.Any()) return 1.0;

            return ajustes.Average();
        }

        private List<(Restaurante rest, double score)> similitudUsuarioRestauranteResultado(
            UsuarioPreferencias usuario,
            List<Restaurante> restaurantesCercanos,
            float[] userEmbedding)
        {
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

            return resultados;
        }

        private static List<Restaurante> ordenarResultado(int maxResults,
            List<(Restaurante rest, double score)> resultados)
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

        private float[] ObtenerEmbeddingDeUsuario(UsuarioPreferencias usuario)
        {
            var textoUsuario = string.Join(" ",
                usuario.Gustos.Concat(usuario.Restricciones)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
            );

            return _embeddingService.GetEmbedding(textoUsuario);
        }

        private bool validarUsuarioYRestaurantes(UsuarioPreferencias usuario,
            List<Restaurante> restaurantesCercanos)
        {
            bool usuarioNoValido = usuario == null || usuario.Gustos == null || !usuario.Gustos.Any();
            bool restauranteNoValido = restaurantesCercanos == null || !restaurantesCercanos.Any();

            return usuarioNoValido || restauranteNoValido;
        }
    }

}
