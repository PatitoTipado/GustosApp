using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Application.Services;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model.@enum;

namespace GustosApp.Application.UseCases.UsuarioUseCases.CondicionesMedicasUseCases
{
    public class GuardarCondicionesUseCase
    {
        private readonly IUsuarioRepository _user;
        private readonly ICondicionMedicaRepository _condiciones;
        private readonly ICacheService _cache;

        public GuardarCondicionesUseCase(
            IUsuarioRepository user,
            ICondicionMedicaRepository condiciones,
            ICacheService cache)
        {
            _user = user;
            _condiciones = condiciones;
            _cache = cache;
        }

        public async Task<List<string>> HandleAsync(
            string uid,
            List<Guid> ids,
            bool skip,
            ModoPreferencias modo,
            CancellationToken ct)
        {
            var usuario = await _user.GetByFirebaseUidAsync(uid, ct)
                ?? throw new InvalidOperationException("Usuario no encontrado.");

            //
            // SKIP → vaciar todas las condiciones
            //
            if (skip)
            {
                usuario.CondicionesMedicas.Clear();
                await _user.SaveChangesAsync(ct);

                // Guardar en redis SOLO si es registro
                if (modo == ModoPreferencias.Registro)
                {
                    await _cache.SetAsync(
                        $"registro:{uid}:condiciones",
                        new List<Guid>(),
                        TimeSpan.FromHours(12)
                    );
                }

                return usuario.ValidarCompatibilidad();
            }

            //
            //  Validar entidades enviadas
            //
            var entidadesValidas = await _condiciones.GetByIdsAsync(ids, ct);
            if (entidadesValidas.Count != ids.Count)
                throw new ArgumentException("Una o más condiciones médicas no existen.");

            //
            // Determinar cambios
            //
            var actuales = usuario.CondicionesMedicas.Select(c => c.Id).ToHashSet();
            var nuevas = ids.ToHashSet();

            var paraAgregar = nuevas.Except(actuales).ToList();
            var paraQuitar = actuales.Except(nuevas).ToList();

            bool huboCambios = paraAgregar.Any() || paraQuitar.Any();

            //
            //  Si no hubo cambios → solo actualizar Redis si es registro
            //
            if (!huboCambios)
            {
                if (modo == ModoPreferencias.Registro)
                {
                    await _cache.SetAsync(
                        $"registro:{uid}:condiciones",
                        ids,
                        TimeSpan.FromHours(12)
                    );
                }

                return new List<string>();
            }

            //
            // Aplicar cambios EN DB
            //

            // Quitar condiciones
            foreach (var c in usuario.CondicionesMedicas.Where(x => paraQuitar.Contains(x.Id)).ToList())
                usuario.CondicionesMedicas.Remove(c);

            // Agregar condiciones
            foreach (var c in entidadesValidas.Where(x => paraAgregar.Contains(x.Id)))
                usuario.CondicionesMedicas.Add(c);

            await _user.SaveChangesAsync(ct);

            //
            // 6Guardar en Redis SOLO si es registro
            //
            if (modo == ModoPreferencias.Registro)
            {
                await _cache.SetAsync(
                    $"registro:{uid}:condiciones",
                    ids,
                    TimeSpan.FromHours(12)
                );
            }

            //
            //  Validar compatibilidad (gustos removidos)
            //
            return usuario.ValidarCompatibilidad();
        }
    }

}

