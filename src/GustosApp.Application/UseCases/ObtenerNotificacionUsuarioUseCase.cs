using GustosApp.Application.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Tests.mocks
{
    public class ObtenerNotificacionUsuarioUseCase
    {
        private readonly INotificacionRepository _repository;

        public ObtenerNotificacionUsuarioUseCase(INotificacionRepository repository)
        {
            this._repository = repository;
        }

        public async Task<List<Notificacion>> HandleAsync(Guid usuarioId, CancellationToken ct)
        {
            return await _repository.ObtenerNotificacionPorUsuarioAsync(usuarioId, ct);
        }
    }
}