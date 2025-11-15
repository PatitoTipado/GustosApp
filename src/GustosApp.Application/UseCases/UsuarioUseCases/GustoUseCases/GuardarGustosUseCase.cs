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
        private readonly IRegistroPasoService _registroPasoService;

        public GuardarGustosUseCase(IUsuarioRepository userRepository,
            IGustoRepository gustoRepository, ICacheService cache,
            IRegistroPasoService registroPasoService)
        {
            _userRepository = userRepository;
            _gustoRepository = gustoRepository;
            _cache = cache;
            _registroPasoService = registroPasoService;
        }
        public async Task<List<string>> HandleAsync(string uid, List<Guid> ids, CancellationToken ct)
        {
            var usuario = await _userRepository.GetByFirebaseUidAsync(uid, ct)
                           ?? throw new Exception("Usuario no encontrado.");

            // Caso: usuario borra todo → resetear paso
            if (ids == null || ids.Count == 0)
            {
                usuario.Gustos.Clear();

                usuario.SetPaso(RegistroPaso.Gustos);

                await _registroPasoService.AplicarPasoAsync(
                    usuario,
                    RegistroPaso.Gustos,
                    $"registro:{uid}:gustos",
                    null,
                    ct
                );

                return new List<string>();
            }

            if (ids.Count < 3)
                throw new ArgumentException("Debe seleccionar al menos 3 gustos.");

            // Validación estricta
            var gustosValidos = await _gustoRepository.GetByIdsAsync(ids, ct);
            if (gustosValidos.Count != ids.Count)
                throw new ArgumentException("Uno o más gustos no existen.");

            // Comparar
            var actuales = usuario.Gustos.Select(g => g.Id).ToHashSet();
            var nuevas = ids.ToHashSet();

            var paraAgregar = nuevas.Except(actuales).ToList();
            var paraQuitar = actuales.Except(nuevas).ToList();

            bool huboCambios = false;

            // Quitar
            foreach (var g in usuario.Gustos.Where(x => paraQuitar.Contains(x.Id)).ToList())
            {
                usuario.Gustos.Remove(g);
                huboCambios = true;
            }

            // Agregar
            foreach (var g in gustosValidos.Where(x => paraAgregar.Contains(x.Id)))
            {
                usuario.Gustos.Add(g);
                huboCambios = true;
            }

            if (!huboCambios)
                return new List<string>();

            var conflictos = usuario.ValidarCompatibilidad();

            await _registroPasoService.AplicarPasoAsync(
                usuario,
                RegistroPaso.Verificacion,
                $"registro:{uid}:gustos",
                ids,
                ct
            );

            return conflictos;
        }
    }
    }
