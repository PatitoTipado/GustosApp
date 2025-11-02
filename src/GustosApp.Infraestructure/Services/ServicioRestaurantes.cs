using System.Text.Json;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Services;
using GustosApp.Domain.Model;
using GustosApp.Infraestructure;
using Microsoft.EntityFrameworkCore;

public class ServicioRestaurantes : IServicioRestaurantes
{
    private readonly GustosDbContext _db;

    public ServicioRestaurantes(GustosDbContext db)
    {
        _db = db;
    }

    private static string NormalizarNombre(string nombre)
        => (nombre ?? string.Empty).Trim().ToLowerInvariant();

 
    public async Task<List<Restaurante>> BuscarAsync(
        string? tipo,
        string? plato,
        double? lat = null,
        double? lng = null,
        int? radioMetros = null)
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

        if (lat.HasValue && lng.HasValue && radioMetros.HasValue && radioMetros.Value > 0)
        {
            double degLat = radioMetros.Value / 111_000.0;
            double degLng = radioMetros.Value / (111_000.0 * Math.Cos(lat.Value * Math.PI / 180.0));

            var minLat = lat.Value - degLat;
            var maxLat = lat.Value + degLat;
            var minLng = lng.Value - degLng;
            var maxLng = lng.Value + degLng;

            q = q.Where(r =>
                r.Latitud >= minLat && r.Latitud <= maxLat &&
                r.Longitud >= minLng && r.Longitud <= maxLng);
        }

        return await q
            .OrderBy(r => r.NombreNormalizado)
            .Take(500)
            .ToListAsync();
    }

    public async Task<Restaurante> CrearAsync(string propietarioUid, CrearRestauranteDto dto)
    {
        var nombreNorm = NormalizarNombre(dto.Nombre);

        if (await _db.Restaurantes.AnyAsync(r => r.NombreNormalizado == nombreNorm))
            throw new ArgumentException("El nombre ya está en uso.");

        var entidad = new Restaurante
        {
            Id = Guid.NewGuid(),
            PropietarioUid = propietarioUid,
            Nombre = dto.Nombre.Trim(),
            NombreNormalizado = nombreNorm,
            Direccion = dto.Direccion.Trim(),
            Latitud = (double)dto.Lat,
            Longitud = (double)dto.Longitud,
            HorariosJson = dto.Horarios is null ? "{}" : JsonSerializer.Serialize(dto.Horarios),
            CreadoUtc = DateTime.UtcNow,
            ActualizadoUtc = DateTime.UtcNow,
            Tipo = Enum.TryParse<TipoRestaurante>(dto.Tipo, true, out var tipoEnum)
                ? tipoEnum
                : TipoRestaurante.Restaurante,
            ImagenUrl = dto.ImagenUrl,
            Valoracion = (decimal?)dto.Valoracion
        };

        _db.Restaurantes.Add(entidad);
        await _db.SaveChangesAsync();

        return entidad;
    }

    public async Task<Restaurante?> ObtenerAsync(Guid id)
    {
        return await _db.Restaurantes
            .Include(r => r.Platos)
            .Include(r => r.GustosQueSirve)
            .Include(r => r.RestriccionesQueRespeta)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Restaurante?> ObtenerPorPropietarioAsync(string propietarioUid)
    {
        return await _db.Restaurantes
            .Include(r => r.Platos)
            .Include(r => r.GustosQueSirve)
            .Include(r => r.RestriccionesQueRespeta)
            .FirstOrDefaultAsync(r => r.PropietarioUid == propietarioUid);
    }

    public async Task<Restaurante?> ActualizarAsync(Guid id, string solicitanteUid, bool esAdmin, ActualizarRestauranteDto dto)
    {
        var r = await _db.Restaurantes
            .Include(x => x.Platos)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (r is null) return null;

        if (!esAdmin && r.PropietarioUid != solicitanteUid)
            throw new UnauthorizedAccessException("No tenés permisos para modificar este restaurante.");

        r.Nombre = dto.Nombre.Trim();
        r.NombreNormalizado = NormalizarNombre(dto.Nombre);
        r.Direccion = dto.Direccion.Trim();
        r.Latitud = (double)dto.Latitud;
        r.Longitud = (double)dto.Longitud;
        r.HorariosJson = dto.Horarios is null ? "{}" : JsonSerializer.Serialize(dto.Horarios);
        r.ActualizadoUtc = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(dto.Tipo) &&
            Enum.TryParse<TipoRestaurante>(dto.Tipo, true, out var tipoEnum))
        {
            r.Tipo = tipoEnum;
        }

        r.ImagenUrl = dto.ImagenUrl;
        r.Valoracion = dto.Valoracion;

        await _db.SaveChangesAsync();
        return r;
    }

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

    public async Task<List<Restaurante>> ListarCercanosAsync(
        double lat, double lng, int radioMetros, string? tipo = null, IEnumerable<string>? platos = null)
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

        if (!string.IsNullOrWhiteSpace(tipo) &&
            Enum.TryParse<TipoRestaurante>(tipo, true, out var tipoParsed))
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
