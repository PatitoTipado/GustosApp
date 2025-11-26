using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public class OpinionRestaurante
    {
        public Guid Id { get; set; }

        public Guid RestauranteId { get; set; }
        public Restaurante Restaurante { get; set; } = null!;

        //  Guarda los datos del user app
        public Guid? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        //  Si la opinion es externa se guarda los datos estos
        public string? AutorExterno { get; set; }
        public string? FuenteExterna { get; set; }
        public string? ImagenAutorExterno { get; set; }

      
        public double Valoracion { get; set; }

    
        public DateTime? FechaVisita { get; set; }

     
        public string? Titulo { get; set; }
        public string? Opinion { get; set; }

     
        public List<OpinionFoto> Fotos { get; set; } = new();

      
        public string? MotivoVisita { get; set; }  // Ej: "Negocio", "Pareja", "Familia", "Amigos", "Solo"
        public string? MesAnioVisita { get; set; } 

        // Metadatos
        public bool EsImportada { get; set; } = false;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;



    }
}
