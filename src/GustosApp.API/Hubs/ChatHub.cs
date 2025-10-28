namespace GustosApp.API.Hubs
{
    using global::GustosApp.Application.UseCases;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    namespace GustosApp.API.Hubs
    {

        [Authorize]
        public class ChatHub : Hub
        {

            private readonly EnviarMensajeGrupoUseCase _enviarMensajeUseCase;

            public ChatHub(EnviarMensajeGrupoUseCase enviarMensajeUseCase)
            {
                _enviarMensajeUseCase = enviarMensajeUseCase;
            }

            public async Task JoinGroup(string grupoId)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, grupoId);
                await Clients.Group(grupoId).SendAsync("UserJoined", $"{Context.User?.Identity?.Name} se unió al chat");
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
