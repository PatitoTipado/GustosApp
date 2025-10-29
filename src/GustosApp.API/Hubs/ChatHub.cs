namespace GustosApp.API.Hubs
{
    using global::GustosApp.Application.UseCases;
    using global::GustosApp.Domain.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    namespace GustosApp.API.Hubs
    {

        [Authorize]
        public class ChatHub : Hub
        {

            private readonly EnviarMensajeGrupoUseCase _enviarMensajeUseCase;
            private readonly ObtenerChatGrupoUseCase _obtenerChatGrupoUseCase;

            public ChatHub(EnviarMensajeGrupoUseCase enviarMensajeUseCase , ObtenerChatGrupoUseCase obtenerChatGrupoUseCase)
            {
                _enviarMensajeUseCase = enviarMensajeUseCase;
                _obtenerChatGrupoUseCase = obtenerChatGrupoUseCase;
            }

            
                public async Task JoinGroup(string groupId)
                {
                     var uid = Context.User?.FindFirst("user_id")?.Value;
                        if (uid == null) return;

                        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);

                    var mensajes = await _obtenerChatGrupoUseCase.HandleAsync(uid,Guid.Parse(groupId));
                    var dtoMensaje = mensajes.Select(m => new
                    {
                        usuario = m.UsuarioNombre,
                        mensaje = m.Mensaje,
                        fecha = m.FechaEnvio
                    }).ToList();

                await Clients.Caller.SendAsync("LoadChatHistory", dtoMensaje);

                    await Clients.Group(groupId)
                        .SendAsync("UserJoined", $"{Context.ConnectionId} se unió al grupo {groupId}");
                }
       

            public async Task LeaveGroup(string grupoId)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, grupoId);
                await Clients.Group(grupoId).SendAsync("UserLeft", $"{Context.User?.Identity?.Name} salió del chat");
            }

            public async Task SendMessageToGroup(string grupoId, string mensaje)
            {
                var uid = Context.User?.FindFirst("user_id")?.Value;
                if (uid == null) return;

                var saved = await _enviarMensajeUseCase.HandleAsync(uid, Guid.Parse(grupoId), mensaje);

                //devolver mapeo de mensaje dto
                await Clients.Group(grupoId).SendAsync("ReceiveMessage", new
                {
                    usuario = saved.UsuarioNombre,
                    mensaje = saved.Mensaje,
                    fecha = saved.FechaEnvio
                });
            }
        }
    }
        }
