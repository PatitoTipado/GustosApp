using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.UseCases.GrupoUseCases.ChatGrupoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class EnviarMensajeGrupoUseCaseTests
    {
        private readonly Mock<IChatRepository> _chatRepo;
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<IGrupoRepository> _grupoRepo;

        private readonly EnviarMensajeGrupoUseCase _sut;

        public EnviarMensajeGrupoUseCaseTests()
        {
            _chatRepo = new Mock<IChatRepository>();
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _grupoRepo = new Mock<IGrupoRepository>();

            _sut = new EnviarMensajeGrupoUseCase(
                _chatRepo.Object,
                _usuarioRepo.Object,
                _grupoRepo.Object
            );
        }

        private Usuario CrearUsuario(string firebaseUid = "uid123")
        {
            return new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = firebaseUid,
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "test@test.com",
                IdUsuario = Guid.NewGuid().ToString()
            };
        }

        private Grupo CrearGrupo(Guid adminId)
        {
            return new Grupo("Grupo Test", adminId)
            {
                Miembros = new List<MiembroGrupo>()
            };
        }

     

      
        [Fact]
        public async Task HandleAsync_usuarioNoEncontrado_lanzaUnauthorized()
        {
            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);

            Func<Task> act = () => _sut.HandleAsync("uid", Guid.NewGuid(), "Hola");

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Usuario no encontrado");
        }

       
        [Fact]
        public async Task HandleAsync_grupoNoEncontrado_lanzaArgumentException()
        {
            var usuario = CrearUsuario("uid");

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _grupoRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Grupo)null!);

            Func<Task> act = () => _sut.HandleAsync("uid", Guid.NewGuid(), "Hola");

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("El grupo no existe.");
        }

       
        [Fact]
        public async Task HandleAsync_usuarioNoEsMiembro_lanzaUnauthorized()
        {
            var usuario = CrearUsuario("uid");
            var grupo = CrearGrupo(usuario.Id);

            // OJO: el grupo no tiene miembros
            grupo.Miembros = new List<MiembroGrupo>();

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _grupoRepo.Setup(r => r.GetByIdAsync(grupo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(grupo);

            Func<Task> act = () => _sut.HandleAsync("uid", grupo.Id, "Hola");

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("No pertenecés a este grupo.");
        }

     
        [Fact]
        public async Task HandleAsync_mensajeVacio_lanzaArgumentException()
        {
            var usuario = CrearUsuario("uid");
            var grupo = CrearGrupo(usuario.Id);

            // agregar usuario como miembro
            grupo.Miembros.Add(new MiembroGrupo(grupo.Id, usuario.Id));

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _grupoRepo.Setup(r => r.GetByIdAsync(grupo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(grupo);

            Func<Task> act = () => _sut.HandleAsync("uid", grupo.Id, "   ");

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("El mensaje no puede estar vacío.");
        }

        
        [Fact]
        public async Task HandleAsync_ok_guardaYDevuelveMensaje()
        {
            var usuario = CrearUsuario("uid");
            var grupo = CrearGrupo(usuario.Id);

            // el usuario es miembro
            grupo.Miembros.Add(new MiembroGrupo(grupo.Id, usuario.Id));

            _usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _grupoRepo.Setup(r => r.GetByIdAsync(grupo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(grupo);

            ChatMensaje saved = null!;

            _chatRepo.Setup(r => r.AddMessageAsync(It.IsAny<ChatMensaje>(), It.IsAny<CancellationToken>()))
       .ReturnsAsync((ChatMensaje m, CancellationToken _) =>
       {
           saved = m;
           return m;
       });

            var result = await _sut.HandleAsync("uid", grupo.Id, "   Hola mundo   ");

            // Validaciones:
            result.Should().NotBeNull();
            result.Mensaje.Should().Be("Hola mundo"); // trim aplicado
            result.UsuarioId.Should().Be(usuario.Id);
            result.GrupoId.Should().Be(grupo.Id);
            result.UsuarioNombre.Should().Be("Juan Pérez");
            result.Id.Should().NotBe(Guid.Empty);

            // Fecha razonable
            result.FechaEnvio.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));

            // Verificar que efectivamente se llamó al repo con el mensaje generado
            _chatRepo.Verify(r => r.AddMessageAsync(It.IsAny<ChatMensaje>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
