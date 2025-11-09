using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.UsuarioUseCases.RestriccionesUseCases
{
    public class GuardarRestriccionesUseCase
    {
        private readonly IUsuarioRepository _user;
        private readonly IRestriccionRepository _restricciones;

        public GuardarRestriccionesUseCase(IUsuarioRepository user, IRestriccionRepository restricciones)
        {
            _user = user;
            _restricciones = restricciones;
        }

        public async Task<List<string>> HandleAsync(string uid, List<Guid> ids, bool skip, CancellationToken ct)
        {
            if (skip)
                return new List<string>();

            var usuario = await _user.GetByFirebaseUidAsync(uid, ct)
                          ?? throw new InvalidOperationException("Usuario no encontrado.");

            var actuales = usuario.Restricciones.Select(r => r.Id).ToHashSet();
            var nuevas = ids.ToHashSet();

            //qué agrega y qué quita
            var paraAgregar = nuevas.Except(actuales).ToList();
            var paraQuitar = actuales.Except(nuevas).ToList();

            // Quitar restricciones desmarcadas
            var paraEliminar = usuario.Restricciones
                .Where(r => paraQuitar.Contains(r.Id))
                .ToList();

            foreach (var restriccion in paraEliminar)
                usuario.Restricciones.Remove(restriccion);

            //  Agregar nuevas restricciones seleccionadas
            if (paraAgregar.Any())
            {
                var nuevasEntidades = await _restricciones.GetRestriccionesByIdsAsync(paraAgregar, ct);
                foreach (var restriccion in nuevasEntidades)
                    usuario.Restricciones.Add(restriccion);
            }

            //  Revalidar compatibilidad de gustos (por si cambian)
            var gustosRemovidos = usuario.ValidarCompatibilidad();

            //  Avanzar paso solo si aún no estaba completo
            usuario.AvanzarPaso(RegistroPaso.Condiciones);

            await _user.SaveChangesAsync(ct);

            return gustosRemovidos;

        }

    }
}
