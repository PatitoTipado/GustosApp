using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases
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

        public async Task<List<GustoDto>> HandleAsync(string firebaseUid, int inicio, int final)
        {
            // Obtener usuario
            var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            // Obtener gustos filtrados del grupo (por ejemplo paginados)
            var gustosGrupo = _gustos.obtenerGustosPorPaginacion(inicio, final); // List<Gusto>

            var gustosUsuarioIds = usuario.Gustos.Select(u => u.Id).ToHashSet();

            // Mezclar y priorizar gustos del usuario
            var gustosCombinados = gustosGrupo
                .Concat(usuario.Gustos)                 // agregar gustos del usuario
                .GroupBy(g => g.Id)                     // quitar duplicados
                .Select(g => g.First())
                .OrderByDescending(g => gustosUsuarioIds.Contains(g.Id)) // priorizar los del usuario
                .Take(10)                               // máximo 10
                .Select(g => new GustoDto
                {
                    Id = g.Id,
                    Nombre = g.Nombre,
                    ImagenUrl = g.ImagenUrl,
                    Seleccionado = gustosUsuarioIds.Contains(g.Id) // marcar seleccionado
                })
                .ToList();

            return gustosCombinados;
        }


    }
}
