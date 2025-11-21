using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Interfaces
{
    public interface IUsuarioRestauranteFavoritoRepository
    {
        Task<int> CountByUsuarioAsync(Guid usuarioId, CancellationToken ct);
        Task CrearAsync(UsuarioRestauranteFavorito usuarioRestauranteFavorito, CancellationToken ct);
        Task<bool> ExistsAsync(Guid usuarioId, Guid restauranteId, CancellationToken ct);
        Task<int> CountByRestauranteAsync(Guid restauranteId, CancellationToken ct);
    }
}
