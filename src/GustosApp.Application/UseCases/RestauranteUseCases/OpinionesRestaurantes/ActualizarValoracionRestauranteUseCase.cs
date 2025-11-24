using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases.RestauranteUseCases.OpinionesRestaurantes
{
    public class ActualizarValoracionRestauranteUseCase
    {
        private readonly IRestauranteRepository _restauranteRepository;
        private readonly IOpinionRestauranteRepository _opinionRestauranteRepository;

        public ActualizarValoracionRestauranteUseCase(IRestauranteRepository restauranteRepository, IOpinionRestauranteRepository opinionRestauranteRepository)
        {
            _restauranteRepository = restauranteRepository;
            _opinionRestauranteRepository = opinionRestauranteRepository;
        }

        public async Task HandleAsync(Guid restauranteId, CancellationToken cancellationToken)
        {
            var valoraciones = await _opinionRestauranteRepository.ObtenerPorRestauranteAsync(restauranteId, cancellationToken);

            if (!valoraciones.Any())
            {
                await _restauranteRepository.ActualizarValoracionAsync(restauranteId, 0, cancellationToken);
                return;
            }

            double promedio = valoraciones.Average(v => (double)v.Valoracion);
            await _restauranteRepository.ActualizarValoracionAsync(restauranteId, promedio, cancellationToken);

        }
    }
}
