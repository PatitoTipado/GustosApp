using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.Application.Tests
{
    public class ActualizarGustosAGrupoUseCaseTests
    {

        private readonly Mock<IGrupoRepository> _mockGrupoRepo;
        private readonly Mock<IGustoRepository> _mockGustoRepo;
        private readonly Mock<IGustosGrupoRepository> _mockGustosGrupoRepo;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepo;
        private readonly Mock<IMiembroGrupoRepository> _mockMiembroGrupoRepo;
        private readonly ActualizarGustosAGrupoUseCase _useCase;

        private readonly Guid _grupoId = Guid.NewGuid();
        private readonly string _firebaseUid = "test_firebase_uid";
        private readonly Guid _usuarioId = Guid.NewGuid();
        private readonly List<string> _gustosInput = new List<string> { "Musica", "Cine" };
        private readonly List<Gusto> _gustosExistentes = new List<Gusto>
    {
        new Gusto { Id = Guid.NewGuid(), Nombre = "Musica" },
        new Gusto { Id = Guid.NewGuid(), Nombre = "Cine" }
    };
        private readonly Usuario _usuarioMiembro = new Usuario { Id = Guid.NewGuid(), FirebaseUid = "test_firebase_uid" };

        public ActualizarGustosAGrupoUseCaseTests()
        {
            _mockGrupoRepo = new Mock<IGrupoRepository>();
            _mockGustoRepo = new Mock<IGustoRepository>();
            _mockGustosGrupoRepo = new Mock<IGustosGrupoRepository>();
            _mockUsuarioRepo = new Mock<IUsuarioRepository>();
            _mockMiembroGrupoRepo = new Mock<IMiembroGrupoRepository>();

            _mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(_firebaseUid, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(_usuarioMiembro);

            _mockGrupoRepo.Setup(r => r.ExistsAsync(_grupoId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(true);

            _mockMiembroGrupoRepo.Setup(r => r.UsuarioEsMiembroActivoAsync(_grupoId, _usuarioMiembro.Id, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(true);

            _mockGustoRepo.Setup(r => r.obtenerGustosPorNombre(_gustosInput))
                          .ReturnsAsync(_gustosExistentes);

            _mockGustosGrupoRepo.Setup(r => r.AgregarGustosAlGrupo(_grupoId, _gustosExistentes, _usuarioMiembro.Id))
                                .ReturnsAsync(true);

            _useCase = new ActualizarGustosAGrupoUseCase(
                _mockGrupoRepo.Object,
                _mockGustoRepo.Object,
                _mockGustosGrupoRepo.Object,
                _mockUsuarioRepo.Object,
                _mockMiembroGrupoRepo.Object);
        }

        [Fact]
        public async Task Handle_DatosValidos_DebeRetornarTrueYLlamarAgregarGustos()
        {
            var result = await _useCase.Handle(_gustosInput, _grupoId, _firebaseUid);

            Assert.True(result);

            _mockUsuarioRepo.Verify(r => r.GetByFirebaseUidAsync(_firebaseUid, It.IsAny<CancellationToken>()), Times.Once);
            _mockGrupoRepo.Verify(r => r.ExistsAsync(_grupoId, It.IsAny<CancellationToken>()), Times.Once);
            _mockMiembroGrupoRepo.Verify(r => r.UsuarioEsMiembroActivoAsync(_grupoId, _usuarioMiembro.Id, It.IsAny<CancellationToken>()), Times.Once);
            _mockGustoRepo.Verify(r => r.obtenerGustosPorNombre(_gustosInput), Times.Once);

            _mockGustosGrupoRepo.Verify(r => r.AgregarGustosAlGrupo(
                _grupoId,
                It.Is<List<Gusto>>(list => list.Count == 2), 
                _usuarioMiembro.Id
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_UsuarioInexistente_DebeLanzarUnauthorizedAccessException()
        {
            _mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(_firebaseUid, It.IsAny<CancellationToken>()))
                            .ReturnsAsync((Usuario)null);

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _useCase.Handle(_gustosInput, _grupoId, _firebaseUid)
            );

            Assert.Contains("El usuario no existe.", exception.Message);

            _mockGrupoRepo.Verify(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockMiembroGrupoRepo.Verify(r => r.UsuarioEsMiembroActivoAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockGustoRepo.Verify(r => r.obtenerGustosPorNombre(It.IsAny<List<string>>()), Times.Never);
            _mockGustosGrupoRepo.Verify(r => r.AgregarGustosAlGrupo(
                It.IsAny<Guid>(),
                It.IsAny<List<Gusto>>(),
                It.IsAny<Guid>()
            ), Times.Never);
        }

        [Fact]
        public async Task Handle_UsuarioExistePeroNoEsMiembroActivo_DebeLanzarUnauthorizedAccessException()
        {
            _mockMiembroGrupoRepo.Setup(r => r.UsuarioEsMiembroActivoAsync(_grupoId, _usuarioMiembro.Id, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(false);

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _useCase.Handle(_gustosInput, _grupoId, _firebaseUid)
            );

            Assert.Contains("El miembro no es un usuario activo (expulsado)", exception.Message);

            _mockUsuarioRepo.Verify(r => r.GetByFirebaseUidAsync(_firebaseUid, It.IsAny<CancellationToken>()), Times.Once);
            _mockGrupoRepo.Verify(r => r.ExistsAsync(_grupoId, It.IsAny<CancellationToken>()), Times.Once); // Se llama antes

            _mockGustoRepo.Verify(r => r.obtenerGustosPorNombre(It.IsAny<List<string>>()), Times.Never);
            _mockGustosGrupoRepo.Verify(r => r.AgregarGustosAlGrupo(
                It.IsAny<Guid>(),
                It.IsAny<List<Gusto>>(),
                It.IsAny<Guid>()
            ), Times.Never);
        }

        [Fact]
        public async Task Handle_GrupoInexistente_DebeLanzarKeyNotFoundException()
        {
            _mockGrupoRepo.Setup(r => r.ExistsAsync(_grupoId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(false);

            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _useCase.Handle(_gustosInput, _grupoId, _firebaseUid)
            );

            Assert.Contains("el grupo no existe.", exception.Message);

            _mockUsuarioRepo.Verify(r => r.GetByFirebaseUidAsync(_firebaseUid, It.IsAny<CancellationToken>()), Times.Once);
            _mockGrupoRepo.Verify(r => r.ExistsAsync(_grupoId, It.IsAny<CancellationToken>()), Times.Once);

            _mockMiembroGrupoRepo.Verify(r => r.UsuarioEsMiembroActivoAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);

            _mockGustoRepo.Verify(r => r.obtenerGustosPorNombre(It.IsAny<List<string>>()), Times.Never);

            _mockGustosGrupoRepo.Verify(r => r.AgregarGustosAlGrupo(
                It.IsAny<Guid>(),
                It.IsAny<List<Gusto>>(),
                It.IsAny<Guid>()
            ), Times.Never);
        }

        [Fact]
        public async Task Handle_GustosInexistentes_DebeLanzarKeyNotFoundException()
        {
            _mockGustoRepo.Setup(r => r.obtenerGustosPorNombre(_gustosInput))
                          .ReturnsAsync(new List<Gusto>());

            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _useCase.Handle(_gustosInput, _grupoId, _firebaseUid)
            );

            Assert.Contains("No existen los gustos mencionados.", exception.Message);
        }
    }
}
