using System;

namespace GustosApp.Domain.Model
{
    public class VotoRestaurante
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid VotacionId { get; private set; }
        public Guid UsuarioId { get; private set; }
        public Guid RestauranteId { get; private set; }
        public DateTime FechaVoto { get; private set; } = DateTime.UtcNow;
        public string? Comentario { get; private set; }

        // Navegación
        public VotacionGrupo Votacion { get; set; }
        public Usuario Usuario { get; set; }
        public Restaurante Restaurante { get; set; }

        private VotoRestaurante() { } // Para EF Core

        public VotoRestaurante(Guid votacionId, Guid usuarioId, Guid restauranteId, string? comentario = null)
        {
            VotacionId = votacionId;
            UsuarioId = usuarioId;
            RestauranteId = restauranteId;
            Comentario = comentario;
        }

        public void ActualizarVoto(Guid nuevoRestauranteId, string? nuevoComentario = null)
        {
            RestauranteId = nuevoRestauranteId;
            Comentario = nuevoComentario;
            FechaVoto = DateTime.UtcNow;
        }
    }
}
