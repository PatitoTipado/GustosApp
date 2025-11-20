using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface ISolicitudRestauranteRepository
    {
        Task<SolicitudRestaurante?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<List<SolicitudRestaurante>> GetPendientesAsync(CancellationToken ct);
        Task AddAsync(SolicitudRestaurante solicitud, CancellationToken ct);
        Task UpdateAsync(SolicitudRestaurante solicitud, CancellationToken ct);
    }

}
