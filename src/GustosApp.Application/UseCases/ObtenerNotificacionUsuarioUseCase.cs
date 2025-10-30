using GustosApp.Application.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Tests.mocks
{
    public class ObtenerNotificacionUsuarioUseCase
    {
        private readonly INotificacionRepository _repository;

        public ObtenerNotificacionUsuarioUseCase(INotificacionRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Notificacion>> HandleAsync(Guid usuarioId, CancellationToken ct)
        {
            if (usuarioId == Guid.Empty)
                throw new ArgumentException("El ID del usuario no puede ser vacío.");


            var notificaciones= await _repository.ObtenerNotificacionPorUsuarioAsync(usuarioId, ct);

            return notificaciones;
        }
    }
}