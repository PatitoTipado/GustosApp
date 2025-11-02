using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface IReseñaRepository
    {
        Task RemoveByRestauranteIdAsync(Guid restauranteId, CancellationToken ct);
        Task AddAsync(ReseñaRestaurante reseña, CancellationToken ct);
    }

}
