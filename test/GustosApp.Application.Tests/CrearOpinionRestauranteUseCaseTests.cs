using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.RestauranteUseCases.OpinionesRestaurantes;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class CrearOpinionRestauranteUseCaseTests
    {
        private readonly Mock<IFileStorageService> _fileStorage;
        private readonly Mock<IOpinionRestauranteRepository> _repoOpinionRest;
        private readonly Mock<IUsuarioRepository> _repoUsuario;

        private readonly CrearOpinionRestauranteUseCase _useCase;

        public CrearOpinionRestauranteUseCaseTests()
        {
            _fileStorage = new Mock<IFileStorageService>();
            _repoOpinionRest = new Mock<IOpinionRestauranteRepository>();
            _repoUsuario = new Mock<IUsuarioRepository>();

            _useCase = new CrearOpinionRestauranteUseCase(
                _fileStorage.Object,
                _repoOpinionRest.Object,
                _repoUsuario.Object
            );
        }

 
        private FileUpload FakeFile(string name)
            => new FileUpload
            {
                FileName = name,
                Content = new MemoryStream(new byte[] { 1, 2, 3 }) // STREAM
            };

        private Usuario FakeUsuario(Guid id)
            => new Usuario
            {
                Id = id,
                FirebaseUid = "uid123",
                Email = "test@test.com",
                Nombre = "Gonza"
            };

   

        [Fact]
        public async Task HandleAsync_UsuarioInexistente_DeberiaLanzarUnauthorizedAccess()
        {
            _repoUsuario.Setup(r => r.GetByFirebaseUidAsync("uid", default))
                .ReturnsAsync((Usuario?)null);

            Func<Task> act = () => _useCase.HandleAsync(
                "uid",
                Guid.NewGuid(),
                5, "opinion", "titulo",
                new List<FileUpload> { FakeFile("img") },
                "Cena",
                DateTime.UtcNow,
                default
            );

            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Usuario no encontrado");
        }

    
        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public async Task HandleAsync_ValoracionInvalida_DeberiaLanzarArgumentException(double val)
        {
            var user = FakeUsuario(Guid.NewGuid());
            _repoUsuario.Setup(r => r.GetByFirebaseUidAsync("uid", default)).ReturnsAsync(user);

            Func<Task> act = () => _useCase.HandleAsync(
                "uid", Guid.NewGuid(),
                val, "op", "titulo",
                new List<FileUpload> { FakeFile("img") },
                null, DateTime.UtcNow, default
            );

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("La valoracion debe estar entre 1 y 5");
        }

    
        [Fact]
        public async Task HandleAsync_ValoracionDuplicada_DeberiaLanzarArgumentException()
        {
            var user = FakeUsuario(Guid.NewGuid());

            _repoUsuario.Setup(r => r.GetByFirebaseUidAsync("uid", default)).ReturnsAsync(user);
            _repoOpinionRest.Setup(r => r.ExisteValoracionAsync(user.Id, It.IsAny<Guid>(), default))
                .ReturnsAsync(true);

            Func<Task> act = () => _useCase.HandleAsync(
                "uid",
                Guid.NewGuid(),
                4, "op", "titulo",
                new List<FileUpload> { FakeFile("img") },
                null, DateTime.UtcNow,
                default
            );

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("El usuario ya valoro a este restaurante");
        }

        [Fact]
        public async Task HandleAsync_SinImagenes_DeberiaLanzarArgumentException()
        {
            var user = FakeUsuario(Guid.NewGuid());
            _repoUsuario.Setup(r => r.GetByFirebaseUidAsync("uid", default)).ReturnsAsync(user);
            _repoOpinionRest.Setup(r => r.ExisteValoracionAsync(user.Id, It.IsAny<Guid>(), default))
                .ReturnsAsync(false);

            Func<Task> act = () => _useCase.HandleAsync(
                "uid",
                Guid.NewGuid(),
                4, "opinion", "titulo",
                new List<FileUpload>(), // SIN IMÁGENES
                null,
                DateTime.UtcNow,
                default
            );

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Debes subir al menos una imagen para crear una opinión.");

            _fileStorage.Verify(f => f.UploadFileAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
                Times.Never);

            _repoOpinionRest.Verify(r =>
                r.CrearAsync(It.IsAny<OpinionRestaurante>(), default),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_ConImagenes_DeberiaCrearOpinion()
        {
            var user = FakeUsuario(Guid.NewGuid());
            var restId = Guid.NewGuid();
            var fecha = DateTime.UtcNow;

            _repoUsuario.Setup(r => r.GetByFirebaseUidAsync("uid", default))
                .ReturnsAsync(user);

            _repoOpinionRest.Setup(r =>
                r.ExisteValoracionAsync(user.Id, restId, default))
                .ReturnsAsync(false);

            _fileStorage.Setup(f =>
                    f.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), "opiniones"))
                .ReturnsAsync("url_subida");

            OpinionRestaurante? guardada = null;

            _repoOpinionRest.Setup(r =>
                    r.CrearAsync(It.IsAny<OpinionRestaurante>(), default))
                .Callback<OpinionRestaurante, CancellationToken>((o, _) => guardada = o)
                .Returns(Task.CompletedTask);

            await _useCase.HandleAsync(
                "uid",
                restId,
                5,
                "Excelente comida",
                "Gran experiencia",
                new List<FileUpload> { FakeFile("foto1") },
                "Cena Familiar",
                fecha,
                default
            );

            // ASSERTS ------------------------------
            guardada.Should().NotBeNull();
            guardada!.RestauranteId.Should().Be(restId);
            guardada.Valoracion.Should().Be(5);
            guardada.Opinion.Should().Be("Excelente comida");
            guardada.Titulo.Should().Be("Gran experiencia");
            guardada.MotivoVisita.Should().Be("Cena Familiar");
            guardada.FechaVisita.Should().Be(fecha);
            guardada.FechaCreacion.Should().Be(fecha);
            guardada.EsImportada.Should().BeFalse();

            guardada.Fotos.Should().HaveCount(1);
            guardada.Fotos[0].Url.Should().Be("url_subida");

            _fileStorage.Verify(f =>
                f.UploadFileAsync(It.IsAny<Stream>(), "foto1", "opiniones"),
                Times.Once);

            _repoOpinionRest.Verify(r =>
                r.CrearAsync(It.IsAny<OpinionRestaurante>(), default),
                Times.Once);
        }
    }
    }
