using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface IVotacionRepository
    {
        Task<VotacionGrupo> CrearVotacionAsync(VotacionGrupo votacion, CancellationToken ct = default);
        Task<VotacionGrupo?> ObtenerPorIdAsync(Guid votacionId, CancellationToken ct = default);
        Task<VotacionGrupo?> ObtenerVotacionActivaAsync(Guid grupoId, CancellationToken ct = default);
        Task<List<VotacionGrupo>> ObtenerHistorialVotacionesAsync(Guid grupoId, CancellationToken ct = default);
        Task<VotoRestaurante> RegistrarVotoAsync(VotoRestaurante voto, CancellationToken ct = default);
        Task<VotoRestaurante?> ObtenerVotoUsuarioAsync(Guid votacionId, Guid usuarioId, CancellationToken ct = default);
        Task ActualizarVotacionAsync(VotacionGrupo votacion, CancellationToken ct = default);
        Task<Dictionary<Guid, int>> ObtenerResultadosAsync(Guid votacionId, CancellationToken ct = default);
        Task<bool> UsuarioYaVotoAsync(Guid votacionId, Guid usuarioId, CancellationToken ct = default);
        Task<VotacionGrupo?> ObtenerPorIdConCandidatosAsync(Guid votacionId, CancellationToken ct);
        Task ActualizarVotoAsync(VotoRestaurante votoExistente, CancellationToken ct);
    }
}
