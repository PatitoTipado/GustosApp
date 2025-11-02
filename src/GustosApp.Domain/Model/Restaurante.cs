using System;
using System.Collections.Generic;

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks;


namespace GustosApp.Domain.Model
{
    public class Restaurante
    {
        public Guid Id { get; set; }
        public string PropietarioUid { get; set; } = string.Empty; // Firebase uid del dueño
        public string Nombre { get; set; } = string.Empty;
        public string NombreNormalizado { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    
        public string HorariosJson { get; set; } = "{}";
        public DateTime CreadoUtc { get; set; }
        public DateTime ActualizadoUtc { get; set; }

        public string PlaceId { get; set; } 
        
        public double? Rating { get; set; }
        public int? CantidadResenas { get; set; }
        public string? Categoria { get; set; }

        public DateTime UltimaActualizacion { get; set; } = DateTime.UtcNow;

        public string? WebUrl { get; set; }
        public string? EmbeddingVector { get; set; }

        public ICollection<ReseñaRestaurante> Reviews { get; set; } = new List<ReseñaRestaurante>();
        public ICollection<Gusto> GustosQueSirve { get; set; } = new List<Gusto>();
        public ICollection<Restriccion> RestriccionesQueRespeta { get; set; } = new List<Restriccion>();

        // ====== V2 ======
        public TipoRestaurante Tipo { get; set; }                 
        public string? ImagenUrl { get; set; }                
        public decimal? Valoracion { get; set; }                  
        public ICollection<RestaurantePlato> Platos { get; set; } = new List<RestaurantePlato>();
        public double Score { get; set; }
    }
}
