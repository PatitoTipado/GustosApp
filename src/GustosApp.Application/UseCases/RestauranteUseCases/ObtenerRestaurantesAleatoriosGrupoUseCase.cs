using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases.RestauranteUseCases
{
    public class ObtenerRestaurantesAleatoriosGrupoUseCase
    {
        private readonly IGustosGrupoRepository _gustosGrupoRepository;
        private readonly IRestauranteRepository _restauranteRepository;
        private readonly IGrupoRepository _grupoRepository;

        public ObtenerRestaurantesAleatoriosGrupoUseCase(
            IGustosGrupoRepository gustosGrupoRepository,
            IRestauranteRepository restauranteRepository,
            IGrupoRepository grupoRepository)
        {
            _gustosGrupoRepository = gustosGrupoRepository;
            _restauranteRepository = restauranteRepository;
            _grupoRepository = grupoRepository;
        }

        public async Task<List<Restaurante>> HandleAsync(
            Guid grupoId,
            RestauranteAleatorio request,
            CancellationToken ct)
        {
            // Verificar que el grupo existe
            var grupo = await _grupoRepository.GetByIdAsync(grupoId, ct);
            if (grupo == null)
            {
                throw new ArgumentException("El grupo no existe", nameof(grupoId));
            }

            // Obtener los IDs de los gustos del grupo
            var gustosIds = await _gustosGrupoRepository.ObtenerGustosIdsDelGrupo(grupoId);

            if (!gustosIds.Any())
            {
                return new List<Restaurante>();
            }

            // Buscar restaurantes que coincidan con los gustos del grupo
            var restaurantes = await _restauranteRepository.ObtenerRestaurantesPorGustosGrupo(gustosIds, ct);

            if (!restaurantes.Any())
            {
                return new List<Restaurante>();
            }

            // Si se proporcionó ubicación y radio, filtrar por cercanía
            if (request.Latitud.HasValue && request.Longitud.HasValue && request.RadioMetros.HasValue)
            {
                restaurantes = restaurantes.Where(r =>
                {
                    var distancia = CalcularDistancia(
                        request.Latitud.Value, request.Longitud.Value,
                        r.Latitud, r.Longitud);
                    return distancia <= request.RadioMetros.Value;
                }).ToList();
            }

            // Seleccionar restaurantes aleatorios
            var random = new Random();
            var cantidad = Math.Min(request.Cantidad, restaurantes.Count);
            var restaurantesAleatorios = restaurantes
                .OrderBy(x => random.Next())
                .Take(cantidad)
                .ToList();


            return restaurantesAleatorios;
        }

        // Método auxiliar para calcular distancia entre dos puntos (fórmula de Haversine)
        private double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
        {
            const double radioTierra = 6371000; // Radio de la Tierra en metros
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return radioTierra * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
