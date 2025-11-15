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
        private readonly IRegistroPasoService _registroPasoService;

        public GuardarRestriccionesUseCase(IUsuarioRepository user, IRestriccionRepository restricciones,
           ICacheService cache, IRegistroPasoService registroPasoService
            )
        {
            _user = user;
            _restricciones = restricciones;
            _cache = cache;
            _registroPasoService = registroPasoService;
        }

        public async Task<List<string>> HandleAsync(
                  string uid,
                 List<Guid> ids,
                 bool skip,
                 CancellationToken ct)
        {
            var usuario = await _user.GetByFirebaseUidAsync(uid, ct)
                          ?? throw new InvalidOperationException("Usuario no encontrado.");


            if (skip)
            {
                usuario.Restricciones.Clear();

                await _registroPasoService.AplicarPasoAsync(
                    usuario,
                    RegistroPaso.Restricciones,
                    $"registro:{uid}:restricciones",
                    null,
                    ct
                );

                return new List<string>();
            }


            var entidadesValidas = await _restricciones.GetRestriccionesByIdsAsync(ids, ct);
            if (entidadesValidas.Count != ids.Count)
                throw new ArgumentException("Una o más restricciones no existen.");

            // Determinar cambios 
            var actuales = usuario.Restricciones.Select(r => r.Id).ToHashSet();
            var nuevas = ids.ToHashSet();

            var paraAgregar = nuevas.Except(actuales).ToList();
            var paraQuitar = actuales.Except(nuevas).ToList();

            bool huboCambios = false;

            // Quitar
            foreach (var r in usuario.Restricciones.Where(x => paraQuitar.Contains(x.Id)).ToList())
            {
                usuario.Restricciones.Remove(r);
                huboCambios = true;
            }

            // Agregar
            foreach (var r in entidadesValidas.Where(x => paraAgregar.Contains(x.Id)))
            {
                usuario.Restricciones.Add(r);
                huboCambios = true;
            }


            if (!huboCambios)
                return new List<string>();


            var gustosRemovidos = usuario.ValidarCompatibilidad();


            await _registroPasoService.AplicarPasoAsync(
                usuario,
                RegistroPaso.Condiciones,
                $"registro:{uid}:restricciones",
                ids,
                ct
            );

            return gustosRemovidos;
        }

    }

}
