using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
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

        public async Task<List<Gusto>> HandleAsync(string firebaseUid, CancellationToken ct)
        {
            var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid, ct)
                          ?? throw new Exception("Usuario no encontrado");


            var todosLosGustos = await _gustos.GetAllAsync(ct);


            var tagsProhibidos = usuario.Restricciones
                .SelectMany(r => r.TagsProhibidos)
                .Concat(usuario.CondicionesMedicas.SelectMany(c => c.TagsCriticos))
                .Select(t => t.NombreNormalizado)
                .Distinct()
                .ToHashSet();

            var gustosFiltrados = todosLosGustos
                .Where(g => !g.Tags.Any(t => tagsProhibidos.Contains(t.NombreNormalizado)))
                .ToList();

          
            return gustosFiltrados;
        }
    }
    }
