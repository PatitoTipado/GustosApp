using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class ActualizarPerfilUsuarioUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _userRepo;
        private readonly Mock<IFileStorageService> _fileStorage;
        private readonly Mock<IFirebaseAuthService> _firebase;

        private readonly ActualizarPerfilUsuarioUseCase _useCase;

        public ActualizarPerfilUsuarioUseCaseTests()
        {
            _userRepo = new Mock<IUsuarioRepository>();
            _fileStorage = new Mock<IFileStorageService>();
            _firebase = new Mock<IFirebaseAuthService>();

            _useCase = new ActualizarPerfilUsuarioUseCase(
                _userRepo.Object,
                _fileStorage.Object,
                _firebase.Object
            );
        }

        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_ThrowException()
        {
            // Arrange
            _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Usuario?)null);

            // Act + Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _useCase.HandleAsync("uid", null, null, null, null, false));
        }

        [Fact]
        public async Task HandleAsync_CambiaEmail_ActualizaFirebaseYUsuario()
        {
            var usuario = new Usuario
            {
                FirebaseUid = "uid",
                Email = "viejo@mail.com"
            };

            _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(usuario);

            // Firebase update email OK
            _firebase.Setup(f => f.UpdateUserEmailAsync("uid", "nuevo@mail.com"))
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _useCase.HandleAsync("uid", "nuevo@mail.com", null, null, null, false);

            // Assert
            Assert.Equal("nuevo@mail.com", usuario.Email);
            _firebase.Verify(f => f.UpdateUserEmailAsync("uid", "nuevo@mail.com"), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ActualizaDatosSimples()
        {
            var usuario = new Usuario
            {
                FirebaseUid = "uid",
                Nombre = "Viejo",
                Apellido = "ApellidoViejo",
                EsPrivado = false
            };

            _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(usuario);

            // Act
            var result = await _useCase.HandleAsync(
                "uid",
                null,
                "NuevoNombre",
                "NuevoApellido",
                null,
                true
            );

            // Assert
            Assert.Equal("NuevoNombre", usuario.Nombre);
            Assert.Equal("NuevoApellido", usuario.Apellido);
            Assert.True(usuario.EsPrivado);
        }
        [Fact]
        public async Task HandleAsync_ReemplazaFoto_BorraAnteriorYSubeNueva()
        {
            var usuario = new Usuario
            {
                FirebaseUid = "uid",
                FotoPerfilUrl = "https://old-photo.jpg"
            };

            _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(usuario);

            // Mock del archivo
            var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var file = new FileUpload{
               
                Content= stream, 
                FileName="nueva.jpg",
                ContentType= "image/jpeg"
                };

            // Mock: Upload devuelve nueva URL
            _fileStorage.Setup(f =>
                f.UploadFileAsync(stream, "nueva.jpg", "perfil"))
            .ReturnsAsync("https://new-photo.jpg");

            // Act
            var result = await _useCase.HandleAsync("uid", null, null, null, file, false);

            // Assert
            _fileStorage.Verify(f => f.DeleteFileAsync("https://old-photo.jpg"), Times.Once);
            _fileStorage.Verify(f => f.UploadFileAsync(stream, "nueva.jpg", "perfil"), Times.Once);
            Assert.Equal("https://new-photo.jpg", usuario.FotoPerfilUrl);
        }

        [Fact]
        public async Task HandleAsync_SubeFoto_SinFotoAnterior_NoDelete()
        {
            var usuario = new Usuario
            {
                FirebaseUid = "uid",
                FotoPerfilUrl = null
            };

            _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(usuario);

            var stream = new MemoryStream(new byte[] { 1 });
            var file = new FileUpload
            {

                Content = stream,
                FileName = "foto.jpg",
                ContentType = "image/jpeg"
            };
         

            _fileStorage.Setup(f =>
                f.UploadFileAsync(stream, "foto.jpg", "perfil"))
            .ReturnsAsync("https://foto-nueva.jpg");

            // Act
            var result = await _useCase.HandleAsync("uid", null, null, null, file, false);

            // Assert
            _fileStorage.Verify(f => f.DeleteFileAsync(It.IsAny<string>()), Times.Never);
            Assert.Equal("https://foto-nueva.jpg", usuario.FotoPerfilUrl);
        }

        [Fact]
        public async Task HandleAsync_FotoNull_NoCambiaLaFoto()
        {
            var usuario = new Usuario
            {
                FirebaseUid = "uid",
                FotoPerfilUrl = "foto-actual.jpg"
            };

            _userRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(usuario);

            // Act
            var result = await _useCase.HandleAsync("uid", null, null, null, null, false);

            // Assert
            Assert.Equal("foto-actual.jpg", usuario.FotoPerfilUrl);
            _fileStorage.Verify(f => f.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

    }
}
