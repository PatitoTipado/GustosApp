using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace GustosApp.Infraestructure.Services
{
    public class FirebaseUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("user_id")?.Value;
        }
    }
}
