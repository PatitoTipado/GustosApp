using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.RestauranteUseCases
{
    public class ActualizarRestauranteDashboardUseCase
    {
        private readonly IRestauranteRepository _restauranteRepository;
        private readonly IGustoRepository _gustoRepository;
        private readonly IRestriccionRepository _restriccionRepository;

        public ActualizarRestauranteDashboardUseCase(
            IRestauranteRepository restauranteRepository,
            IGustoRepository gustoRepository,
            IRestriccionRepository restriccionRepository)
        {
            _restauranteRepository = restauranteRepository;
            _gustoRepository = gustoRepository;
            _restriccionRepository = restriccionRepository;
        }

        public async Task<Restaurante> HandleAsync(
            Guid restauranteId,
            string? direccion,
            double? latitud,
            double? longitud,
            string? horariosJson,
            string? webUrl,
            List<Guid>? gustosQueSirveIds,
            List<Guid>? restriccionesQueRespetaIds,
            CancellationToken ct = default)
        {
            var restaurante = await _restauranteRepository.GetByIdAsync(restauranteId, ct);
            if (restaurante == null)
                throw new InvalidOperationException("El restaurante no existe.");

            // Dirección
            if (!string.IsNullOrWhiteSpace(direccion))
                restaurante.Direccion = direccion;

            // Coordenadas
            if (latitud.HasValue)
                restaurante.Latitud = latitud.Value;

            if (longitud.HasValue)
                restaurante.Longitud = longitud.Value;

            // Horarios (si viene null, no tocamos; si viene string, lo pisamos tal cual)
            if (horariosJson != null)
                restaurante.HorariosJson = horariosJson;

            // WebUrl: si viene null no tocamos; si viene string vacío, lo seteamos en null
            if (webUrl != null)
                restaurante.WebUrl = string.IsNullOrWhiteSpace(webUrl) ? null : webUrl;

            // Gustos
            if (gustosQueSirveIds != null)
            {
                var gustos = gustosQueSirveIds.Count > 0
                    ? await _gustoRepository.GetByIdsAsync(gustosQueSirveIds, ct)
                    : new List<Gusto>();

                restaurante.SetGustos(gustos);
            }

            // Restricciones
            if (restriccionesQueRespetaIds != null)
            {
                var restricciones = restriccionesQueRespetaIds.Count > 0
                    ? await _restriccionRepository.GetRestriccionesByIdsAsync(restriccionesQueRespetaIds, ct)
                    : new List<Restriccion>();

                restaurante.SetRestricciones(restricciones);
            }

            restaurante.ActualizadoUtc = DateTime.UtcNow;
            restaurante.UltimaActualizacion = DateTime.UtcNow;

            await _restauranteRepository.UpdateAsync(restaurante, ct);
            await _restauranteRepository.SaveChangesAsync(ct);

            return restaurante;
        }
    }
}
