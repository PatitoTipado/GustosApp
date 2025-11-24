using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.UsuarioUseCases
{
    public class ObtenerRestaurantesFavoritosUseCase
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly IUsuarioRestauranteFavoritoRepository _favoritos;

        public ObtenerRestaurantesFavoritosUseCase(IUsuarioRepository usuarios,
            IUsuarioRestauranteFavoritoRepository favoritos)
        {
            _usuarios = usuarios;
            _favoritos = favoritos;
        }

        public async Task<List<Restaurante>> HandleAsync(string firebaseUid, CancellationToken ct = default)
        {
            var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid, ct);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            return await _favoritos.GetFavoritosByUsuarioAsync(usuario.Id, ct);
        }
    }

}
