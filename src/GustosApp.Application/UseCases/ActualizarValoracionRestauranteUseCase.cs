using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases
{
    public class ActualizarValoracionRestauranteUseCase
    {
        private readonly IRestauranteRepository _restauranteRepository;
        private readonly IValoracionUsuarioRepository _valoracionUsuarioRepository;

        public ActualizarValoracionRestauranteUseCase(IRestauranteRepository restauranteRepository, IValoracionUsuarioRepository valoracionUsuarioRepository)
        {
            _restauranteRepository = restauranteRepository;
            _valoracionUsuarioRepository = valoracionUsuarioRepository;
        }

        public async Task HandleAsync(Guid restauranteId, CancellationToken cancellationToken)
        {
            var valoraciones = await _valoracionUsuarioRepository.ObtenerPorRestauranteAsync(restauranteId, cancellationToken);

            if (!valoraciones.Any())
            {
                await _restauranteRepository.ActualizarValoracionAsync(restauranteId, 0, cancellationToken);
                return;
            }

            double promedio = valoraciones.Average(v => (double)v.ValoracionUsuario);
            await _restauranteRepository.ActualizarValoracionAsync(restauranteId, promedio, cancellationToken);

        }
    }
    }
