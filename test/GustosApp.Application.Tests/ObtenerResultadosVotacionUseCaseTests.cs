using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.VotacionUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class ObtenerResultadosVotacionUseCaseTests
    {
        private readonly Mock<IVotacionRepository> _mockVotacionRepository;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly Mock<IGrupoRepository> _mockGrupoRepository;
        private readonly ObtenerResultadosVotacionUseCase _useCase;

        public ObtenerResultadosVotacionUseCaseTests()
        {
            _mockVotacionRepository = new Mock<IVotacionRepository>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockGrupoRepository = new Mock<IGrupoRepository>();

            _useCase = new ObtenerResultadosVotacionUseCase(
                _mockVotacionRepository.Object,
                _mockUsuarioRepository.Object,
                _mockGrupoRepository.Object);
        }

        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorizedAccessException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId));
        }

        [Fact]
        public async Task HandleAsync_VotacionNoEncontrada_LanzaArgumentException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "testFirebaseUid",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((VotacionGrupo?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId));

            Assert.Equal("Votación no encontrada", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_UsuarioNoEsMiembro_LanzaUnauthorizedAccessException()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "testFirebaseUid",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };
            var votacion = new VotacionGrupo(grupoId);

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _useCase.HandleAsync(firebaseUid, votacionId));

            Assert.Equal("No eres miembro de este grupo", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_SinVotos_RetornaResultadosVacios()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "testFirebaseUid",
                Email = "test@test.com",
                Nombre = "Test",
                Apellido = "User"
            };
            var grupo = new Grupo("Test Grupo", adminId) { Id = grupoId };
            
            var miembro = new MiembroGrupo(grupoId, usuario.Id, false);
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro, true);
            typeof(MiembroGrupo).GetProperty("Activo")!.SetValue(miembro, true);
            grupo.Miembros.Add(miembro);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(votacion.Id, resultado.VotacionId);
            Assert.Equal(grupoId, resultado.GrupoId);
            Assert.Equal(EstadoVotacion.Activa, resultado.Estado);
            Assert.False(resultado.TodosVotaron);
            Assert.Equal(1, resultado.MiembrosActivos);
            Assert.Equal(0, resultado.TotalVotos);
            Assert.Empty(resultado.RestaurantesVotados);
            Assert.Null(resultado.GanadorId);
            Assert.False(resultado.HayEmpate);
        }

        [Fact]
        public async Task HandleAsync_ConVotosYGanadorClaro_RetornaResultadosConGanador()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var restauranteGanadorId = Guid.NewGuid();
            var usuario1 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user1FirebaseUid",
                Email = "user1@test.com",
                Nombre = "User",
                Apellido = "1"
            };
            var usuario2 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user2FirebaseUid",
                Email = "user2@test.com",
                Nombre = "User",
                Apellido = "2"
            };
            var usuario3 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user3FirebaseUid",
                Email = "user3@test.com",
                Nombre = "User",
                Apellido = "3"
            };
            var restaurante1 = new Restaurante
            {
                Id = restauranteGanadorId,
                PlaceId = "ganadorPlaceId",
                Nombre = "Ganador"
            };
            var restaurante2Id = Guid.NewGuid();
            var restaurante2 = new Restaurante
            {
                Id = restaurante2Id,
                PlaceId = "segundoPlaceId",
                Nombre = "Segundo"
            };
            
            var grupo = new Grupo("Test Grupo", adminId) { Id = grupoId };
            
            // Agregar miembros activos
            var miembro1 = new MiembroGrupo(grupoId, usuario1.Id, false);
            var miembro2 = new MiembroGrupo(grupoId, usuario2.Id, false);
            var miembro3 = new MiembroGrupo(grupoId, usuario3.Id, false);
            
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro1, true);
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro2, true);
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro3, true);
            typeof(MiembroGrupo).GetProperty("Activo")!.SetValue(miembro1, true);
            typeof(MiembroGrupo).GetProperty("Activo")!.SetValue(miembro2, true);
            typeof(MiembroGrupo).GetProperty("Activo")!.SetValue(miembro3, true);
            
            grupo.Miembros.Add(miembro1);
            grupo.Miembros.Add(miembro2);
            grupo.Miembros.Add(miembro3);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            
            // Agregar votos: 2 para ganador, 1 para segundo
            var voto1 = new VotoRestaurante(votacionId, usuario1.Id, restauranteGanadorId, "Me gusta") 
            { 
                Restaurante = restaurante1,
                Usuario = usuario1 
            };
            var voto2 = new VotoRestaurante(votacionId, usuario2.Id, restauranteGanadorId, null) 
            { 
                Restaurante = restaurante1,
                Usuario = usuario2 
            };
            var voto3 = new VotoRestaurante(votacionId, usuario3.Id, restaurante2Id, null) 
            { 
                Restaurante = restaurante2,
                Usuario = usuario3 
            };
            
            votacion.Votos.Add(voto1);
            votacion.Votos.Add(voto2);
            votacion.Votos.Add(voto3);

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario1);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.TodosVotaron);
            Assert.Equal(3, resultado.MiembrosActivos);
            Assert.Equal(3, resultado.TotalVotos);
            Assert.Equal(2, resultado.RestaurantesVotados.Count);
            Assert.Equal(restauranteGanadorId, resultado.GanadorId);
            Assert.False(resultado.HayEmpate);
            
            var restauranteGanadorResult = resultado.RestaurantesVotados.First(r => r.RestauranteId == restauranteGanadorId);
            Assert.Equal(2, restauranteGanadorResult.CantidadVotos);
            Assert.Equal(2, restauranteGanadorResult.Votantes.Count);
        }

        [Fact]
        public async Task HandleAsync_ConEmpate_RetornaResultadosConEmpate()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var restaurante1Id = Guid.NewGuid();
            var restaurante2Id = Guid.NewGuid();
            var usuario1 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user1FirebaseUid",
                Email = "user1@test.com",
                Nombre = "User",
                Apellido = "1"
            };
            var usuario2 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user2FirebaseUid",
                Email = "user2@test.com",
                Nombre = "User",
                Apellido = "2"
            };
            var restaurante1 = new Restaurante
            {
                Id = restaurante1Id,
                PlaceId = "rest1PlaceId",
                Nombre = "Rest 1"
            };
            var restaurante2 = new Restaurante
            {
                Id = restaurante2Id,
                PlaceId = "rest2PlaceId",
                Nombre = "Rest 2"
            };
            
            var grupo = new Grupo("Test Grupo", adminId) { Id = grupoId };
            
            var miembro1 = new MiembroGrupo(grupoId, usuario1.Id, false);
            var miembro2 = new MiembroGrupo(grupoId, usuario2.Id, false);
            
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro1, true);
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro2, true);
            typeof(MiembroGrupo).GetProperty("Activo")!.SetValue(miembro1, true);
            typeof(MiembroGrupo).GetProperty("Activo")!.SetValue(miembro2, true);
            
            grupo.Miembros.Add(miembro1);
            grupo.Miembros.Add(miembro2);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            
            // Crear empate
            var voto1 = new VotoRestaurante(votacionId, usuario1.Id, restaurante1Id, null) 
            { 
                Restaurante = restaurante1,
                Usuario = usuario1 
            };
            var voto2 = new VotoRestaurante(votacionId, usuario2.Id, restaurante2Id, null) 
            { 
                Restaurante = restaurante2,
                Usuario = usuario2 
            };
            
            votacion.Votos.Add(voto1);
            votacion.Votos.Add(voto2);

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario1);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.TodosVotaron);
            Assert.Null(resultado.GanadorId);
            Assert.True(resultado.HayEmpate);
            Assert.Equal(2, resultado.RestaurantesEmpatados.Count);
            Assert.Contains(restaurante1Id, resultado.RestaurantesEmpatados);
            Assert.Contains(restaurante2Id, resultado.RestaurantesEmpatados);
        }

        [Fact]
        public async Task HandleAsync_ConGanadorRuleta_RetornaGanadorRuleta()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var restauranteGanadorId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "userFirebaseUid",
                Email = "user@test.com",
                Nombre = "User",
                Apellido = "Test"
            };
            
            var grupo = new Grupo("Test Grupo", adminId) { Id = grupoId };
            var miembro = new MiembroGrupo(grupoId, usuario.Id, false);
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro, true);
            typeof(MiembroGrupo).GetProperty("Activo")!.SetValue(miembro, true);
            grupo.Miembros.Add(miembro);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            votacion.EstablecerGanadorRuleta(restauranteGanadorId);

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(restauranteGanadorId, resultado.GanadorId);
        }

        [Fact]
        public async Task HandleAsync_MiembrosInactivos_NoLosCuentaComoActivos()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var usuario1 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user1FirebaseUid",
                Email = "user1@test.com",
                Nombre = "User",
                Apellido = "1"
            };
            var usuario2 = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "user2FirebaseUid",
                Email = "user2@test.com",
                Nombre = "User",
                Apellido = "2"
            };
            
            var grupo = new Grupo("Test Grupo", adminId) { Id = grupoId };
            
            // Un miembro activo, uno inactivo
            var miembro1 = new MiembroGrupo(grupoId, usuario1.Id, false);
            var miembro2 = new MiembroGrupo(grupoId, usuario2.Id, false);
            
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro1, true);
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro2, true);
            typeof(MiembroGrupo).GetProperty("Activo")!.SetValue(miembro1, true);
            typeof(MiembroGrupo).GetProperty("Activo")!.SetValue(miembro2, false);
            
            grupo.Miembros.Add(miembro1);
            grupo.Miembros.Add(miembro2);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario1);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.MiembrosActivos);
        }

        [Fact]
        public async Task HandleAsync_ConGanadorPorRuleta_RetornaGanadorRuleta()
        {
            // Arrange
            var firebaseUid = "firebase123";
            var votacionId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var restaurante1Id = Guid.NewGuid();
            var restaurante2Id = Guid.NewGuid();
            var ganadorRuletaId = restaurante2Id;
            var usuario1Id = Guid.NewGuid();
            var usuario2Id = Guid.NewGuid();

            var usuario1 = new Usuario
            {
                Id = usuario1Id,
                FirebaseUid = "user1FirebaseUid",
                Email = "user1@test.com",
                Nombre = "User",
                Apellido = "1"
            };
            
            var grupo = new Grupo("Test Grupo", Guid.NewGuid()) { Id = grupoId };
            var miembro1 = new MiembroGrupo(grupoId, usuario1Id, false);
            var miembro2 = new MiembroGrupo(grupoId, usuario2Id, false);
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro1, true);
            typeof(MiembroGrupo).GetProperty("afectarRecomendacion")!.SetValue(miembro2, true);
            grupo.Miembros.Add(miembro1);
            grupo.Miembros.Add(miembro2);
            
            var votacion = new VotacionGrupo(grupoId) { Grupo = grupo };
            typeof(VotacionGrupo).GetProperty("Id")!.SetValue(votacion, votacionId);
            
            // Empate en votos
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario1Id, restaurante1Id, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario2Id, restaurante2Id, null));
            
            // Establecer ganador por ruleta
            votacion.EstablecerGanadorRuleta(ganadorRuletaId);

            _mockUsuarioRepository
                .Setup(x => x.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario1);

            _mockVotacionRepository
                .Setup(x => x.ObtenerPorIdAsync(votacionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(votacion);

            _mockGrupoRepository
                .Setup(x => x.UsuarioEsMiembroAsync(grupoId, firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _useCase.HandleAsync(firebaseUid, votacionId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(ganadorRuletaId, resultado.GanadorId);
            Assert.False(resultado.HayEmpate);
        }
    }
}
