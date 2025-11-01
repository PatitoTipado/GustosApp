using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases
{
    public class EliminarNotificacionUseCase
    {

        private readonly INotificacionRepository _repository;

        public EliminarNotificacionUseCase(INotificacionRepository repository)
        {
            _repository = repository;
        }
        public async Task HandleAsync(Guid notificacionId, CancellationToken ct)
        {
            if (notificacionId == Guid.Empty)
                throw new ArgumentException("El ID de notificación no puede ser vacío.");

            await _repository.EliminarAsync(notificacionId, ct);
        }
    }
}
