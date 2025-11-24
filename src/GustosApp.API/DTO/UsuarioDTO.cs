using System.ComponentModel.DataAnnotations;
using GustosApp.Domain.Model;


namespace GustosApp.API.DTO
{


    public class GuardarIdsRequest()
    {

        public List<Guid> Ids { get; set; }
        public bool Skip { get; set; }
    }
    public class UsuarioResponse

    {
        public Guid Id { get; set; }
        public string FirebaseUid { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? FotoPerfilUrl { get; set; }
        public string Plan { get; set; } = "Free";
    }


    public class UsuarioSimpleResponse
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string? FotoPerfilUrl { get; set; }
        public string Username { get; set; }
    }

    public class RegistrarUsuarioResponse
    {

        public string Message { get; set; }
        public UsuarioResponse Usuario { get; set; }
    }



    public class UsuarioResumenResponse
    {
        public string Nombre { get; set; }
        public string? Apellido { get; set; }

        public List<ItemResumen> Restricciones { get; set; } = new();
        public List<ItemResumen> CondicionesMedicas { get; set; } = new();
        public List<ItemResumen> Gustos { get; set; } = new();
    }

    public class ItemResumen
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
    }





    public class UsuarioPerfilResponse
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? FotoPerfilUrl { get; set; }
        public bool EsPrivado { get; set; }
        public bool EsAmigo { get; set; }

        public bool EsMiPerfil { get; set; } = false;

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

    public class RegistrarUsuarioRequest
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")] public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")] public string Apellido { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")] public string Email { get; set; }
        public string? FotoPerfilUrl { get; set; }
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")] public string Username { get; set; }


    }

    public class EditarDatosUsuarioDTO
    {
        public string? Email { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }

        public IFormFile? FotoPerfil { get; set; }

        public bool EsPrivado { get; set; }
    }



}

