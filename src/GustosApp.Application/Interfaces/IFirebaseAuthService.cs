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

        Task<DecodedToken> VerifyIdTokenAsync(string idToken);
    }

    // Esto es para testear el caso de uso AppLoginConFirebaseUseCase
    public class DecodedToken
    {
        public virtual string Uid { get; set; }
        public virtual Dictionary<string, object> Claims { get; set; }
    }
}
