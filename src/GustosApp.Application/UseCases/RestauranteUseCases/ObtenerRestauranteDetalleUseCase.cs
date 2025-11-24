using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Validators;
using GustosApp.Application.Interfaces;
using GustosApp.Application.Model;
using GustosApp.Domain.Interfaces;

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

            // Reviews importadas de Google
            if (!string.IsNullOrEmpty(restaurante.PlaceId) &&
                (restaurante.Reviews == null || !restaurante.Reviews.Any()))
            {
                var actualizado = await _servicioRestaurantes.ObtenerResenasDesdeGooglePlaces(restaurante.PlaceId, ct);
                if (actualizado != null)
                    restaurante = actualizado;
            }
            restaurante.Reviews = restaurante.Reviews
                .OrderBy(r => r.EsImportada)              
                .ThenByDescending(r => r.FechaCreacion)  
                .ToList();

      


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
