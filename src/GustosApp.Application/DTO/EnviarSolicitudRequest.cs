using System;

namespace GustosApp.Application.DTO
{
    public class EnviarSolicitudRequest
    {
        public string UsernameDestino { get; set; } = null!;
        public string? Mensaje { get; set; }
    }
}
