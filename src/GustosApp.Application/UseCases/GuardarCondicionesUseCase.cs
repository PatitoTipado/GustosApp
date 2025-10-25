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
    public class GuardarCondicionesUseCase
    {
        private readonly ICondicionMedicaRepository _condiciones;
        private readonly IUsuarioRepository _user;

        public GuardarCondicionesUseCase(ICondicionMedicaRepository condiciones, IUsuarioRepository user)
        {
            _condiciones = condiciones;
            _user = user;
        }

        public async Task<List<string>> HandleAsync(string uid, List<Guid> ids, bool skip, CancellationToken ct)
        {
            if (skip)
                return new List<string>();

            var usuario = await _user.GetByFirebaseUidAsync(uid, ct)
                          ?? throw new InvalidOperationException("Usuario no encontrado.");

            // Obtener actuales y nuevas
            var actuales = usuario.CondicionesMedicas.Select(c => c.Id).ToHashSet();
            var nuevas = ids.ToHashSet();

            var paraAgregar = nuevas.Except(actuales).ToList();
            var paraQuitar = actuales.Except(nuevas).ToList();

            //  Quitar condiciones desmarcadas
            var paraEliminar = usuario.CondicionesMedicas
                .Where(c => paraQuitar.Contains(c.Id))
                .ToList();
            foreach (var condicion in paraEliminar)
                usuario.CondicionesMedicas.Remove(condicion);

            //  Agregar nuevas condiciones
            if (paraAgregar.Any())
            {
                var nuevasEntidades = await _condiciones.GetByIdsAsync(paraAgregar, ct);
                foreach (var condicion in nuevasEntidades)
                    usuario.CondicionesMedicas.Add(condicion);
            }

            //  Revalidar compatibilidad
            var gustosRemovidos = usuario.ValidarCompatibilidad();

            usuario.AvanzarPaso(RegistroPaso.Gustos);
            await _user.SaveChangesAsync(ct);

            return gustosRemovidos;
        }
    }
    }
