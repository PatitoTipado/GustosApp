using GustosApp.Domain.Model;

namespace GustosApp.Application.Interfaces
{
    public interface IOpinionRestauranteRepository
    {
        Task CrearAsync(OpinionRestaurante opinionRestaurante, CancellationToken cancellationToken);
        Task<List<OpinionRestaurante>> ObtenerPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken);
        Task<bool> ExisteValoracionAsync(Guid usuarioId, Guid restauranteId, CancellationToken cancellationToken);
        Task<List<OpinionRestaurante>> ObtenerPorRestauranteAsync(Guid restauranteId, CancellationToken cancellationToken);
    }
}