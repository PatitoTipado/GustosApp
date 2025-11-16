using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.UsuarioUseCases
{
    public class FinalizarRegistroUseCase
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly ICacheService _cache;

        public FinalizarRegistroUseCase(IUsuarioRepository usuarioRepo, ICacheService cache)
        {
            _usuarioRepo = usuarioRepo;
            _cache = cache;
        }

        public async Task HandleAsync(string uid, CancellationToken ct)
        {
            var usuario = await _usuarioRepo.GetByFirebaseUidAsync(uid, ct)
                ?? throw new Exception("Usuario no encontrado.");

            
            if (usuario.Gustos.Count < 3)
                throw new InvalidOperationException("Debes seleccionar al menos 3 gustos para completar el registro.");

            
            var tagsProhibidos = usuario.Restricciones
                .SelectMany(r => r.TagsProhibidos.Select(t => t.NombreNormalizado))
                .Concat(usuario.CondicionesMedicas
                    .SelectMany(c => c.TagsCriticos.Select(t => t.NombreNormalizado)))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var gustosInvalidos = usuario.Gustos
                .Where(g => g.Tags.Any(t => tagsProhibidos.Contains(t.NombreNormalizado)))
                .ToList();

            if (gustosInvalidos.Any())
            {
                throw new InvalidOperationException(
                    "Algunos gustos son incompatibles con tus restricciones: " +
                    string.Join(", ", gustosInvalidos.Select(g => g.Nombre))
                );
            }

          
            if (!usuario.RegistroInicialCompleto)
            {
                usuario.RegistroInicialCompleto = true;
            }

            
            await _usuarioRepo.SaveChangesAsync(ct);

            
            await _cache.DeleteAsync($"registro:{uid}:gustosTemp");
            await _cache.DeleteAsync($"registro:{uid}:restriccionesTemp");
            await _cache.DeleteAsync($"registro:{uid}:condicionesTemp");

           
            await _cache.SetAsync($"registro:{uid}:inicialCompleto", true, TimeSpan.FromHours(12));
        }
    }
}
