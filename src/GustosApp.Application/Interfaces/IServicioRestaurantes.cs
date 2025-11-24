using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Interfaces
{
    public interface IServicioRestaurantes
    {
       // Task<Restaurante> CrearAsync(string propietarioUid, CrearRestauranteDto dto);
        Task<Restaurante?> ObtenerAsync(Guid id);

        Task<Restaurante?> ObtenerPorPropietarioAsync(Guid DuenoID);

        Task<bool> EliminarAsync(Guid id, string solicitanteUid, bool esAdmin);

        Task<List<Restaurante>> BuscarAsync(
        double rating,
        string? tipo,
        string? plato,
        double? lat = null,
        double? lng = null,
        int? radioMetros = null
    );

        Task<IReadOnlyList<Restaurante>> ListarCercanosAsync(
            double lat, double lng, int radioMetros, string? tipo = null, IEnumerable<string>? platos = null);
       
        Task<Restaurante> ObtenerResenasDesdeGooglePlaces(string placeId, CancellationToken ct);
    }
}
