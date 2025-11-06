using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
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
                public async Task HandleAsync(Guid usuarioDestinoId, TipoNotificacion tipo, string nombreUsuario, string nombreGrupo,  CancellationToken cancellationToken)
                {
                    var mensaje = tipo switch
                    {
                        TipoNotificacion.SolicitudAmistad => $"{nombreUsuario} te ha enviado una solicitud de amistad.",
                        TipoNotificacion.InvitacionGrupo => $"{nombreUsuario} te ha invitado a unirte al grupo {nombreGrupo}.",
                        TipoNotificacion.RecordatorioEvento => $"Recordatorio: Tienes un evento pendiente.",
                        TipoNotificacion.MensajeNuevo => $"Nuevo mensaje de {nombreUsuario}.",_=> "Tienes una nueva notificación."
                    };

                    var notificacion = new Notificacion
                    {
                        UsuarioDestinoId = usuarioDestinoId,
                        Titulo = tipo.ToString(),
                        Tipo = tipo,
                        Leida = false,
                        Mensaje = mensaje,
                        FechaCreacion = DateTime.UtcNow
                    };
                    await _repository.crearAsync(notificacion, cancellationToken);
                }
    }
}
