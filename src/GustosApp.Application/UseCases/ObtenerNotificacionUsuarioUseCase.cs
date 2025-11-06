using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
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

        public async Task <Notificacion> HandleAsync(Guid notificacionId,CancellationToken ct)
        {
               return await _repository.GetByIdAsync(notificacionId, ct);
        }

    }
        public class ObtenerNotificacionesUsuarioUseCase
    {
        private readonly INotificacionRepository _repository;

        public ObtenerNotificacionesUsuarioUseCase(INotificacionRepository repository)
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