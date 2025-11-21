using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases.RestauranteUseCases
{
    public class RegistrarTop3GrupoRestaurantesUseCase
    {
        private readonly IRestauranteEstadisticasRepository _estadisticasRepository;

        public RegistrarTop3GrupoRestaurantesUseCase(
            IRestauranteEstadisticasRepository estadisticasRepository)
        {
            _estadisticasRepository = estadisticasRepository;
        }

        public Task HandleAsync(IEnumerable<Guid> restauranteIds, CancellationToken ct = default)
        {
            if (restauranteIds == null)
                throw new ArgumentNullException(nameof(restauranteIds));

            return _estadisticasRepository.IncrementarTop3GrupoAsync(restauranteIds, ct);
        }
    }
}
