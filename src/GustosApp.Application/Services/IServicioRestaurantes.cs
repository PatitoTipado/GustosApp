namespace GustosApp.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GustosApp.Application.DTOs.Restaurantes;

    public interface IServicioRestaurantes
    {
        Task<RestauranteDto> CrearAsync(string propietarioUid, CrearRestauranteDto dto);
        Task<RestauranteDto?> ObtenerAsync(Guid id);
        Task<RestauranteDto?> ObtenerPorPropietarioAsync(string propietarioUid);
        Task<IReadOnlyList<RestauranteDto>> ListarCercanosAsync(double lat, double lng, int radioMetros);
        Task<RestauranteDto?> ActualizarAsync(Guid id, string solicitanteUid, bool esAdmin, ActualizarRestauranteDto dto);
        Task<bool> EliminarAsync(Guid id, string solicitanteUid, bool esAdmin);
    }
}
