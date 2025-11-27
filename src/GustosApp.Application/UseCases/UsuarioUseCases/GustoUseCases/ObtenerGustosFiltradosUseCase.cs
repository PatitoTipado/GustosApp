using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Application.Model;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases
{
    public class ObtenerGustosFiltradosUseCase
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly IGustoRepository _gustos;
        public ObtenerGustosFiltradosUseCase(IUsuarioRepository usuarios, IGustoRepository gustos)
        {
            _usuarios = usuarios;
            _gustos = gustos;
        }

        public async Task<GustosFiltradosResult> HandleAsync(string firebaseUid, CancellationToken ct)
        {
            var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid, ct)
                          ?? throw new Exception("Usuario no encontrado");

            var todosLosGustos = await _gustos.GetAllAsync(ct);

            var tagsProhibidos = (usuario.Restricciones ?? Enumerable.Empty<Restriccion>())
                .SelectMany(r => r.TagsProhibidos ?? Enumerable.Empty<Tag>())
                .Concat((usuario.CondicionesMedicas ?? Enumerable.Empty<CondicionMedica>())
                    .SelectMany(c => c.TagsCriticos ?? Enumerable.Empty<Tag>()))
                .Select(t => t.NombreNormalizado)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToHashSet();

            var gustosFiltrados = todosLosGustos
                .Where(g => !(g.Tags ?? new List<Tag>())
                    .Any(t => tagsProhibidos.Contains(t.NombreNormalizado)))
                .ToList();

            return new GustosFiltradosResult
            {
                GustosFiltrados = gustosFiltrados,
                GustosSeleccionados = usuario.Gustos.Select(g => g.Id).ToList()
            };
        }
    }
}
