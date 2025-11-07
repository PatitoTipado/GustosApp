using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases
{
    public class ObtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase
    {

        private readonly IUsuarioRepository _usuarios;
        private readonly IGustoRepository _gustos;

        public ObtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase(IUsuarioRepository usuarios, IGustoRepository gustos)
        {
            _usuarios = usuarios;
            _gustos = gustos;
        }

        public async Task<List<Gusto>> HandleAsync(string firebaseUid, int inicio, int final)
        {
            var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            var gustosGrupo = _gustos.obtenerGustosPorPaginacion(inicio, final);

            var gustosUsuarioIds = usuario.Gustos.Select(u => u.Id).ToHashSet();

            // Combinar y priorizar gustos
            var gustosCombinados = gustosGrupo
                .Concat(usuario.Gustos)
                .GroupBy(g => g.Id)
                .Select(g => g.First())
                .OrderByDescending(g => gustosUsuarioIds.Contains(g.Id))
                .Take(10)
                .ToList();

            return gustosCombinados;
        }
    }
}
