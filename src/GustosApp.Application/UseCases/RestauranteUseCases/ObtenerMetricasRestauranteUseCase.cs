using System;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;

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

        public async Task<RestauranteMetricasDashboardResponse> HandleAsync(
            Guid restauranteId,
            CancellationToken ct = default)
        {
            if (restauranteId == Guid.Empty)
                throw new ArgumentException("El restauranteId no puede ser vacío.", nameof(restauranteId));

            var estadisticas = await _estadisticasRepository.ObtenerPorRestauranteAsync(restauranteId, ct);

            var totalFavoritos = await _favoritoRepository.CountByRestauranteAsync(restauranteId, ct);

            return new RestauranteMetricasDashboardResponse
            {
                RestauranteId = restauranteId,
                TotalTop3Individual = estadisticas?.TotalTop3Individual ?? 0,
                TotalTop3Grupo = estadisticas?.TotalTop3Grupo ?? 0,
                TotalVisitasPerfil = estadisticas?.TotalVisitasPerfil ?? 0,
                TotalFavoritosHistorico = totalFavoritos,
                TotalFavoritosActual = totalFavoritos
            };
        }
    }
}
