using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Services;
using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;

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

        private static RestauranteDto Map(Restaurante r)
        {
            object? horarios = null;
            try
            {
                horarios = string.IsNullOrWhiteSpace(r.HorariosJson)
                    ? null
                    : JsonSerializer.Deserialize<object>(r.HorariosJson);
            }
            catch { horarios = null; }

            return new RestauranteDto
            {
                Id = r.Id,
                PropietarioUid = r.PropietarioUid,
                Nombre = r.Nombre,
                Direccion = r.Direccion,
                Latitud = r.Latitud,
                Longitud = r.Longitud,
                Horarios = horarios,
                CreadoUtc = r.CreadoUtc,
                ActualizadoUtc = r.ActualizadoUtc,
                Tipo = r.Tipo.ToString(),
                ImagenUrl = r.ImagenUrl,
                Valoracion = r.Valoracion,
                Platos = r.Platos.Select(p => p.Plato.ToString()).ToList()
            };
        }

        public async Task<IReadOnlyList<RestauranteDto>> BuscarAsync(
    string? tipo,
    string? plato,
    double? lat = null,
    double? lng = null,
    int? radioMetros = null)
{
    var q = _db.Restaurantes.AsNoTracking()
        .Include(r => r.Platos)
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(tipo))
    {
        if (!Enum.TryParse<TipoRestaurante>(tipo, ignoreCase: true, out var tipoEnum))
            return Array.Empty<RestauranteDto>(); 
        q = q.Where(r => r.Tipo == tipoEnum);
    }

    if (!string.IsNullOrWhiteSpace(plato))
    {
        if (!Enum.TryParse<PlatoComida>(plato, ignoreCase: true, out var platoEnum))
            return Array.Empty<RestauranteDto>();
        q = q.Where(r => r.Platos.Any(p => p.Plato == platoEnum));
    }

    
    if (lat.HasValue && lng.HasValue && radioMetros.HasValue && radioMetros.Value > 0)
    {
        var degLat = radioMetros.Value / 111_000.0;
        var degLng = radioMetros.Value / (111_000.0 * Math.Cos(lat.Value * Math.PI / 180.0));

        var minLat = lat.Value - degLat;
        var maxLat = lat.Value + degLat;
        var minLng = lng.Value - degLng;
        var maxLng = lng.Value + degLng;

        q = q.Where(r => r.Latitud >= minLat && r.Latitud <= maxLat
                      && r.Longitud >= minLng && r.Longitud <= maxLng)
             .OrderBy(r => Math.Abs(r.Latitud - lat.Value) + Math.Abs(r.Longitud - lng.Value));
    }
    else
    {
        q = q.OrderBy(r => r.NombreNormalizado);
    }

    var lista = await q.Take(200).ToListAsync();
    return lista.Select(Map).ToList();
}


        public async Task<RestauranteDto> CrearAsync(string propietarioUid, CrearRestauranteDto dto)
        {
            var nombreNorm = NormalizarNombre(dto.Nombre);
            var nombreEnUso = await _db.Restaurantes.AsNoTracking()
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
            await _db.Entry(entidad).Collection(x => x.Platos).LoadAsync();

            return Map(entidad);
        }

        public async Task<RestauranteDto?> ObtenerAsync(Guid id)
        {
            var r = await _db.Restaurantes
                .Include(x => x.Platos)
                .FirstOrDefaultAsync(x => x.Id == id);
            return r is null ? null : Map(r);
        }

        public async Task<RestauranteDto?> ObtenerPorPropietarioAsync(string propietarioUid)
        {
            var r = await _db.Restaurantes.AsNoTracking()
                .Include(x => x.Platos)
                .FirstOrDefaultAsync(x => x.PropietarioUid == propietarioUid);
            return r is null ? null : Map(r);
        }

        public async Task<IReadOnlyList<RestauranteDto>> ListarCercanosAsync(
            double lat, double lng, int radioMetros,
            string? tipo = null,
            IEnumerable<string>? platos = null)
        {
            double degLat = radioMetros / 111_000.0;
            double degLng = radioMetros / (111_000.0 * Math.Cos(lat * Math.PI / 180.0));

            var minLat = lat - degLat;
            var maxLat = lat + degLat;
            var minLng = lng - degLng;
            var maxLng = lng + degLng;

            var q = _db.Restaurantes.AsNoTracking()
                .Include(r => r.Platos)
                .Where(r => r.Latitud >= minLat && r.Latitud <= maxLat
                         && r.Longitud >= minLng && r.Longitud <= maxLng);

            if (!string.IsNullOrWhiteSpace(tipo) &&
                Enum.TryParse<TipoRestaurante>(tipo, true, out var tipoParsed))
            {
                q = q.Where(r => r.Tipo == tipoParsed);
            }

            if (platos is not null)
            {
                var set = new HashSet<PlatoComida>(
                    platos.Select(p => Enum.TryParse<PlatoComida>(p, true, out var v) ? v : (PlatoComida?)null)
                          .Where(v => v.HasValue)
                          .Select(v => v!.Value));
                if (set.Count > 0)
                {
                    q = q.Where(r => r.Platos.Any(p => set.Contains(p.Plato)));
                }
            }

            var lista = await q
                .OrderBy(r => Math.Abs(r.Latitud - lat) + Math.Abs(r.Longitud - lng))
                .Take(200)
                .ToListAsync();

            return lista.Select(Map).ToList();
        }

        public async Task<RestauranteDto?> ActualizarAsync(Guid id, string solicitanteUid, bool esAdmin, ActualizarRestauranteDto dto)
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
            return Map(r);
        }

        public async Task<bool> EliminarAsync(Guid id, string solicitanteUid, bool esAdmin)
        {
            var r = await _db.Restaurantes.FirstOrDefaultAsync(x => x.Id == id);
            if (r is null) return false;

            if (!esAdmin && r.PropietarioUid != solicitanteUid)
                throw new UnauthorizedAccessException("No tenés permisos para eliminar este restaurante.");

            _db.Restaurantes.Remove(r);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

