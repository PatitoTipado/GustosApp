using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.AmistadUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases.UsuarioUseCases
{
    public class ConstruirPreferenciasUseCase : IConstruirPreferencias
    {
        private readonly ObtenerUsuarioUseCase _obtenerUsuario;
        private readonly ObtenerGustosUseCase _obtenerGustosUser;
        private readonly ConfirmarAmistadEntreUsuarios _confirmarAmistad;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IGustosGrupoRepository _gustosGrupoRepo;
        private readonly IMiembroGrupoRepository _miembroGrupoRepository;

        public ConstruirPreferenciasUseCase(
            ObtenerUsuarioUseCase obtenerUsuario,
            ObtenerGustosUseCase obtenerGustosUser,
            ConfirmarAmistadEntreUsuarios confirmarAmistad,
            IUsuarioRepository usuarioRepo,
            IGustosGrupoRepository gustosGrupoRepo,
            IMiembroGrupoRepository miembroGrupoRepository)
        {
            _obtenerUsuario = obtenerUsuario;
            _obtenerGustosUser = obtenerGustosUser;
            _confirmarAmistad = confirmarAmistad;
            _usuarioRepo = usuarioRepo;
            _gustosGrupoRepo = gustosGrupoRepo;
            _miembroGrupoRepository = miembroGrupoRepository;
        }

        public async Task<UsuarioPreferencias> HandleAsync(
            string firebaseUid,
            string? amigoUsername,
            Guid? grupoId,
            List<string>? gustosDelFiltro,
            CancellationToken ct)
        {
            if (grupoId.HasValue)
            {
                var gustosGrupo = await _gustosGrupoRepo.ObtenerGustosDelGrupo(grupoId.Value);
                var obtenerPrefDeMiembrosValidos = _miembroGrupoRepository.obtenerMiembrosActivosConSusPreferenciasYCondiciones(grupoId.Value);
                return new UsuarioPreferencias { Gustos = gustosGrupo,
                    CondicionesMedicas= obtenerPrefDeMiembrosValidos.Result.CondicionesMedicas,
                    Restricciones= obtenerPrefDeMiembrosValidos.Result.Restricciones};
            }

            // Preferencias del usuario base
            var prefsUser = await _obtenerGustosUser.HandleAsync(firebaseUid, ct, gustosDelFiltro);

            if (!string.IsNullOrWhiteSpace(amigoUsername))
            {
                //aca trae los gustos y restricciones 
                var amigo = await _usuarioRepo.GetByUsernameAsync(amigoUsername, ct);
                if (amigo != null)
                {
                    var usuarioActual = await _obtenerUsuario.HandleAsync(FirebaseUid: firebaseUid, ct: ct);

                    var amistad = await _confirmarAmistad
                        .HandleAsync(usuarioActual.Id , amigo.Id, ct);

                    if (amistad == null)
                        throw new UnauthorizedAccessException("No hay amistad entre los usuarios.");
                    {
                        var prefsAmigo = await _obtenerGustosUser
                            .HandleAsync(amigo.FirebaseUid, ct, null);

                        var gustosCombinados = prefsUser.Gustos
                            .Concat(prefsAmigo.Gustos)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList();

                        var restriccionesCombinadas = prefsUser.Restricciones
                            .Concat(prefsAmigo.Restricciones)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList();

                        var condicionesCombinadas = prefsUser.CondicionesMedicas
                            .Concat(prefsAmigo.CondicionesMedicas)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList();


                        return new UsuarioPreferencias { Gustos = gustosCombinados,
                                                        Restricciones= restriccionesCombinadas,
                                                        CondicionesMedicas= condicionesCombinadas};
                    }
                }
            }

            return prefsUser;
        }
    }


}
