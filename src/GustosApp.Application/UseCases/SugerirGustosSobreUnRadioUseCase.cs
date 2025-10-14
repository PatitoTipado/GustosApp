
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

        public List<RestauranteDto> Handle(UsuarioPreferenciasDTO usuario, List<RestauranteDto> filtrados, int maxResults = 10, CancellationToken ct = default)
        {
            //mediante el usuario conseguir los restaurantes que tengan al menos un gusto del usuario y respete por lo menos una restriccion
            List<RestauranteDto> restaurantes = filtrarRestaurante(usuario.Gustos, usuario.Restricciones, filtrados , ct);
            //var restaurantes = await _restaurantRepo.GetAllAsync(ct);
            var resultados = new List<(RestauranteDto restaurante, double score)>();
            var userEmb = _embeddingService.GetEmbedding(string.Join(" ", usuario.Gustos));

            foreach (var rest in restaurantes)
            {
                double maxScoreBase = 0;
                foreach (var especialidad in rest.GustosQueSirve)
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

                foreach (var gusto in usuario.Gustos)
                {
                    if (!rest.GustosQueSirve.Any(e => e.Nombre != null && e.Nombre.ToLower().Contains(gusto.ToLower())))
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
            // 4. Ordenar, limitar y mapear al DTO de respuesta (Corregido)
            var recomendacionesOrdenadas = resultados
                // 4.1. Ordena del Score más alto al más bajo (¡Aplica al listado 'resultados'!)
                .OrderByDescending(x => x.score)

                // 4.2. Toma el número máximo de resultados
                .Take(maxResults)

                // 4.3. Mapea al RestauranteResponse
                .Select(x => new RestauranteDto(
                    // Mapeo de propiedades simples
                    id: x.restaurante.Id,
                    propietarioUid: x.restaurante.PropietarioUid,
                    nombre: x.restaurante.Nombre,
                    direccion: x.restaurante.Direccion,
                    latitud: x.restaurante.Lat,
                    longitud: x.restaurante.Lng,
                    horarios: x.restaurante.Horarios,
                    creadoUtc: x.restaurante.CreadoUtc,
                    actualizadoUtc: x.restaurante.ActualizadoUtc,
                    tipo: x.restaurante.Tipo,
                    imagenUrl: x.restaurante.ImagenUrl,
                    valoracion: x.restaurante.Valoracion,

                    // Mapeo de colecciones
                    platos: new List<string>(),

                    gustosQueSirve: x.restaurante.GustosQueSirve
                        .Select(g => new GustoDto(g.Id, g.Nombre, g.ImagenUrl))
                        .ToList(),

                    restriccionesQueRespeta: x.restaurante.RestriccionesQueRespeta
                        .Select(r => new RestriccionResponse(r.id, r.nombre))
                        .ToList(),

                    // El Score final para mostrar
                    score: x.score
                ))
                .ToList();

            return recomendacionesOrdenadas;
        }

        private List<RestauranteDto> filtrarRestaurante(
            List<string> gustos,
            List<string> restricciones,
            List<RestauranteDto> restaurantes, 
            CancellationToken ct)
        {
            if (restaurantes == null || !restaurantes.Any())
            {
                return new List<RestauranteDto>();
            }

            var gustosNormalizados = gustos?.Select(g => g.ToLower()).ToList() ?? new List<string>();
            var restriccionesNormalizadas = restricciones?.Select(r => r.ToLower()).ToList() ?? new List<string>();

            var restaurantesFiltrados = new List<RestauranteDto>();

            var puntuaciones = new Dictionary<RestauranteDto, int>();

            foreach (var r in restaurantes)
            {
                bool esRestringido = restriccionesNormalizadas.Any() && r.RestriccionesQueRespeta.Any(res => restriccionesNormalizadas.Contains(res.nombre.ToLower()));

                if (esRestringido)
                {
                    continue;
                }

                int puntuacion = 0;

                if (gustosNormalizados.Any())
                {
                    puntuacion = r.GustosQueSirve.Count(g => gustosNormalizados.Contains(g.Nombre.ToLower()));

                    if (puntuacion > 0)
                    {
                        puntuaciones.Add(r, puntuacion);
                    }
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
