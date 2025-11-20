using Microsoft.AspNetCore.Mvc;

namespace GustosApp.API.DTO
{
    public class LoginFirebaseRequest
    {
        public string IdToken { get; set; }
        public string Tipo { get; set; }
    }
}
