using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model.@enum;
using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.Infraestructure.Repositories
{

    public class SolicitudRestauranteRepositoryEF : ISolicitudRestauranteRepository
    {
        private readonly GustosDbContext _db;

        public SolicitudRestauranteRepositoryEF(GustosDbContext db) => _db = db;

        public Task<SolicitudRestaurante?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return _db.SolicitudesRestaurantes
                .Include(s => s.Usuario)
                .FirstOrDefaultAsync(s => s.Id == id, ct);
        }

        public Task<List<SolicitudRestaurante>> GetPendientesAsync(CancellationToken ct)
        {
            return _db.SolicitudesRestaurantes
                .Include(s => s.Usuario)
                .Where(s => s.Estado == EstadoSolicitudRestaurante.Pendiente)
                .ToListAsync(ct);
        }

        public async Task AddAsync(SolicitudRestaurante solicitud, CancellationToken ct)
        {
            _db.SolicitudesRestaurantes.Add(solicitud);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(SolicitudRestaurante solicitud, CancellationToken ct)
        {
            await _db.SaveChangesAsync(ct);
        }
    }

}
