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
                .Include(s => s.Imagenes)
                .Include(g => g.Gustos)
                .Include(r => r.Restricciones)
                .FirstOrDefaultAsync(s => s.Id == id, ct);
        }

        public Task<List<SolicitudRestaurante>> GetPendientesAsync(CancellationToken ct)
        {
            return _db.SolicitudesRestaurantes
                .Include(s => s.Usuario)
                .Include(s => s.Imagenes)
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
            _db.SolicitudesRestaurantes.Update(solicitud);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<SolicitudRestaurante>> GetAllAsync(CancellationToken ct)
        {
            return await _db.SolicitudesRestaurantes
                .Include(s => s.Usuario)
                .Include(s => s.Gustos)
                .Include(s => s.Restricciones)
                .Include(s => s.Imagenes)
                .OrderByDescending(s => s.FechaCreacion)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<SolicitudRestaurante>> GetByEstadoAsync(
            EstadoSolicitudRestaurante estado,
            CancellationToken ct)
        {
            return await _db.SolicitudesRestaurantes
                .Where(s => s.Estado == estado)
                .Include(s => s.Usuario)
                .Include(s => s.Gustos)
                .Include(s => s.Restricciones)
                .Include(s => s.Imagenes)
                .OrderByDescending(s => s.FechaCreacion)
                .ToListAsync(ct);
        }

    }

}
