using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class ObtenerGruposUsuarioUseCaseTests
    {
        private readonly Mock<IGrupoRepository> _grupoRepositoryMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly ObtenerGruposUsuarioUseCase _sut;

        public ObtenerGruposUsuarioUseCaseTests()
        {
            _grupoRepositoryMock = new Mock<IGrupoRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();

            _sut = new ObtenerGruposUsuarioUseCase(
                _grupoRepositoryMock.Object,
                _usuarioRepositoryMock.Object);
        }

        private static Usuario CreateUsuario(Guid? id = null, string firebaseUid = "firebase-uid")
        {
            var type = typeof(Usuario);
            var usuario = (Usuario)Activator.CreateInstance(type, nonPublic: true)!;

            type.GetProperty("Id")?.SetValue(usuario, id ?? Guid.NewGuid());
            type.GetProperty("FirebaseUid")?.SetValue(usuario, firebaseUid);

            return usuario;
        }

        private static Grupo CreateGrupo(Guid? id = null, string nombre = "Grupo prueba")
        {
            var type = typeof(Grupo);
            var grupo = (Grupo)Activator.CreateInstance(type, nonPublic: true)!;

            type.GetProperty("Id")?.SetValue(grupo, id ?? Guid.NewGuid());
            type.GetProperty("Nombre")?.SetValue(grupo, nombre);

            return grupo;
        }

        // Verifica que cuando el usuario no existe se lanza UnauthorizedAccessException con el mensaje correcto
        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "uid-inexistente";
            var ct = CancellationToken.None;

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync((Usuario?)null);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _sut.HandleAsync(firebaseUid, ct));

            Assert.Equal("Usuario no encontrado", ex.Message);

            _grupoRepositoryMock.Verify(
                r => r.GetGruposByUsuarioIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // Verifica que cuando el usuario existe se devuelven los grupos obtenidos del repositorio
        [Fact]
        public async Task HandleAsync_UsuarioValido_DevuelveGruposDelRepositorio()
        {
            var firebaseUid = "uid-valido";
            var ct = CancellationToken.None;

            var usuarioId = Guid.NewGuid();
            var usuario = CreateUsuario(id: usuarioId, firebaseUid: firebaseUid);

            var grupo1 = CreateGrupo(nombre: "Grupo 1");
            var grupo2 = CreateGrupo(nombre: "Grupo 2");
            var gruposRepo = new List<Grupo> { grupo1, grupo2 };

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuario);

            _grupoRepositoryMock
                .Setup(r => r.GetGruposByUsuarioIdAsync(usuarioId, ct))
                .ReturnsAsync(gruposRepo);

            var resultado = await _sut.HandleAsync(firebaseUid, ct);
            var lista = resultado.ToList();

            Assert.Equal(2, lista.Count);
            Assert.Same(grupo1, lista[0]);
            Assert.Same(grupo2, lista[1]);

            _usuarioRepositoryMock.Verify(
                r => r.GetByFirebaseUidAsync(firebaseUid, ct),
                Times.Once);

            _grupoRepositoryMock.Verify(
                r => r.GetGruposByUsuarioIdAsync(usuarioId, ct),
                Times.Once);
        }
    }
}
