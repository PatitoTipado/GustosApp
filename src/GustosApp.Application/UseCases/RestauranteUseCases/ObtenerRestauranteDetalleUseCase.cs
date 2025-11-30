using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Validators;
using GustosApp.Application.Interfaces;
using GustosApp.Application.Model;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.RestauranteUseCases
{
    public class ObtenerRestauranteDetalleUseCase
    {
        private readonly IServicioRestaurantes _servicioRestaurantes;
        private readonly IRestauranteEstadisticasRepository _visitas;
        private readonly IUsuarioRestauranteFavoritoRepository _favoritos;
        private readonly IUsuarioRepository _usuarios;

        public ObtenerRestauranteDetalleUseCase(
            IServicioRestaurantes servicioRestaurantes,
            IRestauranteEstadisticasRepository visitas,
            IUsuarioRestauranteFavoritoRepository favoritos,
            IUsuarioRepository usuarios)
        {
            _servicioRestaurantes = servicioRestaurantes;
            _visitas = visitas;
            _favoritos = favoritos;
            _usuarios = usuarios;
        }

        public async Task<RestauranteDetalleResult> HandleAsync(
        Guid restauranteId,
        string? firebaseUid,
        CancellationToken ct)
        {
            var restaurante = await _servicioRestaurantes.ObtenerAsync(restauranteId);
            if (restaurante == null)
                throw new KeyNotFoundException("Restaurante no encontrado");

            // Si no hay reviews, importar las de Google
            if (!string.IsNullOrEmpty(restaurante.PlaceId) &&
                (restaurante.Reviews == null || !restaurante.Reviews.Any()))
            {
                await _servicioRestaurantes.ActualizarReviewsDesdeGoogleLegacyAsync(
                    restaurante.Id, restaurante.PlaceId, ct);

                restaurante = await _servicioRestaurantes.ObtenerAsync(restauranteId)
                              ?? restaurante;
            }

            Guid? usuarioId = null;
            bool esFavorito = false;

            if (!string.IsNullOrWhiteSpace(firebaseUid))
            {
                var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid, ct);
                if (usuario != null)
                {
                    usuarioId = usuario.Id;
                    await _visitas.IncrementarVisitaPerfilAsync(restaurante.Id, ct);
                    esFavorito = await _favoritos.ExistsAsync(usuario.Id, restaurante.Id, ct);
                }
            }

            return new RestauranteDetalleResult(restaurante, esFavorito);
        }


    }
}