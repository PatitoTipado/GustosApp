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
    public class ObtenerChatGrupoUseCaseTests
    {
        private readonly Mock<IChatRepository> _chatRepo;
        private readonly Mock<IGrupoRepository> _grupoRepo;

        private readonly ObtenerChatGrupoUseCase _sut;

        public ObtenerChatGrupoUseCaseTests()
        {
            _chatRepo = new Mock<IChatRepository>();
            _grupoRepo = new Mock<IGrupoRepository>();

            _sut = new ObtenerChatGrupoUseCase(
                _chatRepo.Object,
                _grupoRepo.Object
            );
        }

        // Helpers para crear objetos
        private Usuario CrearUsuario(string firebaseUid)
        {
            return new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = firebaseUid,
                Nombre = "Juan",
                Apellido = "Pérez"
            };
        }

        private MiembroGrupo CrearMiembro(Grupo grupo, Usuario usuario)
        {
            return new MiembroGrupo(grupo.Id, usuario.Id)
            {
                Usuario = usuario 
            };
        }

        private Grupo CrearGrupoConMiembro(string firebaseUid)
        {
            var user = CrearUsuario(firebaseUid);
            var grupo = new Grupo("Grupo Test", user.Id);
            var miembro = CrearMiembro(grupo, user);
            grupo.Miembros = new List<MiembroGrupo> { miembro };
            return grupo;
        }

    
        [Fact]
        public async Task HandleAsync_grupoNoEncontrado_lanzaArgumentException()
        {
            _grupoRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Grupo)null!);

            Func<Task> act = () => _sut.HandleAsync("uid123", Guid.NewGuid());

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Grupo no encontrado");
        }

        [Fact]
        public async Task HandleAsync_usuarioNoEsMiembro_lanzaUnauthorized()
        {
            // Grupo con un miembro distinto
            var grupo = CrearGrupoConMiembro("otroUid");

            _grupoRepo.Setup(r => r.GetByIdAsync(grupo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(grupo);

            Func<Task> act = () => _sut.HandleAsync("uidNoMiembro", grupo.Id);

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("No pertenece a este grupo");
        }

       
        [Fact]
        public async Task HandleAsync_usuarioEsMiembro_retornaMensajes()
        {
            var grupo = CrearGrupoConMiembro("uid123");

            var mensajes = new List<ChatMensaje>
        {
            new ChatMensaje
            {
                Id = Guid.NewGuid(),
                GrupoId = grupo.Id,
                Mensaje = "Hola",
                UsuarioNombre = "Juan Pérez"
            }
        };

            _grupoRepo.Setup(r => r.GetByIdAsync(grupo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(grupo);

            _chatRepo.Setup(r => r.GetMessagesByGrupoIdAsync(grupo.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mensajes);

            var result = await _sut.HandleAsync("uid123", grupo.Id);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Mensaje.Should().Be("Hola");
        }
    }
    }
