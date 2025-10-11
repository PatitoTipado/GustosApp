using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface IRestriccionRepository
    {
        Task<List<Restriccion>> GetAllAsync(CancellationToken ct);
        Task<List<Restriccion>> GetRestriccionesByIdsAsync(List<Guid> ids, CancellationToken ct);

    }
}
