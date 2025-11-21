using GustosApp.Domain.Model.@enum;

namespace GustosApp.API.DTO
{
    public class SolicitudRestaurantePendienteDto
    {
        public Guid Id { get; set; }
        public string NombreRestaurante { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioEmail { get; set; } = string.Empty;
        public string imgLogo { get; set; } 


        public DateTime FechaCreacionUtc { get; set; }
    }
    public class SolicitudRestauranteDetalleDto
    {
        public Guid Id { get; set; }

        // Datos del usuario solicitante
        public Guid UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioEmail { get; set; } = string.Empty;

        // Datos básicos del restaurante
        public string NombreRestaurante { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }

        // Tipo/categoría
        public string PrimaryType { get; set; } = string.Empty;
        public List<string> Types { get; set; } = new();

        // Horarios
        public string? HorariosJson { get; set; } 

        // Gustos y restricciones elegidos
        public List<ItemSimpleDto> Gustos { get; set; } = new();
        public List<ItemSimpleDto> Restricciones { get; set; } = new();

        // Imágenes agrupadas por tipo
        public string ImagenesDestacadas { get; set; }
        public List<string> ImagenesInterior { get; set; } = new();
        public List<string> ImagenesComida { get; set; } = new();
        public string? ImagenMenu { get; set; }
        public string? Logo { get; set; }

        // Metadatos
        public DateTime FechaCreacionUtc { get; set; }
        public List<HorarioSimpleDto> Horarios { get; set; } = new();

    }

    public class HorarioSimpleDto
    {
        public string Dia { get; set; } = default!;
        public bool Cerrado { get; set; }
        public string? Desde { get; set; } // "12:00"
        public string? Hasta { get; set; } // "22:00"
    }


}
