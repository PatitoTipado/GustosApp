using System;

namespace GustosApp.API.DTO
{
    public class EnviarSolicitudRequest
    {
        public string UsernameDestino { get; set; } = null!;
        public string? Mensaje { get; set; }
    }
}
