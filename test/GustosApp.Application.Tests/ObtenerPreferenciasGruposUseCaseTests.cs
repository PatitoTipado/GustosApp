using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using Moq;
using Xunit;


namespace GustosApp.Application.Tests
{
    public class ObtenerPreferenciasGruposUseCaseTests
    {
        private readonly Mock<IGustosGrupoRepository> _gustosGrupoRepositoryMock;
        private readonly ObtenerPreferenciasGruposUseCase _sut;

        public ObtenerPreferenciasGruposUseCaseTests()
        {
            _gustosGrupoRepositoryMock = new Mock<IGustosGrupoRepository>();
            _sut = new ObtenerPreferenciasGruposUseCase(_gustosGrupoRepositoryMock.Object);
        }

        // Verifica que se devuelven las preferencias con los gustos que retorna el repositorio
        [Fact]
        public async Task HandleAsync_GrupoValido_DevuelveUsuarioPreferenciasConGustos()
        {
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var gustosRepo = new List<string> { "Pizza", "Sushi", "Empanadas" };

            _gustosGrupoRepositoryMock
                .Setup(r => r.ObtenerGustosDelGrupo(grupoId))
                .ReturnsAsync(gustosRepo);

            var resultado = await _sut.HandleAsync(grupoId, ct);

            Assert.NotNull(resultado);
            Assert.IsType<UsuarioPreferencias>(resultado);
            Assert.NotNull(resultado.Gustos);

            var lista = resultado.Gustos.ToList();
            Assert.Equal(gustosRepo.Count, lista.Count);
            Assert.Equal(gustosRepo[0], lista[0]);
            Assert.Equal(gustosRepo[1], lista[1]);
            Assert.Equal(gustosRepo[2], lista[2]);

            _gustosGrupoRepositoryMock.Verify(
                r => r.ObtenerGustosDelGrupo(grupoId),
                Times.Once);
        }

        // Verifica que si el repositorio devuelve una lista vacia, las preferencias contienen una lista vacía
        [Fact]
        public async Task HandleAsync_SinGustosEnRepositorio_DevuelveListaVacia()
        {
            var grupoId = Guid.NewGuid();
            var ct = CancellationToken.None;

            var gustosRepo = new List<string>();

            _gustosGrupoRepositoryMock
                .Setup(r => r.ObtenerGustosDelGrupo(grupoId))
                .ReturnsAsync(gustosRepo);

            var resultado = await _sut.HandleAsync(grupoId, ct);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Gustos);
            Assert.Empty(resultado.Gustos);

            _gustosGrupoRepositoryMock.Verify(
                r => r.ObtenerGustosDelGrupo(grupoId),
                Times.Once);
        }
    }
}
