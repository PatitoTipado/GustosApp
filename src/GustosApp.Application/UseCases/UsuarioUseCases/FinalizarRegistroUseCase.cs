using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.UsuarioUseCases
{
    public class FinalizarRegistroUseCase
    {
        private readonly IUsuarioRepository _usuarioRepo;

        public FinalizarRegistroUseCase(IUsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
        }

        public async Task HandleAsync(string uid, CancellationToken ct)
        {
            var usuario = await _usuarioRepo.GetByFirebaseUidAsync(uid, ct)
                ?? throw new Exception("Usuario no encontrado.");

            if (!usuario.Gustos.Any())
                throw new InvalidOperationException("Debe seleccionar al menos un gusto.");


            var tagsRestricciones = usuario.Restricciones
                .SelectMany(r => r.TagsProhibidos.Select(t => t.NombreNormalizado))
                .ToHashSet();

            var tagsProhibidos = usuario.Restricciones
             .SelectMany(r => r.TagsProhibidos.Select(t => t.NombreNormalizado))
             .Concat(usuario.CondicionesMedicas.SelectMany(c => c.TagsCriticos.Select(t => t.NombreNormalizado)))
              .ToHashSet(StringComparer.OrdinalIgnoreCase);



            var gustosInvalidos = usuario.Gustos
                .Where(g => g.Tags.Any(t => tagsProhibidos.Contains(t.NombreNormalizado)))
                .ToList();


            if (gustosInvalidos.Any())
            {
                throw new InvalidOperationException(
                    $"No se puede finalizar: algunos gustos son incompatibles con tus restricciones: {string.Join(", ", gustosInvalidos.Select(g => g.Nombre))}");
            }


            usuario.AvanzarPaso(RegistroPaso.Finalizado);

            await _usuarioRepo.SaveChangesAsync(ct);
        }
    }
}
