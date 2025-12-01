using System;
using System.Collections.Generic;
using System.Linq;

namespace GustosApp.Domain.Model
{
    public enum EstadoVotacion
    {
        Activa,
        Cerrada,
        Cancelada
    }

    public class VotacionGrupo
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid GrupoId { get; private set; }
        public DateTime FechaInicio { get; private set; } = DateTime.UtcNow;
        public DateTime? FechaCierre { get; private set; }
        public EstadoVotacion Estado { get; private set; } = EstadoVotacion.Activa;
        public Guid? RestauranteGanadorId { get; private set; }
        public string? Descripcion { get; private set; }
        
        // Navegación
        public Grupo Grupo { get; set; }
        public ICollection<VotoRestaurante> Votos { get; set; } = new List<VotoRestaurante>();

        public ICollection<VotacionRestaurante> RestaurantesCandidatos { get; set; }
         = new List<VotacionRestaurante>();

        public Restaurante? RestauranteGanador { get; set; }

        private VotacionGrupo() { } // Para EF Core

        public VotacionGrupo(Guid grupoId, string? descripcion = null)
        {
            GrupoId = grupoId;
            Descripcion = descripcion;
        }

        public void CerrarVotacion(Guid? restauranteGanadorId = null)
        {
            if (Estado != EstadoVotacion.Activa)
                throw new InvalidOperationException("Solo se pueden cerrar votaciones activas");

            Estado = EstadoVotacion.Cerrada;
            FechaCierre = DateTime.UtcNow;
            RestauranteGanadorId = restauranteGanadorId;
        }

        public void EstablecerGanadorRuleta(Guid restauranteGanadorId)
        {
            if (Estado != EstadoVotacion.Activa)
                throw new InvalidOperationException("Solo se puede establecer ganador en votaciones activas");

            if (RestauranteGanadorId.HasValue)
                throw new InvalidOperationException("Ya se seleccionó un ganador para esta votación");

            RestauranteGanadorId = restauranteGanadorId;
        }

        public void Cancelar()
        {
            if (Estado == EstadoVotacion.Cerrada)
                throw new InvalidOperationException("No se puede cancelar una votación cerrada");

            Estado = EstadoVotacion.Cancelada;
            FechaCierre = DateTime.UtcNow;
        }

        public Dictionary<Guid, int> ObtenerResultados()
        {
            return Votos
                .GroupBy(v => v.RestauranteId)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public bool TodosHanVotado(int miembrosActivos)
        {
            var votosUnicos = Votos.Select(v => v.UsuarioId).Distinct().Count();
            return votosUnicos >= miembrosActivos;
        }

        public List<Guid> ObtenerRestaurantesEmpatados()
        {
            var resultados = ObtenerResultados();
            if (!resultados.Any()) return new List<Guid>();

            var maxVotos = resultados.Max(r => r.Value);
            return resultados
                .Where(r => r.Value == maxVotos)
                .Select(r => r.Key)
                .ToList();
        }
    }
}
