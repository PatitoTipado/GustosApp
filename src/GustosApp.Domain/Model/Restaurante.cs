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

        // deprecated pero mantenido por compatibilidad
         public string PropietarioUid { get; set; } = string.Empty;

        //Nuevo
        public Guid? DuenoId { get; set; }
        public Usuario? Dueno { get; set; }

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

        // Menú (OCR)
        public bool? MenuProcesado { get; set; }
        public string? MenuError { get; set; }

        public RestauranteMenu? Menu { get; set; }


        public ICollection<OpinionRestaurante> Reviews { get; set; } = new List<OpinionRestaurante>();

        public ICollection<Gusto> GustosQueSirve { get; set; } = new List<Gusto>();
        public ICollection<Restriccion> RestriccionesQueRespeta { get; set; } = new List<Restriccion>();

       
        // ====== V2 ======

        public string PrimaryType { get; set; } = "restaurant";
        public string TypesJson { get; set; } = "[]";

        public string? ImagenUrl { get; set; }                
        public decimal? Valoracion { get; set; }                  
        public ICollection<RestaurantePlato> Platos { get; set; } = new List<RestaurantePlato>();
        public double Score { get; set; }

        // Imágenes
     
        public string? LogoUrl { get; set; }
        public ICollection<RestauranteImagen> Imagenes { get; set; } = new List<RestauranteImagen>();

        public void SetGustos(IEnumerable<Gusto> nuevosGustos)
        {
            if (nuevosGustos == null)
                nuevosGustos = Enumerable.Empty<Gusto>();

            // Remover los que ya no están
            var idsNuevos = nuevosGustos.Select(g => g.Id).ToHashSet();

            var viejos = GustosQueSirve
                .Where(g => !idsNuevos.Contains(g.Id))
                .ToList();

            foreach (var r in viejos)
                GustosQueSirve.Remove(r);

            // Agregar los nuevos que no estaban
            var idsViejos = GustosQueSirve.Select(g => g.Id).ToHashSet();

            foreach (var g in nuevosGustos)
                if (!idsViejos.Contains(g.Id))
                    GustosQueSirve.Add(g);
        }

        public void SetRestricciones(IEnumerable<Restriccion> nuevasRestricciones)
        {
            if (nuevasRestricciones == null)
                nuevasRestricciones = Enumerable.Empty<Restriccion>();

            var idsNuevas = nuevasRestricciones.Select(r => r.Id).ToHashSet();

            // Remover las que ya no van
            var viejas = RestriccionesQueRespeta
                .Where(r => !idsNuevas.Contains(r.Id))
                .ToList();

            foreach (var r in viejas)
                RestriccionesQueRespeta.Remove(r);

            // Agregar nuevas
            var idsViejos = RestriccionesQueRespeta.Select(r => r.Id).ToHashSet();

            foreach (var r in nuevasRestricciones)
                if (!idsViejos.Contains(r.Id))
                    RestriccionesQueRespeta.Add(r);
        }



    }

}
