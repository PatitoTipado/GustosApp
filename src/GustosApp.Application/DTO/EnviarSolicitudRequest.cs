using System;

namespace GustosApp.Application.DTO
{
    public class EnviarSolicitudRequest
    {
        public string EmailDestino { get; set; }
        public string? Mensaje { get; set; }
    }
}
