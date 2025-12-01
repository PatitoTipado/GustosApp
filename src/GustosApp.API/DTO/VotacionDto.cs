using System;
using System.Collections.Generic;

namespace GustosApp.API.DTO
{
    // Requests
    public class IniciarVotacionRequest
    {
        public Guid GrupoId { get; set; }
        public string? Descripcion { get; set; }

        // Los restaurantes DEFINIDOS por el iniciador de la votación
        public List<Guid> RestaurantesCandidatos { get; set; } = new();
    }

    public class RegistrarVotoRequest
    {
        public Guid RestauranteId { get; set; }
        public string? Comentario { get; set; }
    }

    public class CerrarVotacionRequest
    {
        public Guid? RestauranteGanadorId { get; set; }
    }

    public class SeleccionarGanadorRequest
    {
        public Guid RestauranteGanadorId { get; set; }
    }

    // Responses
    public class VotacionResponse
    {
        public Guid Id { get; set; }
        public Guid GrupoId { get; set; }
        public string Estado { get; set; } = "";
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaCierre { get; set; }
        public Guid? RestauranteGanadorId { get; set; }
        public string? Descripcion { get; set; }
    }

    public class VotoResponse
    {
        public Guid Id { get; set; }
        public Guid VotacionId { get; set; }
        public Guid RestauranteId { get; set; }
        public DateTime FechaVoto { get; set; }
        public string? Comentario { get; set; }
    }

    public class ResultadoVotacionResponse
    {
        public Guid VotacionId { get; set; }
        public Guid GrupoId { get; set; }
        public string Estado { get; set; } = "";
        public bool TodosVotaron { get; set; }
        public int MiembrosActivos { get; set; }
        public int TotalVotos { get; set; }
        public List<RestauranteVotadoDto> RestaurantesVotados { get; set; } = new();
        public List<RestauranteCandidatoDto> RestaurantesCandidatos { get; set; } = new();
        public Guid? GanadorId { get; set; }
        public bool HayEmpate { get; set; }
        public List<Guid> RestaurantesEmpatados { get; set; } = new();
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaCierre { get; set; }
    }

    public class RestauranteVotadoDto
    {
        public Guid RestauranteId { get; set; }
        public string RestauranteNombre { get; set; } = "";
        public string RestauranteDireccion { get; set; } = "";
        public string RestauranteImagenUrl { get; set; } = "";
        public int CantidadVotos { get; set; }
        public List<VotanteInfoDto> Votantes { get; set; } = new();
    }

    public class VotanteInfoDto
    {
        public Guid UsuarioId { get; set; }

        public string? FirebaseUid { get; set; } 
        public string UsuarioNombre { get; set; } = "";
        public string UsuarioFoto { get; set; } = "";
        public string? Comentario { get; set; }


    }

    public class RestauranteCandidatoDto
    {
        public Guid RestauranteId { get; set; }
        public string Nombre { get; set; } = "";
        public string Direccion { get; set; } = "";
        public string ImagenUrl { get; set; } = "";
    }

}
