using Microsoft.AspNetCore.SignalR;

namespace GustosApp.API.Hubs
{
    public class VotacionesHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var grupoId = Context.GetHttpContext()?.Request.Query["grupoId"];

            if (!string.IsNullOrEmpty(grupoId))
                await Groups.AddToGroupAsync(Context.ConnectionId, grupoId);

            await base.OnConnectedAsync();
        }
    }
}
