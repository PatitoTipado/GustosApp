using GustosApp.Application.DTO;
using GustosApp.Application.DTOs.Restaurantes;
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

        public async Task<List<RestauranteDto>> Handle(UsuarioPreferenciasDTO usuario, int maxResults = 10, CancellationToken ct = default)
        {
            //mediante el usuario conseguir los restaurantes que tengan al menos un gusto del usuario y respete por lo menos una restriccion
            var restaurantes = await _restaurantRepo.buscarRestauranteParaUsuariosConGustosYRestricciones(usuario.Gustos,usuario.Restricciones, ct);
            //var restaurantes = await _restaurantRepo.GetAllAsync(ct);
            var resultados = new List<(Restaurante restaurante, double score)>();
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
                    latitud: x.restaurante.Latitud,
                    longitud: x.restaurante.Longitud,
                    horarios: x.restaurante.HorariosJson,
                    creadoUtc: x.restaurante.CreadoUtc,
                    actualizadoUtc: x.restaurante.ActualizadoUtc,
                    tipo: null,
                    imagenUrl: x.restaurante.ImagenUrl,
                    valoracion: x.restaurante.Valoracion,

                    // Mapeo de colecciones
                    platos: new List<string>(),
                    
                    gustosQueSirve: x.restaurante.GustosQueSirve
                        .Select(g => new GustoDto(g.Id, g.Nombre, g.ImagenUrl))
                        .ToList(),

                    restriccionesQueRespeta: x.restaurante.RestriccionesQueRespeta
                        .Select(r => new RestriccionResponse(r.Id, r.Nombre))
                        .ToList(),

                    // El Score final para mostrar
                    score: x.score
                ))
                .ToList();

            return recomendacionesOrdenadas;
        }
    }
}
    

    