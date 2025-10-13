using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Application.DTOs.Restaurantes;

namespace GustosApp.Application.Services
{
    public interface IServicioRestaurantes
    {
        Task<RestauranteDto> CrearAsync(string propietarioUid, CrearRestauranteDto dto);
        Task<RestauranteDto?> ObtenerAsync(Guid id);
        Task<RestauranteDto?> ObtenerPorPropietarioAsync(string propietarioUid);
        Task<RestauranteDto?> ActualizarAsync(Guid id, string solicitanteUid, bool esAdmin, ActualizarRestauranteDto dto);
        Task<bool> EliminarAsync(Guid id, string solicitanteUid, bool esAdmin);

        Task<IReadOnlyList<RestauranteDto>> BuscarAsync(
        string? tipo,
        string? plato,
        double? lat = null,
        double? lng = null,
        int? radioMetros = null
    );

    Task<IReadOnlyList<RestauranteDto>> ListarCercanosAsync(
        double lat, double lng, int radioMetros, string? tipo = null, IEnumerable<string>? platos = null);
    }
}
