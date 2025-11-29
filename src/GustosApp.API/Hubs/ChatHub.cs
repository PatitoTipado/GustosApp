namespace GustosApp.API.Hubs
{
    using global::GustosApp.Application.UseCases;
    using global::GustosApp.Application.UseCases.GrupoUseCases.ChatGrupoUseCases;
    using global::GustosApp.Domain.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    namespace GustosApp.API.Hubs
    {

        [Authorize]
        public class ChatHub : Hub
        {
            private static readonly Dictionary<string, string> Connections = new();

            private static readonly HashSet<string> Conectados = new();
            private readonly EnviarMensajeGrupoUseCase _enviarMensajeUseCase;
            private readonly ObtenerChatGrupoUseCase _obtenerChatGrupoUseCase;

            public ChatHub(EnviarMensajeGrupoUseCase enviarMensajeUseCase , ObtenerChatGrupoUseCase obtenerChatGrupoUseCase)
            {
                _enviarMensajeUseCase = enviarMensajeUseCase;
                _obtenerChatGrupoUseCase = obtenerChatGrupoUseCase;
            }


            public async Task JoinGroup(string groupId)
            {
                var firebaseUid = Context.User?.FindFirst("user_id")?.Value;
                if (firebaseUid == null) return;

                await Groups.AddToGroupAsync(Context.ConnectionId, groupId);

                var mensajes = await _obtenerChatGrupoUseCase
                    .HandleAsync(firebaseUid, Guid.Parse(groupId));

                var dtoMensaje = mensajes.Select(m => new
                {
                    usuario = m.UsuarioNombre,
                    mensaje = m.Mensaje,
                    fecha = m.FechaEnvio,
                    uid = m.Usuario?.FirebaseUid  // ⬅️ UID del autor; null para mensajes de sistema
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
                var firebaseUid = Context.User?.FindFirst("user_id")?.Value;
                if (firebaseUid == null) return;

                var saved = await _enviarMensajeUseCase.HandleAsync(firebaseUid, Guid.Parse(grupoId), mensaje);

                await Clients.Group(grupoId).SendAsync("ReceiveMessage", new
                {
                    usuario = saved.UsuarioNombre,
                    mensaje = saved.Mensaje,
                    fecha = saved.FechaEnvio,
                    uid = firebaseUid
                });
            }
            public override async Task OnConnectedAsync()
            {
                var uid = Context.User?.FindFirst("user_id")?.Value;
                if (uid != null)
                {
                    Conectados.Add(uid);

                    await Clients.All.SendAsync("UsuariosConectados", Conectados);
                }

                await base.OnConnectedAsync();
            }

            public override async Task OnDisconnectedAsync(Exception ex)
            {
                var uid = Context.User?.FindFirst("user_id")?.Value;
                if (uid != null)
                {
                    Conectados.Remove(uid);

                    await Clients.All.SendAsync("UsuariosConectados", Conectados);
                }

                await base.OnDisconnectedAsync(ex);
            }
        }
    }
        }
