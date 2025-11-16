using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases
{
    public class GuardarGustosUseCase
    {
        private readonly IUsuarioRepository _userRepository;
        private readonly IGustoRepository _gustoRepository;
        private readonly ICacheService _cache;

        public GuardarGustosUseCase(
            IUsuarioRepository userRepository,
            IGustoRepository gustoRepository,
            ICacheService cache)
        {
            _userRepository = userRepository;
            _gustoRepository = gustoRepository;
            _cache = cache;
        }

        public async Task<List<string>> HandleAsync(
            string uid,
            List<Guid> ids,
            ModoPreferencias modo,
            CancellationToken ct)
        {
            var usuario = await _userRepository.GetByFirebaseUidAsync(uid, ct)
                ?? throw new InvalidOperationException("Usuario no encontrado.");

            //
            // Caso: usuario borra todo → borrar gustos
            //
            if (ids == null || ids.Count == 0)
            {
                usuario.Gustos.Clear();
                await _userRepository.SaveChangesAsync(ct);

                // Guardar en redis SOLO si es registro
                if (modo == ModoPreferencias.Registro)
                {
                    await _cache.SetAsync(
                        $"registro:{uid}:gustos",
                        new List<Guid>(),
                        TimeSpan.FromHours(12)
                    );
                }

                return new List<string>();
            }

            //
            //  Validar mínimo 3 gustos
            //
            if (ids.Count < 3)
                throw new ArgumentException("Debe seleccionar al menos 3 gustos.");

            //
            //  Validación estricta: todos deben existir
            //
            var gustosValidos = await _gustoRepository.GetByIdsAsync(ids, ct);
            if (gustosValidos.Count != ids.Count)
                throw new ArgumentException("Uno o más gustos no existen.");

            //
            // Comparar actuales vs nuevas
            //
            var actuales = usuario.Gustos.Select(g => g.Id).ToHashSet();
            var nuevas = ids.ToHashSet();

            var paraAgregar = nuevas.Except(actuales).ToList();
            var paraQuitar = actuales.Except(nuevas).ToList();

            bool huboCambios = paraAgregar.Any() || paraQuitar.Any();

            //
            //  Si no hubo cambios → cachear en registro y terminar
            //
            if (!huboCambios)
            {
                if (modo == ModoPreferencias.Registro)
                {
                    await _cache.SetAsync(
                        $"registro:{uid}:gustos",
                        ids,
                        TimeSpan.FromHours(12)
                    );
                }

                return new List<string>();
            }

            //
            //  Aplicar cambios en DB (tu lógica original)
            //

            // Quitar
            foreach (var g in usuario.Gustos.Where(x => paraQuitar.Contains(x.Id)).ToList())
                usuario.Gustos.Remove(g);

            // Agregar
            foreach (var g in gustosValidos.Where(x => paraAgregar.Contains(x.Id)))
                usuario.Gustos.Add(g);

            await _userRepository.SaveChangesAsync(ct);

            //
            // 7Cache SOLO en registro
            //
            if (modo == ModoPreferencias.Registro)
            {
                await _cache.SetAsync(
                    $"registro:{uid}:gustos",
                    ids,
                    TimeSpan.FromHours(12)
                );
            }

            //
            // Validar compatibilidad (tu lógica original)
            //
            var conflictos = usuario.ValidarCompatibilidad();

            return conflictos;
        }
    }

}
