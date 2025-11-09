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

        public Guid UsuarioId { get; set; }
        public Guid RestauranteId { get; set; }

        public int Valoracion { get; set; }

        public string? Opinion { get; set; } 

        public string? Titulo { get; set; }

        public string? Img { get; set; } 

        public DateTime FechaCreacion { get; set; }
        public Usuario? Usuario { get; set; }
        public Restaurante? Restaurante { get; set; }

        public OpinionRestaurante(Guid usuarioId, Guid restauranteId, int valoracion, string? opinion = null, string? titulo = null,string? img = null)
        {
            UsuarioId = usuarioId;
            RestauranteId = restauranteId;
            Opinion = opinion ?? string.Empty;
            Titulo = titulo ?? string.Empty;
            Img = img ?? string.Empty;

            if (valoracion < 1 || valoracion > 5)
            {
                throw new ArgumentException("La valoración debe estar entre 1 y 5 estrellas.");
            }
            Valoracion = valoracion;
        }
    }
}
