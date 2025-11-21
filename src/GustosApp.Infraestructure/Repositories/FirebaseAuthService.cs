using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using GustosApp.Application.Interfaces;
namespace GustosApp.Infraestructure.Repositories
{
 


    public class FirebaseAuthService : IFirebaseAuthService
    {
        public async Task SetUserRoleAsync(string firebaseUid, string rol)
        {
            await FirebaseAuth.DefaultInstance
                .SetCustomUserClaimsAsync(firebaseUid, new Dictionary<string, object>
                {
                { "rol", rol }
                });
        }
    }

}
