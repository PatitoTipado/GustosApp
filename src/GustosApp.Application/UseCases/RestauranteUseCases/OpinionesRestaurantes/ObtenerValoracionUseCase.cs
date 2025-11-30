using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases.RestauranteUseCases.OpinionesRestaurantes

{
    public class ObtenerValoracionUseCase
    {
        private readonly IRestauranteRepository _restauranteRepository;
        private readonly IOpinionRestauranteRepository _opinionRestauranteRepository;
        private readonly IServicioRestaurantes _servicioRestaurantes;

        public ObtenerValoracionUseCase(IRestauranteRepository restauranteRepository,IOpinionRestauranteRepository opinionRestauranteRepository,IServicioRestaurantes servicioRestaurantes)
        {
            _restauranteRepository = restauranteRepository;
            _opinionRestauranteRepository = opinionRestauranteRepository;
            _servicioRestaurantes = servicioRestaurantes;
        }

        public async Task<ValoracionCombinadaResult> HandleAsync(Guid restauranteId, CancellationToken ct)
        {
            var restaurante = await _restauranteRepository.GetRestauranteByIdAsync(restauranteId, ct);
            if (restaurante == null)
            {
                throw new KeyNotFoundException($"Restaurante con ID {restauranteId} no encontrado");
            }

            // Obtener valoraciones de la app
            var valoracionesUsuarios = await _opinionRestauranteRepository.ObtenerPorRestauranteAsync(restauranteId, ct);
            var valoracionesApp = valoracionesUsuarios
                .Where(v => !v.EsImportada && v.UsuarioId.HasValue)
                .ToList();

            double promedioApp = 0;
            int cantidadApp = valoracionesApp.Count;

            if (cantidadApp > 0) 
                promedioApp = valoracionesApp.Average(v => v.Valoracion);

            // Obtener valoraciones de Google Places
            double promedioGoogle = 0;
            int cantidadGoogle = 0;
            bool googleDisponible = false;

            if (!string.IsNullOrWhiteSpace(restaurante.PlaceId))
            {
                var metricaGoogle = await _servicioRestaurantes.ObtenerMetricasGooglePlaces(restaurante.PlaceId, ct);
                if (metricaGoogle != null && metricaGoogle.TotalRatings > 0)
                {
                    promedioGoogle = metricaGoogle.Rating;
                    cantidadGoogle = metricaGoogle.TotalRatings;
                    googleDisponible = true;
                }
                else
                {
                    if (restaurante.Rating.HasValue && restaurante.CantidadResenas.HasValue && restaurante.CantidadResenas.Value > 0)
                    {
                        promedioGoogle = restaurante.Rating.Value;
                        cantidadGoogle = restaurante.CantidadResenas.Value;
                        googleDisponible = true;
                    }
                }
            }

            // Calcular valoracion combinada
            double valoracionCombinada = 0;
            int totalValoraciones = cantidadApp + cantidadGoogle;

            if (totalValoraciones > 0)
            {
                if (cantidadApp > 0 && cantidadGoogle > 0)
                {
                    // Promedio ponderado: (app * cantidadApp + google * cantidadGoogle) / total
                    double sumaPonderada = (promedioApp * cantidadApp) + (promedioGoogle * cantidadGoogle);
                    valoracionCombinada = sumaPonderada / totalValoraciones;
                }
                else if (cantidadApp > 0)
                {
                    valoracionCombinada = promedioApp;
                }
                else if (cantidadGoogle > 0)
                {
                    valoracionCombinada = promedioGoogle;
                }
            }

            return new ValoracionCombinadaResult
            {
                RestauranteId = restauranteId,
                NombreRestaurante = restaurante.Nombre,
                ValoracionCombinada = Math.Round(valoracionCombinada, 2),
                TotalValoraciones = totalValoraciones,
                ValoracionUsuariosApp = new ValoracionFuenteResult
                {
                    Rating = Math.Round(promedioApp, 2),
                    CantidadValoraciones = cantidadApp,
                    Disponible = cantidadApp > 0
                },
                ValoracionGooglePlaces = new ValoracionFuenteResult
                {
                    Rating = Math.Round(promedioGoogle, 2),
                    CantidadValoraciones = cantidadGoogle,
                    Disponible = googleDisponible
                }
            };
        }

         public record ValoracionCombinadaResult
         {
        public Guid RestauranteId { get; init; }
        public string NombreRestaurante { get; init; } = string.Empty;
        public double ValoracionCombinada { get; init; }
          public int TotalValoraciones { get; init; }
           public ValoracionFuenteResult ValoracionUsuariosApp { get; init; } = null!;
           public ValoracionFuenteResult ValoracionGooglePlaces { get; init; } = null!;
        }

    public record ValoracionFuenteResult
    {
        public double Rating { get; init; }
        public int CantidadValoraciones { get; init; }
        public bool Disponible { get; init; }
    }
    }

         
}