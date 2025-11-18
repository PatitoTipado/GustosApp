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
    public class ConstruirPreferenciasUseCase
    {
        private readonly ObtenerUsuarioUseCase _obtenerUsuario;
        private readonly ObtenerGustosUseCase _obtenerGustosUser;
        private readonly ConfirmarAmistadEntreUsuarios _confirmarAmistad;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IGustosGrupoRepository _gustosGrupoRepo;

        public ConstruirPreferenciasUseCase(
            ObtenerUsuarioUseCase obtenerUsuario,
            ObtenerGustosUseCase obtenerGustosUser,
            ConfirmarAmistadEntreUsuarios confirmarAmistad,
            IUsuarioRepository usuarioRepo,
            IGustosGrupoRepository gustosGrupoRepo)
        {
            _obtenerUsuario = obtenerUsuario;
            _obtenerGustosUser = obtenerGustosUser;
            _confirmarAmistad = confirmarAmistad;
            _usuarioRepo = usuarioRepo;
            _gustosGrupoRepo = gustosGrupoRepo;
        }

        public async Task<UsuarioPreferencias> HandleAsync(
            string firebaseUid,
            string? amigoUsername,
            Guid? grupoId,
            List<string>? gustosDelFiltro,
            CancellationToken ct)
        {

            var usuarioActual = await _obtenerUsuario.HandleAsync(FirebaseUid: firebaseUid, ct:ct);

            // PRIORIDAD 1: preferencias de GRUPO
            if (grupoId.HasValue)
            {
                var gustosGrupo = await _gustosGrupoRepo.ObtenerGustosDelGrupo(grupoId.Value);
                return new UsuarioPreferencias { Gustos = gustosGrupo };
            }

            // Preferencias del usuario base
            var prefsUser = await _obtenerGustosUser.HandleAsync(firebaseUid, ct, gustosDelFiltro);

            // PRIORIDAD 2: preferencias con AMIGO (si viene uno)
            if (!string.IsNullOrWhiteSpace(amigoUsername))
            {
                var amigo = await _usuarioRepo.GetByUsernameAsync(amigoUsername, ct);
                if (amigo != null)
                {
                    // validar amistad
                    var amistad = await _confirmarAmistad
                        .HandleAsync(usuarioActual.Id , amigo.Id, ct);

                    if (amistad == null)
                        throw new UnauthorizedAccessException("No hay amistad entre los usuarios.");
                    {
                        var prefsAmigo = await _obtenerGustosUser
                            .HandleAsync(amigo.FirebaseUid, ct, null);

                        var combinados = prefsUser.Gustos
                            .Concat(prefsAmigo.Gustos)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList();

                        return new UsuarioPreferencias { Gustos = combinados };
                    }
                }
            }

            // Usuario solo
            return prefsUser;
        }
    }


}
