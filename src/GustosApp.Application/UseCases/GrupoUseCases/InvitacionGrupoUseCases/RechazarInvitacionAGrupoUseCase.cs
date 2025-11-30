using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.GrupoUseCases.InvitacionGrupoUseCases
{
    public class RechazarInvitacionAGrupoUseCase
    {

        private readonly IUsuarioRepository _usuarioRepo;
        private readonly INotificacionRepository _notificacionRepo;
        private readonly IInvitacionGrupoRepository _invitacionRepository;
       

        public RechazarInvitacionAGrupoUseCase(IUsuarioRepository usuarioRepo, INotificacionRepository notificacionRepo,
          IInvitacionGrupoRepository invitacionRepository)
        {
            _usuarioRepo = usuarioRepo;
            _notificacionRepo = notificacionRepo;
            _invitacionRepository = invitacionRepository;
          
        }



        public async Task HandleAsync(string uid, Guid notificacionId, CancellationToken ct)
        {
          
            var usuario = await _usuarioRepo.GetByFirebaseUidAsync(uid, ct);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

           
            var notificacion = await _notificacionRepo.GetByIdAsync(notificacionId, ct);
            if (notificacion == null)
                throw new ArgumentException("Notificación no encontrada");

           
            if (!notificacion.InvitacionId.HasValue)
                throw new ArgumentException("La notificación no está asociada a una invitación.");


            var invitacionId = notificacion.InvitacionId.Value;

            //  Obtener invitación
            var invitacion = await _invitacionRepository.GetByIdAsync(invitacionId, ct);
            if (invitacion == null)
                throw new ArgumentException("Invitación no encontrada");


            // Verificar destinatario correcto
            if (invitacion.UsuarioInvitadoId != usuario.Id)
                throw new UnauthorizedAccessException("Esta invitación no es para ti");

            // Estado debe ser pendiente
            if (invitacion.Estado != EstadoInvitacion.Pendiente)
                throw new ArgumentException("La invitación ya fue procesada");

            //  Verificar expiración
            if (invitacion.EstaExpirada())
                throw new ArgumentException("La invitación ha expirado");

            // Marcar como rechazada
            invitacion.Rechazar();
            await _invitacionRepository.UpdateAsync(invitacion, ct);

            //  Eliminar notificación
            await _notificacionRepo.EliminarAsync(notificacionId, ct);
        }
    }
}
