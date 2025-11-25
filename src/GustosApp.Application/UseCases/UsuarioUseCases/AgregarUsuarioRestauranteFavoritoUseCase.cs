using GustosApp.Application.Common.Exceptions;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases.UsuarioUseCases
{
    public class AgregarUsuarioRestauranteFavoritoUseCase
    {
        private readonly IUsuarioRepository _repositorioUsuario;
        private readonly IUsuarioRestauranteFavoritoRepository _repositorioFavorito;

        public AgregarUsuarioRestauranteFavoritoUseCase(IUsuarioRestauranteFavoritoRepository repositorioFavorito, IUsuarioRepository repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario;
            _repositorioFavorito = repositorioFavorito;
        }

        public async Task HandleAsync(string firebaseUid, Guid restauranteId, CancellationToken ct = default)
        {
            var usuario = await _repositorioUsuario.GetByFirebaseUidAsync(firebaseUid, ct);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            var cantidadRestauranteFavorito = await _repositorioFavorito.CountByUsuarioAsync(usuario.Id, ct);

            if (usuario.Plan == PlanUsuario.Free && cantidadRestauranteFavorito >= 3)
            {
                throw new LimiteFavoritosAlcanzadoException(
                      tipoPlan: "Free",
                     limiteActual: 3,
                    favoritosActuales: cantidadRestauranteFavorito
                  );
            }

            var existe = await _repositorioFavorito.ExistsAsync(usuario.Id, restauranteId, ct);
            if (existe)
            {
                throw new Exception("Este restaurante ya está en tus favoritos.");
            }

            var favorito = new UsuarioRestauranteFavorito
            {
                UsuarioId = usuario.Id,
                RestauranteId = restauranteId,
                FechaAgregado = DateTime.UtcNow
            };
            await _repositorioFavorito.CrearAsync(favorito, ct);
        }

        public async Task HandleAsyncDelete(string firebaseUid, Guid restauranteId, CancellationToken ct = default)
        {
            var usuario = await _repositorioUsuario.GetByFirebaseUidAsync(firebaseUid, ct);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            var existe = await _repositorioFavorito.ExistsAsync(usuario.Id, restauranteId, ct);

            // Si no existe, simplemente retornar sin hacer nada (idempotente)
            if (!existe)
            {
                return; // Ya no está en favoritos, operación exitosa
            }

            await _repositorioFavorito.EliminarAsync(usuario.Id, restauranteId, ct);
        }
    }
}