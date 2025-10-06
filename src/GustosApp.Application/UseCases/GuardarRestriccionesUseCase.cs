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
    public class GuardarRestriccionesUseCase
    {
        private readonly IUsuarioRepository _user;
        private readonly IRestriccionRepository _restricciones;

        public GuardarRestriccionesUseCase(IUsuarioRepository user, IRestriccionRepository restricciones)
        {
            _user = user;
            _restricciones = restricciones;
        }

        public async Task<GuardarRestriccionesResponse> HandleAsync(string uid, List<Guid> ids, bool skip, CancellationToken ct)
        {
            // 1️⃣ Si el usuario decidió omitir este paso
            if (skip)
                return new GuardarRestriccionesResponse("Paso omitido", new List<string>());

            // 2️⃣ Validar entrada
            if (ids == null || ids.Count == 0)
                throw new ArgumentException("Debe seleccionar al menos una restricción.");

            // 3️⃣ Buscar usuario
            var usuario = await _user.GetByFirebaseUidAsync(uid, ct)
                          ?? throw new InvalidOperationException("Usuario no encontrado.");

            // 4️⃣ Limpiar restricciones anteriores
            usuario.Restricciones.Clear();

            // 5️⃣ Cargar nuevas restricciones
            var nuevas = await _restricciones.GetRestriccionesByIdsAsync(ids, ct);

            foreach (var restriccion in nuevas)
                usuario.Restricciones.Add(restriccion);

            // 6️⃣ Validar compatibilidad (devuelve gustos eliminados)
            var gustosRemovidos = usuario.ValidarCompatibilidad();

            // 7️⃣ Avanzar paso
            usuario.AvanzarPaso(RegistroPaso.Condiciones);

            // 8️⃣ Guardar cambios
            await _user.SaveChangesAsync(ct);

            // 9️⃣ Devolver resultado
            return new GuardarRestriccionesResponse(
                "Restricciones actualizadas correctamente.",
                gustosRemovidos
            );
        }
    }
}
