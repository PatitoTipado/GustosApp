
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.Extensions.Logging;

namespace GustosApp.Application.UseCases.RestauranteUseCases
{
    public class SugerirGustosSobreUnRadioUseCase : IRecomendadorRestaurantes
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IRestauranteRepository _restauranteRepository;

        private const double UMBRAL_MINIMO = 0.05;
        private const double UMBRAL_RELEVANCIA_RESENA = 0.60;
        private const double FACTOR_PENALIZACION = 0.15;

        private const double BOOST_POSITIVO = 1.02;
        private const double PENALIZACION_NEGATIVA = 0.95;

        public SugerirGustosSobreUnRadioUseCase(
            IEmbeddingService embeddingService,
            IRestauranteRepository restauranteRepository)
        {
            _embeddingService = embeddingService;
            _restauranteRepository = restauranteRepository;
        }

        public async Task<List<Restaurante>> Handle(
            UsuarioPreferencias usuario,
            List<Restaurante> restaurantesCercanos,
            int maxResultados = 10,
            CancellationToken ct = default)
        {
            if (ValidarEntrada(usuario, restaurantesCercanos))
                throw new KeyNotFoundException(
                    "No se encontraron restaurantes cercanos que coincidan con los gustos del usuario o el usuario no tiene gustos validos");

            float[] embeddingUsuario = ObtenerEmbeddingUsuario(usuario);

            List<(Restaurante rest, double puntuacion)> resultados =
                CalcularSimilitudUsuarioRestaurante(usuario, restaurantesCercanos, embeddingUsuario);

            if (!resultados.Any())
                throw new KeyNotFoundException("No se obtuvo ningun restaurante en la zona para el usuario");

            List<Restaurante> restaurantesConResenas =
                await ConsultarRestaurantesConResenas(resultados);

            AjustarPuntuacionPorResenas(embeddingUsuario, resultados, restaurantesConResenas);

            return OrdenarResultados(maxResultados, resultados);
        }

        private void AjustarPuntuacionPorResenas(
            float[] embeddingUsuario,
            List<(Restaurante rest, double puntuacion)> resultados,
            List<Restaurante> restaurantesConResenas)
        {
            foreach (var restConResenas in restaurantesConResenas)
            {
                var item = resultados.FirstOrDefault(x => x.rest.Id == restConResenas.Id);
                if (item.rest == null) continue;

                double ajuste = CalcularAjustePorResenas(restConResenas, embeddingUsuario);

                double nuevaPuntuacion = item.puntuacion * ajuste;

                item.rest.Score = nuevaPuntuacion;

                resultados.RemoveAll(x => x.rest.Id == restConResenas.Id);
                resultados.Add((item.rest, nuevaPuntuacion));
            }
        }

        private async Task<List<Restaurante>> ConsultarRestaurantesConResenas(
            List<(Restaurante rest, double puntuacion)> resultados)
        {
            var ids = resultados.Select(r => r.rest.Id).ToList();
            return await _restauranteRepository.obtenerRestauranteConResenias(ids);
        }

        private double CalcularAjustePorResenas(Restaurante rest, float[] embeddingUsuario)
        {
            if (rest.Reviews == null || !rest.Reviews.Any())
                return 1.0;

            var ajustes = new List<double>();

            foreach (var resena in rest.Reviews)
            {
                if (string.IsNullOrWhiteSpace(resena.Opinion))
                    continue;

                float[] embeddingResena = _embeddingService.GetEmbedding(resena.Opinion);

                double relevancia = CosineSimilarity.Coseno(embeddingUsuario, embeddingResena);

                if (relevancia < UMBRAL_RELEVANCIA_RESENA)
                    continue;

                if (resena.Valoracion >= 3)
                    ajustes.Add(BOOST_POSITIVO);
                else
                    ajustes.Add(PENALIZACION_NEGATIVA);
            }

            return ajustes.Any() ? ajustes.Average() : 1.0;
        }

        private List<(Restaurante rest, double puntuacion)> CalcularSimilitudUsuarioRestaurante(
            UsuarioPreferencias usuario,
            List<Restaurante> restaurantesCercanos,
            float[] embeddingUsuario)
        {
            var resultados = new List<(Restaurante rest, double puntuacion)>();

            foreach (var rest in restaurantesCercanos)
            {
                var embeddingRest = ObtenerEmbeddingRestaurante(rest);
                if (embeddingRest == null) continue;

                var similitud = CosineSimilarity.Coseno(embeddingUsuario, embeddingRest);

                double penalizacion = CalcularPenalizacion(usuario, rest);

                double puntuacionFinal = similitud * (1 - penalizacion);

                if (puntuacionFinal >= UMBRAL_MINIMO)
                {
                    rest.Score = puntuacionFinal;
                    resultados.Add((rest, puntuacionFinal));
                }
            }

            return resultados;
        }

        private static List<Restaurante> OrdenarResultados(
            int maxResultados,
            List<(Restaurante rest, double puntuacion)> resultados)
        {
            return resultados
                .GroupBy(x => x.rest.Id)
                .Select(g => g.First())
                .OrderByDescending(x => x.puntuacion)
                .Take(maxResultados)
                .Select(x => x.rest)
                .ToList();
        }

        private static double CalcularPenalizacion(UsuarioPreferencias usuario, Restaurante rest)
        {
            int gustosFaltantes = usuario.Gustos.Count(g =>
                !rest.GustosQueSirve.Any(e =>
                    e.Nombre != null &&
                    e.Nombre.Contains(g, StringComparison.OrdinalIgnoreCase)));

            int restriccionesIncumplidas = usuario.Restricciones.Count(r =>
                !rest.RestriccionesQueRespeta.Any(e =>
                    e.Nombre != null &&
                    e.Nombre.Contains(r, StringComparison.OrdinalIgnoreCase)));

            double penalizacion =
                (gustosFaltantes + restriccionesIncumplidas)
                * FACTOR_PENALIZACION
                / (usuario.Gustos.Count + usuario.Restricciones.Count);

            return penalizacion;
        }

        private float[] ObtenerEmbeddingRestaurante(Restaurante rest)
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

        private float[] ObtenerEmbeddingUsuario(UsuarioPreferencias usuario)
        {
            var textoUsuario = string.Join(" ",
                usuario.Gustos.Concat(usuario.Restricciones)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
            );

            return _embeddingService.GetEmbedding(textoUsuario);
        }

        private bool ValidarEntrada(UsuarioPreferencias usuario,
            List<Restaurante> restaurantesCercanos)
        {
            bool usuarioNoValido = usuario == null || usuario.Gustos == null || !usuario.Gustos.Any();
            bool restaurantesInvalidos = restaurantesCercanos == null || !restaurantesCercanos.Any();

            return usuarioNoValido || restaurantesInvalidos;
        }
    }
}