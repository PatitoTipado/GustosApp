using System;
using System.Collections.Generic;

namespace GustosApp.API.DTO
{
    public class UsuarioPerfilResponse
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? FotoPerfilUrl { get; set; }
        public bool EsPrivado { get; set; }

        public List<GustoLiteDto> Gustos { get; set; } = new();
        public List<VisitadoDto> Visitados { get; set; } = new();
    }

    public class GustoLiteDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class VisitadoDto
    {
        
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
