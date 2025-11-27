using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.API.DTO;
using GustosApp.Application.Common.Exceptions;
using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;
using Moq;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class UnirseGrupoUseCaseTests
    {
        private readonly Mock<IGrupoRepository> _grupoRepositoryMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<IMiembroGrupoRepository> _miembroGrupoRepositoryMock;
        private readonly Mock<IGustosGrupoRepository> _gustosGrupoRepositoryMock;
        private readonly UnirseGrupoUseCase _sut;

        public UnirseGrupoUseCaseTests()
        {
            _grupoRepositoryMock = new Mock<IGrupoRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _miembroGrupoRepositoryMock = new Mock<IMiembroGrupoRepository>();
            _gustosGrupoRepositoryMock = new Mock<IGustosGrupoRepository>();

            _sut = new UnirseGrupoUseCase(
                _grupoRepositoryMock.Object,
                _usuarioRepositoryMock.Object,
                _miembroGrupoRepositoryMock.Object,
                _gustosGrupoRepositoryMock.Object);
        }

        // Usuario no encontrado → UnauthorizedAccessException
        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaUnauthorizedAccessException()
        {
            var firebaseUid = "uid-inexistente";
            var codigo = "COD123";
            var ct = CancellationToken.None;

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync((Usuario?)null);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _sut.HandleAsync(firebaseUid, codigo, ct));

            Assert.Equal("Usuario no encontrado", ex.Message);
        }

        // Grupo no encontrado → ArgumentException
        [Fact]
        public async Task HandleAsync_GrupoNoEncontradoPorCodigo_LanzaArgumentException()
        {
            var firebaseUid = "uid-valido";
            var codigo = "COD123";
            var ct = CancellationToken.None;

            var usuario = CrearUsuario(firebaseUid);

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuario);

            _grupoRepositoryMock
                .Setup(r => r.GetByCodigoInvitacionAsync(codigo, ct))
                .ReturnsAsync((Grupo?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(firebaseUid, codigo, ct));

            Assert.Equal("Código de invitación inválido", ex.Message);
        }

        // Código expirado → ArgumentException
        [Fact]
        public async Task HandleAsync_CodigoInvitacionExpirado_LanzaArgumentException()
        {
            var firebaseUid = "uid-valido";
            var ct = CancellationToken.None;
            var usuario = CrearUsuario(firebaseUid);

            var adminId = Guid.NewGuid();
            var grupo = new Grupo("Grupo Test", adminId);

            // FORZAR fecha expirada
            typeof(Grupo).GetProperty(nameof(Grupo.FechaExpiracionCodigo))!
                .SetValue(grupo, DateTime.UtcNow.AddDays(-1));

            var codigo = grupo.CodigoInvitacion!;

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuario);

            _grupoRepositoryMock
                .Setup(r => r.GetByCodigoInvitacionAsync(codigo, ct))
                .ReturnsAsync(grupo);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(firebaseUid, codigo, ct));

            Assert.Equal("El código de invitación ha expirado", ex.Message);
        }

        // Ya es miembro → ArgumentException
        [Fact]
        public async Task HandleAsync_UsuarioYaEsMiembroActivo_LanzaArgumentException()
        {
            var firebaseUid = "uid-valido";
            var ct = CancellationToken.None;
            var usuario = CrearUsuario(firebaseUid);

            var adminId = Guid.NewGuid();
            var grupo = new Grupo("Grupo Test", adminId);
            var codigo = grupo.CodigoInvitacion!;

            _usuarioRepositoryMock
                .Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct))
                .ReturnsAsync(usuario);

            _grupoRepositoryMock
                .Setup(r => r.GetByCodigoInvitacionAsync(codigo, ct))
                .ReturnsAsync(grupo);

            _miembroGrupoRepositoryMock
                .Setup(r => r.UsuarioEsMiembroActivoAsync(grupo.Id, usuario.Id, ct))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.HandleAsync(firebaseUid, codigo, ct));

            Assert.Equal("Ya eres miembro de este grupo", ex.Message);
        }

        // Usuario Free con 3+ grupos → LimiteGruposAlcanzadoException
        [Fact]
        public async Task HandleAsync_UsuarioFreeConTresGrupos_LanzaLimiteGruposAlcanzadoException()
        {
            var firebaseUid = "uid-free";
            var ct = CancellationToken.None;
            var usuario = CrearUsuario(firebaseUid);
            usuario.Plan = PlanUsuario.Free;

            var adminId = Guid.NewGuid();
            var grupo = new Grupo("Grupo Test", adminId);
            var codigo = grupo.CodigoInvitacion!;

            var gruposUsuario = new List<Grupo>
        {
            new Grupo("G1", Guid.NewGuid()),
            new Grupo("G2", Guid.NewGuid()),
            new Grupo("G3", Guid.NewGuid())
        };

            _usuarioRepositoryMock.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct)).ReturnsAsync(usuario);
            _grupoRepositoryMock.Setup(r => r.GetByCodigoInvitacionAsync(codigo, ct)).ReturnsAsync(grupo);
            _miembroGrupoRepositoryMock.Setup(r => r.UsuarioEsMiembroActivoAsync(grupo.Id, usuario.Id, ct)).ReturnsAsync(false);
            _grupoRepositoryMock.Setup(r => r.GetGruposByUsuarioIdAsync(usuario.Id, ct)).ReturnsAsync(gruposUsuario);

            var ex = await Assert.ThrowsAsync<LimiteGruposAlcanzadoException>(() =>
                _sut.HandleAsync(firebaseUid, codigo, ct));

            Assert.Equal("Free", ex.TipoPlan);
            Assert.Equal(3, ex.LimiteActual);
            Assert.Equal(3, ex.GruposActuales);
        }

        // Usuario Premium puede unirse sin validar límite
        [Fact]
        public async Task HandleAsync_UsuarioPremium_NoValidaLimiteGruposYSeUneCorrectamente()
        {
            var firebaseUid = "uid-premium";
            var ct = CancellationToken.None;

            var usuario = CrearUsuario(firebaseUid);
            usuario.Plan = PlanUsuario.Plus;

            usuario.Gustos = new List<Gusto>
        {
            new Gusto { Id = Guid.NewGuid(), Nombre = "Pizzas" },
            new Gusto { Id = Guid.NewGuid(), Nombre = "Sushi" }
        };

            var adminId = Guid.NewGuid();
            var grupo = new Grupo("Grupo Premium", adminId);
            var codigo = grupo.CodigoInvitacion!;

            _usuarioRepositoryMock.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct)).ReturnsAsync(usuario);
            _grupoRepositoryMock.Setup(r => r.GetByCodigoInvitacionAsync(codigo, ct)).ReturnsAsync(grupo);
            _miembroGrupoRepositoryMock.Setup(r => r.UsuarioEsMiembroActivoAsync(grupo.Id, usuario.Id, ct)).ReturnsAsync(false);

            MiembroGrupo? miembroCreado = null;

            _miembroGrupoRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<MiembroGrupo>(), ct))
                .Callback<MiembroGrupo, CancellationToken>((m, _) => miembroCreado = m)
                .ReturnsAsync((MiembroGrupo m, CancellationToken _) => m);

            _gustosGrupoRepositoryMock
                .Setup(r => r.AgregarGustosAlGrupo(grupo.Id, It.IsAny<List<Gusto>>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            var grupoCompleto = new Grupo("Grupo Premium Completo", adminId);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupo.Id, ct))
                .ReturnsAsync(grupoCompleto);

            var resultado = await _sut.HandleAsync(firebaseUid, codigo, ct);

            _grupoRepositoryMock.Verify(r => r.GetGruposByUsuarioIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

            _gustosGrupoRepositoryMock.Verify(
                r => r.AgregarGustosAlGrupo(
                    grupo.Id,
                    It.Is<List<Gusto>>(l => l.SequenceEqual(usuario.Gustos)),
                    It.Is<Guid>(id => miembroCreado!.Id == id)),
                Times.Once);

            Assert.Same(grupoCompleto, resultado);
        }

        // Free con menos de 3 grupos → éxito
        [Fact]
        public async Task HandleAsync_UsuarioFreeConMenosDeTresGrupos_SeUneCorrectamente()
        {
            var firebaseUid = "uid-free";
            var ct = CancellationToken.None;
            var usuario = CrearUsuario(firebaseUid);
            usuario.Plan = PlanUsuario.Free;

            usuario.Gustos = new List<Gusto>
        {
            new Gusto { Id = Guid.NewGuid(), Nombre = "Pizzas" },
            new Gusto { Id = Guid.NewGuid(), Nombre = "Pastas" }
        };

            var adminId = Guid.NewGuid();
            var grupo = new Grupo("Grupo Test", adminId);
            var codigo = grupo.CodigoInvitacion!;

            var gruposUsuario = new List<Grupo>
        {
            new Grupo("G1", Guid.NewGuid()),
            new Grupo("G2", Guid.NewGuid())
        };

            _usuarioRepositoryMock.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct)).ReturnsAsync(usuario);
            _grupoRepositoryMock.Setup(r => r.GetByCodigoInvitacionAsync(codigo, ct)).ReturnsAsync(grupo);
            _miembroGrupoRepositoryMock.Setup(r => r.UsuarioEsMiembroActivoAsync(grupo.Id, usuario.Id, ct)).ReturnsAsync(false);
            _grupoRepositoryMock.Setup(r => r.GetGruposByUsuarioIdAsync(usuario.Id, ct)).ReturnsAsync(gruposUsuario);

            MiembroGrupo? miembroCreado = null;

            _miembroGrupoRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<MiembroGrupo>(), ct))
                .Callback<MiembroGrupo, CancellationToken>((m, _) => miembroCreado = m)
                .ReturnsAsync((MiembroGrupo m, CancellationToken _) => m);

            _gustosGrupoRepositoryMock
                .Setup(r => r.AgregarGustosAlGrupo(grupo.Id, It.IsAny<List<Gusto>>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            var grupoCompleto = new Grupo("Grupo Completo", adminId);

            _grupoRepositoryMock
                .Setup(r => r.GetByIdAsync(grupo.Id, ct))
                .ReturnsAsync(grupoCompleto);

            var resultado = await _sut.HandleAsync(firebaseUid, codigo, ct);

            _miembroGrupoRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<MiembroGrupo>(), ct), Times.Once);

            Assert.Same(grupoCompleto, resultado);
        }

        // GrupoCompleto null → error
        [Fact]
        public async Task HandleAsync_GrupoCompletoNoEncontrado_LanzaInvalidOperationException()
        {
            var firebaseUid = "uid-valido";
            var ct = CancellationToken.None;
            var usuario = CrearUsuario(firebaseUid);

            var adminId = Guid.NewGuid();
            var grupo = new Grupo("Grupo Test", adminId);
            var codigo = grupo.CodigoInvitacion!;

            _usuarioRepositoryMock.Setup(r => r.GetByFirebaseUidAsync(firebaseUid, ct)).ReturnsAsync(usuario);
            _grupoRepositoryMock.Setup(r => r.GetByCodigoInvitacionAsync(codigo, ct)).ReturnsAsync(grupo);
            _miembroGrupoRepositoryMock.Setup(r => r.UsuarioEsMiembroActivoAsync(grupo.Id, usuario.Id, ct)).ReturnsAsync(false);
            _grupoRepositoryMock.Setup(r => r.GetGruposByUsuarioIdAsync(usuario.Id, ct)).ReturnsAsync(Enumerable.Empty<Grupo>());
            _miembroGrupoRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<MiembroGrupo>(), ct))
                                       .ReturnsAsync((MiembroGrupo m, CancellationToken _) => m);

            _gustosGrupoRepositoryMock.Setup(r => r.AgregarGustosAlGrupo(grupo.Id, It.IsAny<List<Gusto>>(), It.IsAny<Guid>()))
                                      .ReturnsAsync(true);

            _grupoRepositoryMock.Setup(r => r.GetByIdAsync(grupo.Id, ct))
                .ReturnsAsync((Grupo?)null);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.HandleAsync(firebaseUid, codigo, ct));

            Assert.Equal("Error al unirse al grupo", ex.Message);
        }

        private static Usuario CrearUsuario(string firebaseUid)
        {
            return new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = firebaseUid,
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan@example.com",
                Plan = PlanUsuario.Free,
                Gustos = new List<Gusto>()
            };
        }
    }
    }
