using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using GustosApp.Application.Services;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Domain.Model;
using GustosApp.Infraestructure;

namespace GustosApp.Infraestructure.Services
{
    public class ServicioRestaurantes : IServicioRestaurantes
    {
        private readonly GustosDbContext _db;

        public ServicioRestaurantes(GustosDbContext db)
        {
            _db = db;
        }

        private static string NormalizarNombre(string nombre)
            => (nombre ?? string.Empty).Trim().ToLowerInvariant();

        // Crear restaurante
        public async Task<Restaurante> CrearAsync(string propietarioUid, CrearRestauranteDto dto)
        {
            var nombreNorm = NormalizarNombre(dto.Nombre);
            var nombreEnUso = await _db.Restaurantes
                .AsNoTracking()
                .AnyAsync(r => r.NombreNormalizado == nombreNorm);

            if (nombreEnUso)
                throw new ArgumentException("El nombre ya está en uso.");

            if (!Enum.TryParse<TipoRestaurante>(dto.Tipo, true, out var tipoParsed))
                throw new ArgumentException("Tipo inválido.");

            var platosParsed = (dto.Platos ?? new())
                .Select(s =>
                {
                    if (!Enum.TryParse<PlatoComida>(s, true, out var p))
                        throw new ArgumentException($"Plato inválido: {s}");
                    return p;
                })
                .Distinct()
                .ToList();

            var ahora = DateTime.UtcNow;
            var entidad = new Restaurante
            {
                Id = Guid.NewGuid(),
                PropietarioUid = propietarioUid,
                Nombre = dto.Nombre.Trim(),
                NombreNormalizado = nombreNorm,
                Direccion = dto.Direccion.Trim(),
                Latitud = dto.Latitud,
                Longitud = dto.Longitud,
                HorariosJson = dto.Horarios is null ? "{}" : JsonSerializer.Serialize(dto.Horarios),
                CreadoUtc = ahora,
                ActualizadoUtc = ahora,
                Tipo = tipoParsed,
                ImagenUrl = string.IsNullOrWhiteSpace(dto.ImagenUrl) ? null : dto.ImagenUrl!.Trim(),
                Valoracion = dto.Valoracion
            };

            foreach (var p in platosParsed)
                entidad.Platos.Add(new RestaurantePlato { RestauranteId = entidad.Id, Plato = p });

            _db.Restaurantes.Add(entidad);
            await _db.SaveChangesAsync();

            return entidad;
        }

        // Obtener restaurante por ID
        public async Task<Restaurante?> ObtenerAsync(Guid id)
        {
            return await _db.Restaurantes
                .Include(x => x.Platos)
                .Include(x => x.GustosQueSirve)
                .Include(x => x.RestriccionesQueRespeta)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // Obtener por propietario UID
        public async Task<Restaurante?> ObtenerPorPropietarioAsync(string propietarioUid)
        {
            return await _db.Restaurantes
                .AsNoTracking()
                .Include(x => x.Platos)
                .Include(x => x.GustosQueSirve)
                .Include(x => x.RestriccionesQueRespeta)
                .FirstOrDefaultAsync(x => x.PropietarioUid == propietarioUid);
        }

        // Actualizar restaurante
        public async Task<Restaurante?> ActualizarAsync(Guid id, string solicitanteUid, bool esAdmin, ActualizarRestauranteDto dto)
        {
            var r = await _db.Restaurantes
                .Include(x => x.Platos)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (r is null) return null;

            if (!esAdmin && r.PropietarioUid != solicitanteUid)
                throw new UnauthorizedAccessException("No tenés permisos para modificar este restaurante.");

            var nombreNorm = NormalizarNombre(dto.Nombre);
            var conflictoNombre = await _db.Restaurantes
                .AnyAsync(x => x.Id != id && x.NombreNormalizado == nombreNorm);

            if (conflictoNombre)
                throw new ArgumentException("El nombre ya está en uso.");

            if (!Enum.TryParse<TipoRestaurante>(dto.Tipo, true, out var tipoParsed))
                throw new ArgumentException("Tipo inválido.");

            var platosParsed = (dto.Platos ?? new())
                .Select(s =>
                {
                    if (!Enum.TryParse<PlatoComida>(s, true, out var p))
                        throw new ArgumentException($"Plato inválido: {s}");
                    return p;
                })
                .Distinct()
                .ToList();

            r.Nombre = dto.Nombre.Trim();
            r.NombreNormalizado = nombreNorm;
            r.Direccion = dto.Direccion.Trim();
            r.Latitud = dto.Latitud;
            r.Longitud = dto.Longitud;
            r.HorariosJson = dto.Horarios is null ? "{}" : JsonSerializer.Serialize(dto.Horarios);
            r.ActualizadoUtc = DateTime.UtcNow;
            r.Tipo = tipoParsed;
            r.ImagenUrl = string.IsNullOrWhiteSpace(dto.ImagenUrl) ? null : dto.ImagenUrl!.Trim();
            r.Valoracion = dto.Valoracion;

            var actuales = r.Platos.Select(x => x.Plato).ToHashSet();
            var nuevos = platosParsed.ToHashSet();

            var aAgregar = nuevos.Except(actuales).ToList();
            var aQuitar = actuales.Except(nuevos).ToList();

            foreach (var p in aQuitar)
            {
                var row = r.Platos.First(x => x.Plato == p);
                _db.RestaurantePlatos.Remove(row);
            }

            foreach (var p in aAgregar)
            {
                r.Platos.Add(new RestaurantePlato { RestauranteId = r.Id, Plato = p });
            }

            await _db.SaveChangesAsync();
            return r;
        }

        // Eliminar restaurante
        public async Task<bool> EliminarAsync(Guid id, string solicitanteUid, bool esAdmin)
        {
            var r = await _db.Restaurantes.FindAsync(id);
            if (r is null) return false;

            if (!esAdmin && r.PropietarioUid != solicitanteUid)
                throw new UnauthorizedAccessException("No tenés permisos para eliminar este restaurante.");

            _db.Restaurantes.Remove(r);
            await _db.SaveChangesAsync();
            return true;
        }

        // Buscar con filtros
        public async Task<List<Restaurante>> BuscarAsync(string? tipo, string? plato, double? lat = null, double? lng = null, int? radioMetros = null)
        {
            var q = _db.Restaurantes
                .Include(r => r.Platos)
                .Include(r => r.GustosQueSirve)
                .Include(r => r.RestriccionesQueRespeta)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tipo))
            {
                if (Enum.TryParse<TipoRestaurante>(tipo, true, out var tipoEnum))
                    q = q.Where(r => r.Tipo == tipoEnum);
            }

            if (!string.IsNullOrWhiteSpace(plato))
            {
                q = q.Where(r => r.Platos.Any(p => p.Plato.ToString().Contains(plato)));
            }

            return await q.Take(500).ToListAsync();
        }

        // Cercanos
        public async Task<List<Restaurante>> ListarCercanosAsync(double lat, double lng, int radioMetros, string? tipo = null, IEnumerable<string>? platos = null)
        {
            double degLat = radioMetros / 111_000.0;
            double degLng = radioMetros / (111_000.0 * Math.Cos(lat * Math.PI / 180.0));

            var minLat = lat - degLat;
            var maxLat = lat + degLat;
            var minLng = lng - degLng;
            var maxLng = lng + degLng;

            var q = _db.Restaurantes
                .Include(r => r.Platos)
                .Include(r => r.GustosQueSirve)
                .Include(r => r.RestriccionesQueRespeta)
                .Where(r => r.Latitud >= minLat && r.Latitud <= maxLat
                         && r.Longitud >= minLng && r.Longitud <= maxLng);

            if (!string.IsNullOrWhiteSpace(tipo) && Enum.TryParse<TipoRestaurante>(tipo, true, out var tipoParsed))
            {
                q = q.Where(r => r.Tipo == tipoParsed);
            }

            if (platos is not null && platos.Any())
            {
                var set = new HashSet<PlatoComida>(
                    platos.Select(p => Enum.TryParse<PlatoComida>(p, true, out var v) ? v : (PlatoComida?)null)
                          .Where(v => v.HasValue)
                          .Select(v => v!.Value));

                q = q.Where(r => r.Platos.Any(p => set.Contains(p.Plato)));
            }

            return await q
                .OrderBy(r => Math.Abs(r.Latitud - lat) + Math.Abs(r.Longitud - lng))
                .Take(200)
                .ToListAsync();
        }



    }
}

