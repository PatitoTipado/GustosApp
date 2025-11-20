using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model.@enum;

namespace GustosApp.Domain.Model
{
    public class SolicitudRestaurante
    {
        public Guid Id { get; set; }

        // Usuario que hace la solicitud
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        // Datos básicos del restaurante
        public string Nombre { get; set; } = default!;
        public string Direccion { get; set; } = default!;
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }

        // Clasificación (Google Places / tipos)
        public string? PrimaryType { get; set; }
        public string TypesJson { get; set; } = "[]";

        // Horarios enviados del front (JSON)
        public string? HorariosJson { get; set; }

        public string? MotivoRechazo { get; set; }

        // Campos de negocio
        public List<string> Platos { get; set; } = new();     // Platos cargados por el usuario
        public List<Guid> GustosIds { get; set; } = new();    // IDs que luego se convertirán en relaciones
        public List<Guid> RestriccionesIds { get; set; } = new();

        // Estado de la solicitud
        public EstadoSolicitudRestaurante Estado { get; set; } = EstadoSolicitudRestaurante.Pendiente;

        // Fecha de creación
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        // Imágenes asociadas a esta solicitud
        public ICollection<SolicitudRestauranteImagen> Imagenes { get; set; }
            = new List<SolicitudRestauranteImagen>();
    }

}
