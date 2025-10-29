using GustosApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases
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
            await _repository.MarcarComoLeidaAsync(notificacionId, ct);
        }

    }
}
