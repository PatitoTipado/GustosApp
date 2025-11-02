using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Services
{
    public interface IServicioRestaurantes
    {
        Task<Restaurante> CrearAsync(string propietarioUid, CrearRestauranteDto dto);
        Task<Restaurante?> ObtenerAsync(Guid id);
        Task<Restaurante?> ObtenerPorPropietarioAsync(string propietarioUid);
        Task<Restaurante?> ActualizarAsync(Guid id, string solicitanteUid, bool esAdmin, ActualizarRestauranteDto dto);
        Task<bool> EliminarAsync(Guid id, string solicitanteUid, bool esAdmin);

        Task<List<Restaurante>> BuscarAsync(
        string? tipo,
        string? plato,
        double? lat = null,
        double? lng = null,
        int? radioMetros = null
    );

    Task<List<Restaurante>> ListarCercanosAsync(
        double lat, double lng, int radioMetros, string? tipo = null, IEnumerable<string>? platos = null);
    }
}
