using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Interfaces
{
    public interface IFirebaseAuthService
    {
        Task SetUserRoleAsync(string firebaseUid, string rol);

        Task UpdateUserEmailAsync(string firebaseUid, string Email);
    }


}
