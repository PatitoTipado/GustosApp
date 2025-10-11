<<<<<<< HEAD
﻿namespace GustosApp.Domain.Model
{
    public class Restaurante
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }

        public List<RestauranteEspecialidad> Especialidad { get; set; } = new List<RestauranteEspecialidad>();
        
        public Restaurante(Guid id, string nombre,decimal latitud,decimal longitud,List<RestauranteEspecialidad> especialidad)
        {
            Id = id;
            Especialidad = especialidad;
            Nombre = nombre;
            Latitud = latitud;
            Longitud = longitud;
        }

    }
}
=======


using System;
using System.Collections.Generic;

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <summary>
        /// JSON con horarios por día y tz. Se persiste como string para simplicidad inicial.
        /// </summary>
        public string HorariosJson { get; set; } = "{}";
        public DateTime CreadoUtc { get; set; }
        public DateTime ActualizadoUtc { get; set; }

        

        public string PlaceId { get; set; } = string.Empty;
        
        public double? Rating { get; set; }
        public int? CantidadResenas { get; set; }
        public string? Categoria { get; set; }
        public string? ImagenUrl { get; set; }
        public DateTime UltimaActualizacion { get; set; } = DateTime.UtcNow;

        public string? WebUrl { get; set; }
        public string? EmbeddingVector { get; set; }

        public ICollection<ReviewRestaurante> Reviews { get; set; } = new List<ReviewRestaurante>();

    }
}
>>>>>>> origin/develop
