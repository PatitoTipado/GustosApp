
namespace GustosApp.Application.Tests
{
    using GustosApp.Application.UseCases.AmistadUseCases;
    using GustosApp.Application.UseCases.UsuarioUseCases;
    using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
    using GustosApp.Domain.Common;
    using GustosApp.Domain.Interfaces;
    using GustosApp.Domain.Model;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class ConstruirPreferenciasUseCaseTests
    {
        private readonly Mock<ObtenerUsuarioUseCase> _mockObtenerUsuario;
        private readonly Mock<ObtenerGustosUseCase> _mockObtenerGustos;
        private readonly Mock<ConfirmarAmistadEntreUsuarios> _mockConfirmarAmistad;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepo;
        private readonly Mock<IGustosGrupoRepository> _mockGustosGrupoRepo;
        private readonly Mock<IMiembroGrupoRepository> _mockMiembroGrupoRepo;

        private readonly ConstruirPreferenciasUseCase _useCase;

        public ConstruirPreferenciasUseCaseTests()
        {
            _mockObtenerUsuario = new Mock<ObtenerUsuarioUseCase>(null);
            _mockObtenerGustos = new Mock<ObtenerGustosUseCase>(null);
            _mockConfirmarAmistad = new Mock<ConfirmarAmistadEntreUsuarios>(null);
            _mockUsuarioRepo = new Mock<IUsuarioRepository>();
            _mockGustosGrupoRepo = new Mock<IGustosGrupoRepository>();
            _mockMiembroGrupoRepo = new Mock<IMiembroGrupoRepository>();

            _useCase = new ConstruirPreferenciasUseCase(
                _mockObtenerUsuario.Object,
                _mockObtenerGustos.Object,
                _mockConfirmarAmistad.Object,
                _mockUsuarioRepo.Object,
                _mockGustosGrupoRepo.Object,
                _mockMiembroGrupoRepo.Object
            );
        }

        private UsuarioPreferencias CrearPrefs(
            List<string>? gustos = null,
            List<string>? restricciones = null,
            List<string>? condiciones = null)
        {
            return new UsuarioPreferencias
            {
                Gustos = gustos ?? new List<string>(),
                Restricciones = restricciones ?? new List<string>(),
                CondicionesMedicas = condiciones ?? new List<string>()
            };
        }

        private Usuario CrearUsuario(Guid? id = null, string? firebaseUid = null, string? username = null)
        {
            return new Usuario
            {
                Id = id ?? Guid.NewGuid(),
                FirebaseUid = firebaseUid ?? "firebase123",
                IdUsuario = username ?? "testuser"
            };
        }

        // ==========================================================
        // 1) CUANDO HAY GRUPO → SE DEVUELVE LO DEL GRUPO
        // ==========================================================
        [Fact]
        public async Task HandleAsync_CuandoHayGrupoId_RetornaPreferenciasDelGrupo()
        {
            // Arrange
            var grupoId = Guid.NewGuid();

            _mockGustosGrupoRepo
                .Setup(r => r.ObtenerGustosDelGrupo(grupoId))
                .ReturnsAsync(new List<string> { "Pizza", "Sushi" });

            _mockMiembroGrupoRepo
                .Setup(r => r.obtenerMiembrosActivosConSusPreferenciasYCondiciones(grupoId))
                .ReturnsAsync(new UsuarioPreferencias
                {
                    Restricciones = new List<string> { "Sin gluten" },
                    CondicionesMedicas = new List<string> { "Hipertensión" }
                });

            // Act
            var result = await _useCase.HandleAsync(
                "uid",
                null,
                grupoId,
                null,
                CancellationToken.None
            );

            // Assert
            Assert.Equal(new List<string> { "Pizza", "Sushi" }, result.Gustos);
            Assert.Equal(new List<string> { "Sin gluten" }, result.Restricciones);
            Assert.Equal(new List<string> { "Hipertensión" }, result.CondicionesMedicas);
        }

        // ==========================================================
        // 2) SIN GRUPO Y SIN AMIGO → PREFERENCIAS DEL USUARIO BASE
        // ==========================================================
        [Fact]
        public async Task HandleAsync_SinGrupoNiAmigo_DevuelvePreferenciasBase()
        {
            // Arrange
            var prefsUser = CrearPrefs(
                new List<string> { "Milanesa" },
                new List<string> { "Vegano" },
                new List<string> { "Diabetes" }
            );

            _mockObtenerGustos
                .Setup(g => g.HandleAsync("uid", It.IsAny<CancellationToken>(), It.IsAny<List<string>>()))
                .ReturnsAsync(prefsUser);

            // Act
            var result = await _useCase.HandleAsync(
                "uid",
                null,
                null,
                null,
                CancellationToken.None
            );

            // Assert
            Assert.Equal(prefsUser.Gustos, result.Gustos);
            Assert.Equal(prefsUser.Restricciones, result.Restricciones);
            Assert.Equal(prefsUser.CondicionesMedicas, result.CondicionesMedicas);
        }

        // ==========================================================
        // 3) SIN GRUPO PERO CON AMIGO + AMISTAD → COMBINA LAS PREFS
        // ==========================================================
        [Fact]
        public async Task HandleAsync_ConAmigoYAmistad_CombinaPreferencias()
        {
            // Arrange
            var usuarioActual = CrearUsuario(id: Guid.NewGuid(), firebaseUid: "uid");
            var amigo = CrearUsuario(id: Guid.NewGuid(), firebaseUid: "amigoUid", username: "pepe");

            var prefsUser = CrearPrefs(
                new List<string> { "Pizza" },
                new List<string> { "Vegano" },
                new List<string> { "Asma" }
            );

            var prefsAmigo = CrearPrefs(
                new List<string> { "Sushi" },
                new List<string> { "Celiaco" },
                new List<string> { "Hipertensión" }
            );

            _mockUsuarioRepo
                .Setup(r => r.GetByUsernameAsync("pepe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(amigo);

            _mockObtenerUsuario
                .Setup(u => u.HandleAsync(
                    "uid",
                    null,
                    null,
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync(usuarioActual);

            _mockConfirmarAmistad
                .Setup(a => a.HandleAsync(usuarioActual.Id, amigo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync( new SolicitudAmistad(Guid.NewGuid(), Guid.NewGuid(), "hola")
);

            _mockObtenerGustos
                .Setup(g => g.HandleAsync("uid", It.IsAny<CancellationToken>(), It.IsAny<List<string>>()))
                .ReturnsAsync(prefsUser);

            _mockObtenerGustos
                .Setup(g => g.HandleAsync("amigoUid", It.IsAny<CancellationToken>(), null))
                .ReturnsAsync(prefsAmigo);

            // Act
            var result = await _useCase.HandleAsync(
                "uid",
                "pepe",
                null,
                null,
                CancellationToken.None
            );

            // Assert
            Assert.Equal(2, result.Gustos.Count);                   // Pizza + Sushi
            Assert.Contains("Pizza", result.Gustos);
            Assert.Contains("Sushi", result.Gustos);

            Assert.Equal(2, result.Restricciones.Count);           // Vegano + Celiaco
            Assert.Equal(2, result.CondicionesMedicas.Count);      // Asma + Hipertensión
        }
    }

}
