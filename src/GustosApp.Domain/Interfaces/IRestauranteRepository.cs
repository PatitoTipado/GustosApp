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
            Task AddAsync(Restaurante restaurante, CancellationToken ct);
            Task SaveChangesAsync(CancellationToken ct);
            Task<List<Restaurante>> GetAllAsync(CancellationToken ct = default);
            Task<List<Restaurante>> buscarRestauranteParaUsuariosConGustosYRestricciones(List <string> gustos, List<string>restricciones, CancellationToken ct = default);


        Task<List<Restaurante>> GetNearbyAsync(
            double lat, double lng, int radiusMeters,
            TimeSpan? maxAge = null,
            CancellationToken ct = default);
    }
}