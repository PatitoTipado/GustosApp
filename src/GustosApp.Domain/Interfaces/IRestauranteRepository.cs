using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface IRestauranteRepository
    {
      
            Task<Restaurante?> GetByPlaceIdAsync(string placeId, CancellationToken ct);

            Task<Restaurante?> GetRestauranteByIdAsync(Guid id, CancellationToken ct);
            Task AddAsync(Restaurante restaurante, CancellationToken ct);

             Task<Restaurante?> GetByIdAsync(Guid id, CancellationToken ct);
           

            Task SaveChangesAsync(CancellationToken ct);
            Task<List<Restaurante>> GetAllAsync(CancellationToken ct = default);
            Task<List<Restaurante>> buscarRestauranteParaUsuariosConGustosYRestricciones(List <string> gustos, List<string>restricciones, CancellationToken ct = default);
            Task<List<Restaurante>> ObtenerRestaurantesPorGustosGrupo(List<Guid> gustosIds, CancellationToken ct = default);
            Task<Restaurante?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken ct = default);

        Task<List<Restaurante>> GetNearbyAsync(
            double lat, double lng, int radiusMeters,
            TimeSpan? maxAge = null,
            CancellationToken ct = default);
        Task ActualizarValoracionAsync(Guid restauranteId, double promedio, CancellationToken cancellationToken);
        Task UpdateAsync(Restaurante restaurante, CancellationToken ct);
        Task <List<Restaurante>> BuscarPorPrefijo(string prefijo, CancellationToken ct = default);
   
        Task<List<Restaurante>> BuscarPorTextoAsync(string texto, CancellationToken ct = default);
        Task<List<Restaurante>> obtenerRestauranteConResenias(List<Guid> ids);
    }
}