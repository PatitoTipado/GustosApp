using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class RegistrarUsuarioUseCaseTests
    {

        private readonly Mock<IUsuarioRepository> _mockRepo;
        private readonly RegistrarUsuarioUseCase _useCase;

        public RegistrarUsuarioUseCaseTests()
        {
            _mockRepo = new Mock<IUsuarioRepository>();
            _useCase = new RegistrarUsuarioUseCase(_mockRepo.Object);
        }

        [Fact]
        public async Task HandleAsync_UsuarioNoExiste_CreaUsuario()
        {
            // Arrange
            var firebaseUid = "uid-test";
            var request = new RegistrarUsuarioRequest
            {
                Nombre = "Juan",
                Apellido = "Perez",
                Email = "juan@test.com",
                FotoPerfilUrl = "foto.jpg"
            };

            _mockRepo.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            Usuario? usuarioCreado = null;
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                .Callback<Usuario, CancellationToken>((u, ct) => usuarioCreado = u)
                .Returns(Task.CompletedTask);

            _mockRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var resp = await _useCase.HandleAsync(firebaseUid, request, CancellationToken.None);

            // Assert
            Assert.NotNull(resp);
            Assert.Equal(firebaseUid, resp.FirebaseUid);
            Assert.Equal(request.Email, resp.Email);
            Assert.Equal(request.Nombre, resp.Nombre);
            Assert.Equal(request.Apellido, resp.Apellido);
            Assert.Equal(request.FotoPerfilUrl, resp.FotoPerfilUrl);
            Assert.NotNull(usuarioCreado);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_UsuarioYaExiste_DevuelveUsuarioExistente()
        {
            // Arrange
            var firebaseUid = "uid-existente";
            var usuarioExistente = new Usuario(firebaseUid, "existente@test.com", "Ana", "Lopez", "id123", "foto2.jpg");

            var request = new RegistrarUsuarioRequest
            {
                Nombre = "Ana",
                Apellido = "Lopez",
                Email = "existente@test.com",
                FotoPerfilUrl = "foto2.jpg"
            };

            _mockRepo.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioExistente);

            // Act
            var resp = await _useCase.HandleAsync(firebaseUid, request, CancellationToken.None);

            // Assert
            Assert.NotNull(resp);
            Assert.Equal(usuarioExistente.FirebaseUid, resp.FirebaseUid);
            Assert.Equal(usuarioExistente.Email, resp.Email);
            Assert.Equal(usuarioExistente.Nombre, resp.Nombre);
            Assert.Equal(usuarioExistente.Apellido, resp.Apellido);
            Assert.Equal(usuarioExistente.FotoPerfilUrl, resp.FotoPerfilUrl);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }


}
