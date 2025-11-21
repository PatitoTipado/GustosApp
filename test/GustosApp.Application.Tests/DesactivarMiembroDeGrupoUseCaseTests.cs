using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class DesactivarMiembroDeGrupoUseCaseTests
    {
        // ---------------------------
        // Mocks compartidos
        // ---------------------------
        private readonly Mock<IGrupoRepository> _mockGrupoRepo = new Mock<IGrupoRepository>();
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepo = new Mock<IUsuarioRepository>();
        private readonly Mock<IMiembroGrupoRepository> _mockMiembroRepo = new Mock<IMiembroGrupoRepository>();


        // ---------------------------
        // Helpers de creación
        // ---------------------------
        private Usuario CrearUsuario(Guid id, string? username = null)
            => new Usuario { Id = id, IdUsuario = username };

        private Grupo CrearGrupo(Guid grupoId, Guid adminId)
            => new Grupo("", adminId) { Id = grupoId };

        private MiembroGrupo CrearMiembroActivo(Guid grupoId, Guid usuarioId)
            => new MiembroGrupo(grupoId, usuarioId) { afectarRecomendacion = true };

        private MiembroGrupo CrearMiembroInactivo(Guid grupoId, Guid usuarioId)
            => new MiembroGrupo(grupoId, usuarioId) { afectarRecomendacion = false };


        // ---------------------------
        // Método que crea la instancia del use case
        // ---------------------------
        private DesactivarMiembroDeGrupoUseCase CrearUseCase()
        {
            return new DesactivarMiembroDeGrupoUseCase(
                _mockGrupoRepo.Object,
                _mockUsuarioRepo.Object,
                _mockMiembroRepo.Object
            );
        }


        // ========================================================================
        // ======================== SETUPS MODULARES ===============================
        // ========================================================================


        private void SetupSolicitante(string firebaseUid, Guid userId)
        {
            _mockUsuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CrearUsuario(userId));
        }

        private void SetupUsuarioADesactivar(Guid usuarioId, string username)
        {
            _mockUsuarioRepo
                .Setup(r => r.GetByIdAsync(usuarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CrearUsuario(usuarioId, username));
        }

        private void SetupGrupo(Guid grupoId, Guid adminId)
        {
            _mockGrupoRepo
                .Setup(r => r.GetByIdAsync(grupoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CrearGrupo(grupoId, adminId));
        }

        private void SetupEsAdmin(Guid grupoId, Guid usuarioId, bool esAdmin)
        {
            _mockGrupoRepo
                .Setup(r => r.UsuarioEsAdministradorAsync(grupoId, usuarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(esAdmin);
        }

        private void SetupMiembroGrupo(Guid grupoId, Guid usuarioId, bool estaActivo)
        {
            var miembro = estaActivo
                ? CrearMiembroActivo(grupoId, usuarioId)
                : CrearMiembroInactivo(grupoId, usuarioId);

            _mockMiembroRepo
                .Setup(r => r.GetByGrupoYUsuarioAsync(grupoId, usuarioId.ToString(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(miembro);
        }

        private void SetupDesactivacionDB(Guid grupoId, Guid usuarioId, bool resultado)
        {
            _mockMiembroRepo
                .Setup(r => r.DesactivarMiembroDeGrupo(grupoId, usuarioId))
                .ReturnsAsync(resultado);
        }


        // ========================================================================
        // ======================== TEST FINAL ====================================
        // ========================================================================

        [Fact]
        public async Task Admin_DesactivaMiembroActivo_DebeRetornarTrueYDesactivar()
        {
            // ARRANGE
            string firebaseUid = "admin_uid";
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var miembroId = Guid.NewGuid();

            SetupSolicitante(firebaseUid, adminId);
            SetupUsuarioADesactivar(miembroId, "Juan Perez");
            SetupGrupo(grupoId, adminId);
            SetupEsAdmin(grupoId, adminId, true);
            SetupMiembroGrupo(grupoId, miembroId, estaActivo: true);
            SetupDesactivacionDB(grupoId, miembroId, resultado: true);

            var useCase = CrearUseCase();

            // ACT
            var resultado = await useCase.Handle(grupoId, miembroId, firebaseUid);

            // ASSERT
            Assert.True(resultado, "El use case debe retornar true si la desactivación fue exitosa.");

            _mockMiembroRepo.Verify(
                r => r.DesactivarMiembroDeGrupo(grupoId, miembroId),
                Times.Once
            );
        }
        [Fact]
        public async Task SolicitanteNoExiste_DebeLanzarUnauthorizedAccessException()
        {
            // ARRANGE
            string firebaseUid = "solicitante_invalido";
            var grupoId = Guid.NewGuid();
            var miembroId = Guid.NewGuid();

            // Simular que el solicitante NO es encontrado
            _mockUsuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            // Configuración mínima para que el flujo avance hasta la validación 1
            SetupUsuarioADesactivar(miembroId, "Miembro");
            SetupGrupo(grupoId, Guid.NewGuid());

            var useCase = CrearUseCase();

            // ACT & ASSERT
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                useCase.Handle(grupoId, miembroId, firebaseUid)
            );

            // Verificar que NINGUNA operación de DB posterior fue llamada
            _mockMiembroRepo.Verify(
                r => r.DesactivarMiembroDeGrupo(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UsuarioADesactivarNoExiste_DebeLanzarArgumentException()
        {
            // ARRANGE
            string firebaseUid = "admin_uid";
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var miembroId = Guid.NewGuid();

            SetupSolicitante(firebaseUid, adminId);
            // Simular que el usuario a desactivar NO es encontrado
            _mockUsuarioRepo
                .Setup(r => r.GetByIdAsync(miembroId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);
            SetupGrupo(grupoId, adminId);

            var useCase = CrearUseCase();

            // ACT & ASSERT
            await Assert.ThrowsAsync<ArgumentException>(() =>
                useCase.Handle(grupoId, miembroId, firebaseUid)
            );
        }

        [Fact]
        public async Task UsuarioADesactivarNoEsMiembro_DebeLanzarInvalidOperationException()
        {
            // ARRANGE
            string firebaseUid = "admin_uid";
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var miembroId = Guid.NewGuid();

            SetupSolicitante(firebaseUid, adminId);
            SetupUsuarioADesactivar(miembroId, "Miembro");
            SetupGrupo(grupoId, adminId);
            SetupEsAdmin(grupoId, adminId, true);

            // Simular que el miembro NO es encontrado en el repositorio de Miembros
            _mockMiembroRepo
                .Setup(r => r.GetByGrupoYUsuarioAsync(grupoId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((MiembroGrupo)null);

            var useCase = CrearUseCase();

            // ACT & ASSERT
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                useCase.Handle(grupoId, miembroId, firebaseUid)
            );

            _mockMiembroRepo.Verify(
                r => r.DesactivarMiembroDeGrupo(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never
            );
        }

        [Fact]
        public async Task MiembroRegular_IntentaDesactivarOtroMiembro_DebeLanzarUnauthorizedAccessException()
        {
            // ARRANGE
            string firebaseUid = "miembro_uid";
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var solicitanteId = Guid.NewGuid(); // Miembro regular
            var miembroADesactivarId = Guid.NewGuid();

            SetupSolicitante(firebaseUid, solicitanteId);
            SetupUsuarioADesactivar(miembroADesactivarId, "Otro Miembro");
            SetupGrupo(grupoId, adminId);
            // Solicitante NO es admin
            SetupEsAdmin(grupoId, solicitanteId, false);
            SetupMiembroGrupo(grupoId, miembroADesactivarId, estaActivo: true);

            var useCase = CrearUseCase();

            // ACT & ASSERT
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                useCase.Handle(grupoId, miembroADesactivarId, firebaseUid)
            );

            _mockMiembroRepo.Verify(
                r => r.DesactivarMiembroDeGrupo(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never
            );
        }
        [Fact]
        public async Task MismoUsuario_SeDesactivaASiMismo_DebeRetornarTrueYDesactivar()
        {
            // ARRANGE
            string firebaseUid = "miembro_uid";
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid(); // Es el solicitante y el que se desactiva

            SetupSolicitante(firebaseUid, usuarioId);
            SetupUsuarioADesactivar(usuarioId, "Usuario A"); // El mismo ID
            SetupGrupo(grupoId, adminId);
            // Solicitante NO es admin
            SetupEsAdmin(grupoId, usuarioId, false);
            SetupMiembroGrupo(grupoId, usuarioId, estaActivo: true);
            SetupDesactivacionDB(grupoId, usuarioId, resultado: true);

            var useCase = CrearUseCase();

            // ACT
            var resultado = await useCase.Handle(grupoId, usuarioId, firebaseUid);

            // ASSERT
            Assert.True(resultado, "El use case debe retornar true si el mismo usuario se desactiva con éxito.");

            _mockMiembroRepo.Verify(
                r => r.DesactivarMiembroDeGrupo(grupoId, usuarioId),
                Times.Once
            );
        }
        [Fact]
        public async Task MiembroYaInactivo_DebeRetornarTrueYNoLlamarADesactivar()
        {
            // ARRANGE
            string firebaseUid = "admin_uid";
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var miembroId = Guid.NewGuid();

            SetupSolicitante(firebaseUid, adminId);
            SetupUsuarioADesactivar(miembroId, "Juan Perez");
            SetupGrupo(grupoId, adminId);
            SetupEsAdmin(grupoId, adminId, true);
            // EL miembro ya está INACTIVO
            SetupMiembroGrupo(grupoId, miembroId, estaActivo: false);

            var useCase = CrearUseCase();

            // ACT
            var resultado = await useCase.Handle(grupoId, miembroId, firebaseUid);

            // ASSERT
            Assert.True(resultado, "Debe retornar true (idempotencia) si el miembro ya está inactivo.");

            // Verificar que la función de desactivación en la DB NUNCA fue llamada
            _mockMiembroRepo.Verify(
                r => r.DesactivarMiembroDeGrupo(grupoId, miembroId),
                Times.Never
            );
        }
    }

}
