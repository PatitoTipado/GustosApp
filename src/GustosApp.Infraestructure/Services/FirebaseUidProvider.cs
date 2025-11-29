using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace GustosApp.Infraestructure.Services
{
    public class FirebaseUidProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
            => connection.User?.FindFirst("user_id")?.Value!;
    }


}
