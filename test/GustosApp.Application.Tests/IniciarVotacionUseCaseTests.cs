using System;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.VotacionUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class IniciarVotacionUseCaseTests
    {
        private readonly Mock<IVotacionRepository> _mockVotacionRepository;
        private readonly Mock<IGrupoRepository> _mockGrupoRepository;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly IniciarVotacionUseCase _useCase;

        public IniciarVotacionUseCaseTests()
        {
            _mockVotacionRepository = new Mock<IVotacionRepository>();
            _mockGrupoRepository = new Mock<IGrupoRepository>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();

            _useCase = new IniciarVotacionUseCase(
                _mockVotacionRepository.Object,
                _mockGrupoRepository.Object,
                _mockUsuarioRepository.Object);
        }

      
        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "firebase123";
            var grupoId = Guid.NewGuid();

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _useCase.HandleAsync(firebaseUid, grupoId, "desc", new List<Guid> { Guid.NewGuid() }));
        }

        
        [Fact]
        public async Task HandleAsync_UsuarioNoEsMiembro_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "firebase123";
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _useCase.HandleAsync(firebaseUid, grupoId, "desc", new List<Guid> { Guid.NewGuid() }));
        }

       
        [Fact]
        public async Task HandleAsync_VotacionActivaExistente_LanzaInvalidOperationException()
        {
            var firebaseUid = "firebase123";
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };

            var votacionActiva = new VotacionGrupo(grupoId);

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockVotacionRepository
                .Setup(x => x.ObtenerVotacionActivaAsync(grupoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacionActiva);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.HandleAsync(firebaseUid, grupoId, null, new List<Guid> { Guid.NewGuid() }));

            Assert.Equal("Ya existe una votación activa en este grupo", ex.Message);
        }

      
        [Fact]
        public async Task HandleAsync_SinRestaurantesCandidatos_LanzaInvalidOperationException()
        {
            var firebaseUid = "firebase123";
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockVotacionRepository
                .Setup(x => x.ObtenerVotacionActivaAsync(grupoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((VotacionGrupo?)null);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.HandleAsync(firebaseUid, grupoId, "desc", new List<Guid>()));

            Assert.Equal("Debe seleccionar al menos un restaurante candidato.", ex.Message);
        }

       
        [Fact]
        public async Task HandleAsync_Valido_CreaVotacionConDescripcion()
        {
            var firebaseUid = "firebase123";
            var grupoId = Guid.NewGuid();
            var descripcion = "Votación viernes";

            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var candidatos = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockVotacionRepository
                .Setup(x => x.ObtenerVotacionActivaAsync(grupoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((VotacionGrupo?)null);

            _mockVotacionRepository
                .Setup(x => x.CrearVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((VotacionGrupo v, CancellationToken _) => v);

            var result = await _useCase.HandleAsync(firebaseUid, grupoId, descripcion, candidatos);

            Assert.NotNull(result);
            Assert.Equal(grupoId, result.GrupoId);
            Assert.Equal(descripcion, result.Descripcion);
            Assert.Equal(EstadoVotacion.Activa, result.Estado);
            Assert.Equal(candidatos.Count, result.RestaurantesCandidatos.Count);
        }

       
        [Fact]
        public async Task HandleAsync_SinDescripcion_CreaVotacion()
        {
            var firebaseUid = "firebase123";
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario { Id = Guid.NewGuid(), FirebaseUid = firebaseUid };
            var candidatos = new List<Guid> { Guid.NewGuid() };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockVotacionRepository
                .Setup(x => x.ObtenerVotacionActivaAsync(grupoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((VotacionGrupo?)null);

            _mockVotacionRepository
                .Setup(x => x.CrearVotacionAsync(It.IsAny<VotacionGrupo>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((VotacionGrupo v, CancellationToken _) => v);

            var result = await _useCase.HandleAsync(firebaseUid, grupoId, null, candidatos);

            Assert.NotNull(result);
            Assert.Null(result.Descripcion);
            Assert.Single(result.RestaurantesCandidatos);
        }
    }
    }
