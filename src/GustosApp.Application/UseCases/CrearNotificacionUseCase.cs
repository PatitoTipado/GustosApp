using GustosApp.Application.Interfaces;
using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases
{
    public class CrearNotificacionUseCase
    {
        private readonly INotificacionRepository _repository;

        public CrearNotificacionUseCase(INotificacionRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(Guid usuarioDestinoId,string titulo,string mensaje,TipoNotificacion tipo,CancellationToken cancellationToken)
        {
            var notificacion = new Notificacion
            {
                UsuarioDestinoId = usuarioDestinoId,
                Titulo = titulo,
                Mensaje = mensaje,
                Tipo = tipo,
                Leida = false,
                FechaCreacion = DateTime.UtcNow
            };
            await _repository.crearAsync(notificacion, cancellationToken);
        }
    
         }
}
