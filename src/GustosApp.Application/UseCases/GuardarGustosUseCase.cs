using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
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
        public async Task HandleAsync(string uid, List<Guid> ids, CancellationToken ct)
        {
          
            if (ids.Count < 3)
                throw new ArgumentException("Debe seleccionar al menos 3 gustos.");

           
            var usuario = await _userRepository.GetByFirebaseUidAsync(uid, ct)
                          ?? throw new Exception("Usuario no encontrado.");


            var gustosSeleccionados = await _gustoRepository.GetByIdsAsync(ids, ct);

            if (!gustosSeleccionados.Any())
                throw new Exception("No se encontraron gustos válidos con los IDs proporcionados.");

       
            foreach (var gusto in gustosSeleccionados)
            {
                if (!usuario.Gustos.Any(g => g.Id == gusto.Id))
                {
                    usuario.Gustos.Add(gusto);
                }
            }

            usuario.AvanzarPaso(RegistroPaso.Verificacion);

            await _userRepository.SaveChangesAsync(ct);
        }
    }
    }
