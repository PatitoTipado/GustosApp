using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface ICondicionMedicaRepository
    {
        Task<List<CondicionMedica>> GetAllAsync(CancellationToken ct);
        Task <List<CondicionMedica>>GetByIdsAsync(List<Guid> ids, CancellationToken ct);
    }
}
