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

namespace GustosApp.Application.UseCases.UsuarioUseCases.CondicionesMedicasUseCases
{
    public class GuardarCondicionesUseCase
    {
        private readonly ICondicionMedicaRepository _condiciones;
        private readonly IUsuarioRepository _user;
        private readonly ICacheService _cache;
        private readonly IRegistroPasoService _registroPasoService;

        public GuardarCondicionesUseCase(ICondicionMedicaRepository condiciones,
            IUsuarioRepository user,ICacheService cache, IRegistroPasoService registroPasoService)
        {
            _condiciones = condiciones;
            _user = user;
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
                usuario.CondicionesMedicas.Clear();

                await _registroPasoService.AplicarPasoAsync(
                    usuario,
                    RegistroPaso.Condiciones,
                    $"registro:{uid}:condiciones",
                    null,
                    ct
                );

                return new List<string>();
            }

          
            var entidadesValidas = await _condiciones.GetByIdsAsync(ids, ct);
            if (entidadesValidas.Count != ids.Count)
                throw new ArgumentException("Una o más condiciones médicas no existen.");

            //Determinar cambios 
            var actuales = usuario.CondicionesMedicas.Select(c => c.Id).ToHashSet();
            var nuevas = ids.ToHashSet();

            var paraAgregar = nuevas.Except(actuales).ToList();
            var paraQuitar = actuales.Except(nuevas).ToList();

            bool huboCambios = false;

            // Quitar
            foreach (var c in usuario.CondicionesMedicas.Where(x => paraQuitar.Contains(x.Id)).ToList())
            {
                usuario.CondicionesMedicas.Remove(c);
                huboCambios = true;
            }

            // Agregar
            foreach (var c in entidadesValidas.Where(x => paraAgregar.Contains(x.Id)))
            {
                usuario.CondicionesMedicas.Add(c);
                huboCambios = true;
            }

            if (!huboCambios)
                return new List<string>();

         
            var gustosRemovidos = usuario.ValidarCompatibilidad();

         
            await _registroPasoService.AplicarPasoAsync(
                usuario,
                RegistroPaso.Gustos,
                $"registro:{uid}:condiciones",
                ids,
                ct
            );

            return gustosRemovidos;
        }

    }
}

