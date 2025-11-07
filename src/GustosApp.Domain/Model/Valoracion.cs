using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public class Valoracion
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UsuarioId { get; set; }
        public Guid RestauranteId { get; set; }
       
        public int ValoracionUsuario {  get; set; }
        public string? Comentario { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
       
        public Usuario? Usuario { get; set; }
        public Restaurante? Restaurante { get; set; }

        public Valoracion(Guid usuarioId, Guid restauranteId, int valoracionUsuario, string? comentario = null)
        {
            UsuarioId = usuarioId;
            RestauranteId = restauranteId;

            if(valoracionUsuario < 1 || valoracionUsuario > 5)
            {
                throw new ArgumentException("La valoración debe estar entre 1 y 5 estrellas.");
            }
            ValoracionUsuario = valoracionUsuario;
            Comentario = comentario;
        }

    }
}
