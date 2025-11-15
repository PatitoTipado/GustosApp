
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GustosApp.API.Controllers
{
    

    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected string GetFirebaseUid()
        {
            var firebaseUid = User.FindFirst("user_id")?.Value
                            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
                throw new UnauthorizedAccessException("No se encontró el UID de Firebase en el token.");

            return firebaseUid;
        }
    }

}
