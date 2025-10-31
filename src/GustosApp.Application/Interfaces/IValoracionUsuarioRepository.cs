using GustosApp.Domain.Model;

namespace GustosApp.Application.Interfaces
{
    public interface IValoracionUsuarioRepository
    {
        Task CrearAsync(Valoracion valoracion,CancellationToken cancellationToken);
        Task<List<Valoracion>> ObtenerPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken);
        Task<bool> ExisteValoracionAsync(Guid usuarioId, Guid restauranteId, CancellationToken cancellationToken);

    }
}