using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Tests
{
    public class BuscarUsuariosUseCaseTest
    {
        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorizedAccessException()
        {
            var usuarioRepo = new Mock<IUsuarioRepository>();
            usuarioRepo.Setup(r => r.GetByFirebaseUidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            var casoDeUso = new BuscarUsuariosUseCase(usuarioRepo.Object);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await casoDeUso.HandleAsync("uid_inexistente", null, CancellationToken.None);
            });
        }

        [Fact]
        public async Task HandleAsync_UsuarioEncontrado()
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "uid"
            };

            var listaUsuarios = new List<Usuario>
            {
               new Usuario { Id = Guid.NewGuid(), FirebaseUid = "otro1" },
               new Usuario { Id = Guid.NewGuid(), FirebaseUid = "otro2" }
            };

            var usuarioRepo = new Mock<IUsuarioRepository>();

            usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            usuarioRepo.Setup(r => r.GetAllExceptAsync(usuario.Id, 50, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(listaUsuarios);

            var casoDeUso = new BuscarUsuariosUseCase(usuarioRepo.Object);

            var resultado = await casoDeUso.HandleAsync("uid", null, CancellationToken.None);

            Assert.Equal(2, resultado.Count());

            usuarioRepo.Verify(r => r.GetAllExceptAsync(usuario.Id, 50, It.IsAny<CancellationToken>()),
               Times.Once);
        }

        [Fact]
        public async Task HandleAsync_UsernameMuyCorto_LanzaExcepcion()
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = "uid"
            };

            var usuarioRepo = new Mock<IUsuarioRepository>();

            usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            var casoDeUso = new BuscarUsuariosUseCase(usuarioRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await casoDeUso.HandleAsync("uid", "a", CancellationToken.None);
            });
        }
    }
}
