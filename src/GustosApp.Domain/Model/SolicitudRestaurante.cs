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

        // Horarios enviados del front (JSON)
        public string? HorariosJson { get; set; }

        public string? MotivoRechazo { get; set; }

        // Campos de negocio
        public List<Guid> GustosIds { get; set; } = new List<Guid>();
        public List<Guid> RestriccionesIds { get; set; } = new List<Guid>();

        public ICollection<Gusto> Gustos { get; set; } = new List <Gusto>();
        public ICollection<Restriccion> Restricciones { get; set; } =new List <Restriccion>();


        // Estado de la solicitud
        public EstadoSolicitudRestaurante Estado { get; set; } = EstadoSolicitudRestaurante.Pendiente;

        // Fecha de creación
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Imágenes asociadas a esta solicitud
        public ICollection<SolicitudRestauranteImagen> Imagenes { get; set; }
            = new List<SolicitudRestauranteImagen>();
    }

}
