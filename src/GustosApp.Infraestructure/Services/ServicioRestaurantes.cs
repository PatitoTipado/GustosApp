using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
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
                Lat = r.Latitud,
                Lng = r.Longitud,
                Horarios = horarios,
                CreadoUtc = r.CreadoUtc,
                ActualizadoUtc = r.ActualizadoUtc,
                PrimaryType = r.PrimaryType,
                Types = SafeDeserializeTypes(r.TypesJson),
                ImagenUrl = r.ImagenUrl,
                Valoracion = r.Valoracion,
                Platos = r.Platos.Select(p => p.Plato.ToString()).ToList(),

                //
                GustosQueSirve = r.GustosQueSirve
                    .Select(g => new GustoDto
                    {
                        Id = g.Id,
                        Nombre = g.Nombre
                    })
                    .ToList(),

                //
                RestriccionesQueRespeta = r.RestriccionesQueRespeta
                    .Select(g => new RestriccionResponse(g.Id, g.Nombre))
                    .ToList()
            };

            static List<string> SafeDeserializeTypes(string json)
            {
                try
                {
                    return string.IsNullOrWhiteSpace(json)
                        ? new List<string>()
                        : JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                }
                catch
                {
                    return new List<string>();
                }
            }
        }



        public async Task<IReadOnlyList<RestauranteDto>> BuscarAsync(
    string? tipo,
    string? plato,
    double? lat = null,
    double? lng = null,
    int? radioMetros = null)
        {
            // 1. CARGA INICIAL y DEFINICIÓN DE INCLUDES (Ejecución en la BD)

            var q = _db.Restaurantes.AsNoTracking()

                .Include(r => r.Platos)

                .Include(r => r.GustosQueSirve)

                .Include(r => r.RestriccionesQueRespeta)

                .AsSplitQuery();

            foreach (var r in q)
            {
                Console.WriteLine(r.Id);
                Console.WriteLine(r.Nombre);

                Console.WriteLine(r.GustosQueSirve.Count);
                Console.WriteLine(r.GustosQueSirve.Count);
                Console.WriteLine(r.GustosQueSirve.Count);

                foreach (var rr in r.GustosQueSirve)
                {
                    Console.WriteLine($"- {rr.Nombre}");
                }
            }

            if (!string.IsNullOrWhiteSpace(tipo))
            {
                var t = tipo.Trim();
                q = q.Where(r => r.PrimaryType == t || r.TypesJson.Contains($"\"{t}\""));
            }

            if (!string.IsNullOrWhiteSpace(plato))
            {
                if (!Enum.TryParse<PlatoComida>(plato, ignoreCase: true, out var platoEnum))
                    return Array.Empty<RestauranteDto>();
                q = q.Where(r => r.Platos.Any(p => p.Plato == platoEnum));
            }

            if (!lat.HasValue || !lng.HasValue || !radioMetros.HasValue || radioMetros.Value <= 0)
            {
                q = q.OrderBy(r => r.NombreNormalizado);
            }


            var lista = await q.Take(1000).ToListAsync();


            if (lat.HasValue && lng.HasValue && radioMetros.HasValue && radioMetros.Value > 0)
            {
                var latVal = lat.Value;
                var lngVal = lng.Value;
                var radioMetrosVal = radioMetros.Value;

                var degLat = radioMetrosVal / 111_000.0;
                var degLng = radioMetrosVal / (111_000.0 * Math.Cos(latVal * Math.PI / 180.0));

                var minLat = latVal - degLat;
                var maxLat = latVal + degLat;
                var minLng = lngVal - degLng;
                var maxLng = lngVal + degLng;

                // Latitud/Longitud son double (no-null), filtramos directo
                lista = lista
                    .Where(r =>
                        r.Latitud >= minLat && r.Latitud <= maxLat &&
                        r.Longitud >= minLng && r.Longitud <= maxLng)
                    .ToList();

                lista = lista
                    .OrderBy(r => Math.Abs(r.Latitud - latVal) + Math.Abs(r.Longitud - lngVal))
                    .Take(200)
                    .ToList();
            }

            Console.WriteLine("Lista");
            Console.WriteLine("Lista");
            Console.WriteLine("Lista");


            foreach (var r in lista)
            {
                Console.WriteLine(r.Id);
                Console.WriteLine(r.Nombre);

                Console.WriteLine(r.GustosQueSirve.Count);
                Console.WriteLine(r.GustosQueSirve.Count);
                Console.WriteLine(r.GustosQueSirve.Count);

                foreach (var rr in r.GustosQueSirve)
                {
                    Console.WriteLine($"- {rr.Nombre}");
                }
            }


            return lista.Select(Map).ToList();
        }


        public async Task<RestauranteDto> CrearAsync(string propietarioUid, CrearRestauranteDto dto)
        {
            var nombreNorm = NormalizarNombre(dto.Nombre);
            var nombreEnUso = await _db.Restaurantes.AsNoTracking()
                .AnyAsync(r => r.NombreNormalizado == nombreNorm);
            if (nombreEnUso)
                throw new ArgumentException("El nombre ya está en uso.");

            var primaryType = string.IsNullOrWhiteSpace(dto.PrimaryType)
                ? "restaurant"
                : dto.PrimaryType!.Trim();

            var typesList = (dto.Types ?? new())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .Distinct()
                .ToList();

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

            var (latOpt, lngOpt) = dto.Coordenadas;
            if (latOpt is null || lngOpt is null)
                throw new ArgumentException("Lat/Lng requeridos en el cuerpo (usa 'lat' y 'lng' o 'latitud' y 'longitud')."); var entidad = new Restaurante
                {
                    Id = Guid.NewGuid(),
                    PropietarioUid = propietarioUid,
                    Nombre = dto.Nombre.Trim(),
                    NombreNormalizado = nombreNorm,
                    Direccion = dto.Direccion.Trim(),
                    Latitud = latOpt.Value,
                    Longitud = lngOpt.Value,
                    HorariosJson = dto.HorariosComoJson ?? "{}",
                    CreadoUtc = ahora,
                    ActualizadoUtc = ahora,
                    PrimaryType = primaryType,
                    TypesJson = JsonSerializer.Serialize(typesList),
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

            if (!string.IsNullOrWhiteSpace(tipo))
            {
                var t = tipo.Trim();
                q = q.Where(r => r.PrimaryType == t || r.TypesJson.Contains($"\"{t}\""));
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

            var primaryType = string.IsNullOrWhiteSpace(dto.PrimaryType)
                ? r.PrimaryType
                : dto.PrimaryType!.Trim();

            var typesList = (dto.Types ?? new())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .Distinct()
                .ToList();

            var platosParsed = (dto.Platos ?? new())
                .Select(s =>
                {
                    if (!Enum.TryParse<PlatoComida>(s, true, out var p))
                        throw new ArgumentException($"Plato inválido: {s}");
                    return p;
                })
                .Distinct()
                .ToList();
            var coords = dto.Coordenadas;
            var latOpt = coords.lat;   // double?
            var lngOpt = coords.lng;   // double?

            if (latOpt is null || lngOpt is null)
                throw new ArgumentException("Lat/Lng requeridos para actualizar ubicación.");

            r.Nombre = dto.Nombre.Trim();
            r.NombreNormalizado = nombreNorm;
            r.Direccion = dto.Direccion.Trim();
            r.Latitud = latOpt.Value;
            r.Longitud = lngOpt.Value;
            r.HorariosJson = dto.HorariosComoJson ?? "{}";
            r.ActualizadoUtc = DateTime.UtcNow;

            r.PrimaryType = primaryType;
            r.TypesJson = JsonSerializer.Serialize(typesList);
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
            await _db.Entry(r).Collection(x => x.Platos).LoadAsync();
            await _db.Entry(r).Collection(x => x.GustosQueSirve).LoadAsync();
            await _db.Entry(r).Collection(x => x.RestriccionesQueRespeta).LoadAsync();
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

