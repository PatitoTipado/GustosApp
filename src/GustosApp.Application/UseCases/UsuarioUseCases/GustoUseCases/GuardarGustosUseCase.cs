using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases
{
    public class GuardarGustosUseCase
    {
        private readonly IUsuarioRepository _userRepository;
        private readonly IGustoRepository _gustoRepository;

        public GuardarGustosUseCase(IUsuarioRepository userRepository, IGustoRepository gustoRepository)
        {
            _userRepository = userRepository;
            _gustoRepository = gustoRepository;
        }
        public async Task<List<string>> HandleAsync(string uid, List<Guid> ids, CancellationToken ct)
        {
            if (ids == null || ids.Count < 3)
                throw new ArgumentException("Debe seleccionar al menos 3 gustos.");

            var usuario = await _userRepository.GetByFirebaseUidAsync(uid, ct)
                          ?? throw new Exception("Usuario no encontrado.");

            var actuales = usuario.Gustos.Select(g => g.Id).ToHashSet();
            var nuevas = ids.ToHashSet();

            var paraAgregar = nuevas.Except(actuales).ToList();
            var paraQuitar = actuales.Except(nuevas).ToList();

            // Quitar gustos desmarcados
            var paraEliminar = usuario.Gustos
                .Where(g => paraQuitar.Contains(g.Id))
                .ToList();
            foreach (var gusto in paraEliminar)
                usuario.Gustos.Remove(gusto);

            // Agregar nuevos gustos
            if (paraAgregar.Any())
            {
                var nuevos = await _gustoRepository.GetByIdsAsync(paraAgregar, ct);
                foreach (var gusto in nuevos)
                    usuario.Gustos.Add(gusto);
            }

            // Validar compatibilidad y avanzar
            var conflictos = usuario.ValidarCompatibilidad();
            usuario.AvanzarPaso(RegistroPaso.Verificacion);

            await _userRepository.SaveChangesAsync(ct);

            return conflictos;
        }
    }
}
