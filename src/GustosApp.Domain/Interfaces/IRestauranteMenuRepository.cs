using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface IRestauranteMenuRepository
    {
        Task AddAsync(RestauranteMenu menu, CancellationToken ct);
        Task UpdateAsync(RestauranteMenu menu, CancellationToken ct);
        Task<RestauranteMenu?> GetByRestauranteIdAsync(Guid restauranteId, CancellationToken ct);
    }

}
