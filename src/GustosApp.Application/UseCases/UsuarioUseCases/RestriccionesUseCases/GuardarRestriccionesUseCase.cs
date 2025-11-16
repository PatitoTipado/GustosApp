using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Application.Services;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.UsuarioUseCases.RestriccionesUseCases
{
    public class GuardarRestriccionesUseCase
    {
        private readonly IUsuarioRepository _user;
        private readonly IRestriccionRepository _restricciones;
        private readonly ICacheService _cache;

        public GuardarRestriccionesUseCase(
            IUsuarioRepository user,
            IRestriccionRepository restricciones,
            ICacheService cache)
        {
            _user = user;
            _restricciones = restricciones;
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
            //  SKIP → vaciar todas las restricciones
            //
            if (skip)
            {
                usuario.Restricciones.Clear();
                await _user.SaveChangesAsync(ct);

                // Redis solo en registro
                if (modo == ModoPreferencias.Registro)
                {
                    await _cache.SetAsync(
                        $"registro:{uid}:restricciones",
                        new List<Guid>(),
                        TimeSpan.FromHours(12)
                    );
                }

                return usuario.ValidarCompatibilidad();
            }

            //
            //  Validar entidades
            //
            var entidadesValidas = await _restricciones.GetRestriccionesByIdsAsync(ids, ct);
            if (entidadesValidas.Count != ids.Count)
                throw new ArgumentException("Una o más restricciones no existen.");

            //
            // Determinar cambios
            //
            var actuales = usuario.Restricciones.Select(r => r.Id).ToHashSet();
            var nuevas = ids.ToHashSet();

            var paraAgregar = nuevas.Except(actuales).ToList();
            var paraQuitar = actuales.Except(nuevas).ToList();

            bool huboCambios = paraAgregar.Any() || paraQuitar.Any();

            //
            //  Si no hubo cambios → solo cache (en registro)
            //
            if (!huboCambios)
            {
                if (modo == ModoPreferencias.Registro)
                {
                    await _cache.SetAsync(
                        $"registro:{uid}:restricciones",
                        ids,
                        TimeSpan.FromHours(12)
                    );
                }

                return new List<string>();
            }

            //
            //  Aplicar cambios EN DB
            //
            foreach (var r in usuario.Restricciones.Where(x => paraQuitar.Contains(x.Id)).ToList())
                usuario.Restricciones.Remove(r);

            foreach (var r in entidadesValidas.Where(x => paraAgregar.Contains(x.Id)))
                usuario.Restricciones.Add(r);

            await _user.SaveChangesAsync(ct);

            //
            //  Guardar en Redis solo si es registro
            //
            if (modo == ModoPreferencias.Registro)
            {
                await _cache.SetAsync(
                    $"registro:{uid}:restricciones",
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
