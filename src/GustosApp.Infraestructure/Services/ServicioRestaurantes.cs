
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Services;  
using GustosApp.Domain.Model;
using GustosApp.Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.Infraestructure.Services
{
    public class ServicioRestaurantes : IServicioRestaurantes
    {
        private readonly GustosDbContext _db;

        public ServicioRestaurantes(GustosDbContext db) => _db = db;

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
                Lng= r.Longitud,
                Horarios = horarios,
                CreadoUtc = r.CreadoUtc,
                ActualizadoUtc = r.ActualizadoUtc
            };
        }

        public async Task<RestauranteDto> CrearAsync(string propietarioUid, CrearRestauranteDto dto)
        {
            
            if (await _db.Restaurantes.AsNoTracking().AnyAsync(r => r.PropietarioUid == propietarioUid))
                throw new InvalidOperationException("El usuario ya registró un restaurante.");

            var nombreNorm = NormalizarNombre(dto.Nombre);

          
            if (await _db.Restaurantes.AsNoTracking().AnyAsync(r => r.NombreNormalizado == nombreNorm))
                throw new ArgumentException("El nombre ya está en uso.");

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
                ActualizadoUtc = ahora
            };

            _db.Restaurantes.Add(entidad);
            await _db.SaveChangesAsync();

            return Map(entidad);
        }

        public async Task<RestauranteDto?> ObtenerAsync(Guid id)
        {
            var r = await _db.Restaurantes.FindAsync(id);
            return r is null ? null : Map(r);
        }

        public async Task<RestauranteDto?> ObtenerPorPropietarioAsync(string propietarioUid)
        {
            var r = await _db.Restaurantes.AsNoTracking()
                .FirstOrDefaultAsync(x => x.PropietarioUid == propietarioUid);
            return r is null ? null : Map(r);
        }

        public async Task<IReadOnlyList<RestauranteDto>> ListarCercanosAsync(double lat, double lng, int radioMetros)
        {
            
            double degLat = radioMetros / 111_000.0;
            double degLng = radioMetros / (111_000.0 * Math.Cos(lat * Math.PI / 180.0));

            var minLat = lat - degLat;
            var maxLat = lat + degLat;
            var minLng = lng - degLng;
            var maxLng = lng + degLng;

            var lista = await _db.Restaurantes.AsNoTracking()
                .Where(r => r.Latitud >= minLat && r.Latitud <= maxLat
                         && r.Longitud >= minLng && r.Longitud <= maxLng)
                .OrderBy(r => Math.Abs(r.Latitud - lat) + Math.Abs(r.Longitud - lng))
                .Take(200)
                .ToListAsync();

            return lista.Select(Map).ToList();
        }

        public async Task<RestauranteDto?> ActualizarAsync(Guid id, string solicitanteUid, bool esAdmin, ActualizarRestauranteDto dto)
        {
            var r = await _db.Restaurantes.FirstOrDefaultAsync(x => x.Id == id);
            if (r is null) return null;

            if (!esAdmin && r.PropietarioUid != solicitanteUid)
                throw new UnauthorizedAccessException("No tenés permisos para modificar este restaurante.");

            var nombreNorm = NormalizarNombre(dto.Nombre);
            if (await _db.Restaurantes.AnyAsync(x => x.Id != id && x.NombreNormalizado == nombreNorm))
                throw new ArgumentException("El nombre ya está en uso.");

            r.Nombre = dto.Nombre.Trim();
            r.NombreNormalizado = nombreNorm;
            r.Direccion = dto.Direccion.Trim();
            r.Latitud = dto.Latitud;
            r.Longitud = dto.Longitud;
            r.HorariosJson = dto.Horarios is null ? "{}" : JsonSerializer.Serialize(dto.Horarios);
            r.ActualizadoUtc = DateTime.UtcNow;

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

