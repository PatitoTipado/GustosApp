using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model.@enum;

namespace GustosApp.Domain.Model
{
    public class SolicitudRestauranteImagen
    {
        public Guid Id { get; set; }

        public Guid SolicitudId { get; set; }
        public SolicitudRestaurante Solicitud { get; set; }

        public TipoImagenSolicitud Tipo { get; set; }
        public string Url { get; set; } = default!;
    }


}
