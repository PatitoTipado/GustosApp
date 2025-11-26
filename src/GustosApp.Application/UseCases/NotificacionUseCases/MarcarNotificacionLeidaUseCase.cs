using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases.NotificacionUseCases
{
    public class MarcarNotificacionLeidaUseCase
    {
        private readonly INotificacionRepository _repository;

        public MarcarNotificacionLeidaUseCase(INotificacionRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(Guid notificacionId, CancellationToken ct)
        {
            if (notificacionId == Guid.Empty)
                throw new ArgumentException("El ID de notificación no puede ser vacío.");

            await _repository.MarcarComoLeidaAsync(notificacionId, ct);
        }

    }
}
