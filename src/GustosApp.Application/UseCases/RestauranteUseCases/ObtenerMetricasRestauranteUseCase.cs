using System;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Application.record;
using System.Data;

namespace GustosApp.Application.UseCases.RestauranteUseCases
{
    public class ObtenerMetricasRestauranteUseCase
    {
        private readonly IRestauranteEstadisticasRepository _estadisticasRepository;
        private readonly IUsuarioRestauranteFavoritoRepository _favoritoRepository;

        public ObtenerMetricasRestauranteUseCase(
            IRestauranteEstadisticasRepository estadisticasRepository,
            IUsuarioRestauranteFavoritoRepository favoritoRepository)
        {
            _estadisticasRepository = estadisticasRepository;
            _favoritoRepository = favoritoRepository;
        }

        public async Task<RestauranteMetricasRecord> HandleAsync(
            Guid restauranteId,
            CancellationToken ct = default)
        {
            if (restauranteId == Guid.Empty)
                throw new ArgumentException("El restauranteId no puede ser vacío.", nameof(restauranteId));

            var estadisticas = await _estadisticasRepository.ObtenerPorRestauranteAsync(restauranteId, ct);

            if (estadisticas==null)
            {
                throw new KeyNotFoundException("No se encontraron estadisticas con esa clave");
            }

            var totalFavoritos = await _favoritoRepository.CountByRestauranteAsync(restauranteId, ct);

            return new RestauranteMetricasRecord( restauranteId , estadisticas, totalFavoritos);
        }
    }
}
