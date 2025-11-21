using System;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases.RestauranteUseCases
{
    public class RegistrarVisitaPerfilRestauranteUseCase
    {
        private readonly IRestauranteEstadisticasRepository _estadisticasRepository;

        public RegistrarVisitaPerfilRestauranteUseCase(
            IRestauranteEstadisticasRepository estadisticasRepository)
        {
            _estadisticasRepository = estadisticasRepository;
        }

        public Task HandleAsync(Guid restauranteId, CancellationToken ct = default)
        {
            if (restauranteId == Guid.Empty)
                throw new ArgumentException("El restauranteId no puede ser vacío.", nameof(restauranteId));

            return _estadisticasRepository.IncrementarVisitaPerfilAsync(restauranteId, ct);
        }
    }
}
