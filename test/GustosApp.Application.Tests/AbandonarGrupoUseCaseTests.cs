using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;
using Moq;

namespace GustosApp.Application.Tests
{

    public class AbandonarGrupoUseCaseTests
    {
        private readonly Mock<IGrupoRepository> _grupoRepo;
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<IMiembroGrupoRepository> _miembroRepo;
        private readonly Mock<IChatRealTimeService> _chat;

        private readonly AbandonarGrupoUseCase _sut;

        public AbandonarGrupoUseCaseTests()
        {
            _grupoRepo = new Mock<IGrupoRepository>();
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _miembroRepo = new Mock<IMiembroGrupoRepository>();
            _chat = new Mock<IChatRealTimeService>();

            _sut = new AbandonarGrupoUseCase(
                _grupoRepo.Object,
                _usuarioRepo.Object,
                _miembroRepo.Object,
                _chat.Object
            );
        }

        // ==== Helpers para no repetir ====

        private Usuario CrearUsuario(string firebaseUid = "uid_test")
        {
            return new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = firebaseUid,
                Email = "test@test.com",
                Nombre = "Nombre",
                Apellido = "Apellido",
                IdUsuario = Guid.NewGuid().ToString(),  
                Rol = RolUsuario.Usuario
            };
        }

        private Grupo CrearGrupo()
        {
            return new Grupo(
                nombre: "Grupo Test",
                administradorId: Guid.NewGuid(),
                descripcion: null
            );
        }

        private MiembroGrupo CrearMiembro(Grupo grupo, bool esAdmin = false, bool activo = true)
        {
            var miembro = new MiembroGrupo(grupo.Id, Guid.NewGuid(), esAdmin);
            if (!activo)
                miembro.AbandonarGrupo();
            return miembro;
        }

        [Fact]
        public async Task HandleAsync_usuarioNoEncontrado_lanzaUnauthorized()
        {
            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);

            Func<Task> act = () => _sut.HandleAsync("uid_inexistente", Guid.NewGuid());

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Usuario no encontrado");
        }

        [Fact]
        public async Task HandleAsync_grupoNoEncontrado_lanzaArgumentException()
        {
            var usuario = CrearUsuario("uid1");

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _grupoRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Grupo)null!);

            Func<Task> act = () => _sut.HandleAsync("uid1", Guid.NewGuid());

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Grupo no encontrado");
        }

  
        [Fact]
        public async Task HandleAsync_usuarioNoEsMiembro_lanzaArgumentException()
        {
            var usuario = CrearUsuario("uid2");
            var grupo = CrearGrupo();

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid2", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _grupoRepo.Setup(r => r.GetByIdAsync(grupo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(grupo);

            _miembroRepo.Setup(r => r.GetByGrupoYUsuarioAsync(grupo.Id, usuario.IdUsuario, It.IsAny<CancellationToken>()))
                .ReturnsAsync((MiembroGrupo)null!);

            Func<Task> act = () => _sut.HandleAsync("uid2", grupo.Id);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("No eres miembro de este grupo");
        }

     
        [Fact]
        public async Task HandleAsync_adminUnico_lanzaInvalidOperationException()
        {
            var usuario = CrearUsuario("uid3");
            var grupo = CrearGrupo();
            var miembro = CrearMiembro(grupo, esAdmin: true, activo: true);

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid3", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _grupoRepo.Setup(r => r.GetByIdAsync(grupo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(grupo);

            _miembroRepo.Setup(r => r.GetByGrupoYUsuarioAsync(grupo.Id, usuario.IdUsuario, It.IsAny<CancellationToken>()))
                .ReturnsAsync(miembro);

            // Solo hay un admin activo → bloquea salida
            _miembroRepo.Setup(r => r.GetMiembrosByGrupoIdAsync(grupo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<MiembroGrupo> { miembro });

            Func<Task> act = () => _sut.HandleAsync("uid3", grupo.Id);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("No puedes abandonar el grupo siendo el único administrador. Promueve a otro miembro a administrador primero.");
        }

   
        [Fact]
        public async Task HandleAsync_casoCorrecto_abandonaYActualiza()
        {
            var usuario = CrearUsuario("uid4");
            var grupo = CrearGrupo();

            // No es admin → no entra en la lógica de admin único
            var miembro = CrearMiembro(grupo, esAdmin: false, activo: true);

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid4", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _grupoRepo.Setup(r => r.GetByIdAsync(grupo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(grupo);

            _miembroRepo.Setup(r => r.GetByGrupoYUsuarioAsync(grupo.Id, usuario.IdUsuario, It.IsAny<CancellationToken>()))
                .ReturnsAsync(miembro);

            _miembroRepo.Setup(r => r.UpdateAsync(miembro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(miembro);

            _chat.Setup(c => c.UsuarioAbandono(
                    grupo.Id,
                    usuario.Id,
                    usuario.IdUsuario,         
                    usuario.FirebaseUid
                ))
                .Returns(Task.CompletedTask);

            var result = await _sut.HandleAsync("uid4", grupo.Id);

            // Estado
            result.Should().BeTrue();
            miembro.Activo.Should().BeFalse();

            // Llamadas
            _miembroRepo.Verify(r => r.UpdateAsync(miembro, It.IsAny<CancellationToken>()), Times.Once);

            _chat.Verify(c => c.UsuarioAbandono(
                grupo.Id,
                usuario.Id,
                usuario.IdUsuario,
                usuario.FirebaseUid
            ), Times.Once);
        }
    }
    }
