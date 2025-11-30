using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class ActualizarNombreGrupoUseCaseTests
    {
        private readonly Mock<IGrupoRepository> _grupoRepo;
        private readonly Mock<IUsuarioRepository> _usuarioRepo;

        private readonly ActualizarNombreGrupoUseCase _sut;

        public ActualizarNombreGrupoUseCaseTests()
        {
            _grupoRepo = new Mock<IGrupoRepository>();
            _usuarioRepo = new Mock<IUsuarioRepository>();

            _sut = new ActualizarNombreGrupoUseCase(
                _grupoRepo.Object,
                _usuarioRepo.Object
            );
        }

        // Helper para no repetir
        private Usuario CrearUsuario(Guid? id = null, string firebase = "uid_test")
        {
            return new Usuario
            {
                Id = id ?? Guid.NewGuid(),
                FirebaseUid = firebase,
                Email = "test@test.com",
                Nombre = "Nombre",
                Apellido = "Apellido",
                IdUsuario = Guid.NewGuid().ToString()
            };
        }

        private Grupo CrearGrupo(Guid administradorId)
        {
            return new Grupo("Nombre original", administradorId);
        }

   
        [Fact]
        public async Task HandleAsync_nombreVacio_lanzaArgumentException()
        {
            Func<Task> act = () => _sut.HandleAsync("uid", Guid.NewGuid(), "");

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("El nombre del grupo no puede estar vacío");
        }

      
        [Fact]
        public async Task HandleAsync_usuarioNoEncontrado_lanzaUnauthorized()
        {
            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);

            Func<Task> act = () => _sut.HandleAsync("uid", Guid.NewGuid(), "Nuevo Nombre");

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Usuario no encontrado");
        }

   
        [Fact]
        public async Task HandleAsync_grupoNoEncontrado_lanzaKeyNotFound()
        {
            var user = CrearUsuario();

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _grupoRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Grupo)null!);

            Func<Task> act = () => _sut.HandleAsync("uid", Guid.NewGuid(), "Nuevo Nombre");

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Grupo no encontrado");
        }

      
        [Fact]
        public async Task HandleAsync_usuarioNoEsAdmin_lanzaUnauthorized()
        {
            var user = CrearUsuario();
            var grupo = CrearGrupo(Guid.NewGuid()); // admin es otro

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _grupoRepo.Setup(r => r.GetByIdAsync(grupo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(grupo);

            Func<Task> act = () => _sut.HandleAsync("uid", grupo.Id, "Nuevo Nombre");

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Solo el administrador puede cambiar el nombre del grupo");
        }

      
        [Fact]
        public async Task HandleAsync_ok_actualizaNombreYLlamaUpdate()
        {
            var user = CrearUsuario();
            var grupo = CrearGrupo(user.Id); // admin es el usuario

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _grupoRepo.Setup(r => r.GetByIdAsync(grupo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(grupo);

            _grupoRepo.Setup(r => r.UpdateAsync(grupo, It.IsAny<CancellationToken>()))
                .ReturnsAsync(grupo);

            var result = await _sut.HandleAsync("uid", grupo.Id, "   Nuevo Nombre   ");

            // Estado del grupo
            result.Nombre.Should().Be("Nuevo Nombre"); // Trim aplicado

            // Llamado a update
            _grupoRepo.Verify(r => r.UpdateAsync(grupo, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
    }
