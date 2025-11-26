using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
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
    public class ObtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCaseTest
    {

        [Fact]
        public async Task HandleAsync_DebePriorizarGustosDelUsuarioCombinandoConPaginacion()
        {
            const string testUid = "firebase-test-123";
            const int inicio = 0;
            const int final = 10;

            //6 elementos únicos, con C duplicado en la entrada) ---
            var gustoA = new Gusto { Id = Guid.NewGuid(), Nombre = "Picante" };
            var gustoB = new Gusto { Id = Guid.NewGuid(), Nombre = "Mexicano" };
            var gustoC = new Gusto { Id = Guid.NewGuid(), Nombre = "Sushi" };

            var gustosDelUsuario = new List<Gusto> { gustoA, gustoB, gustoC };

            var gustoD = new Gusto { Id = Guid.NewGuid(), Nombre = "Vegano" };
            var gustoE = new Gusto { Id = Guid.NewGuid(), Nombre = "Tailandes" };
            var gustoF = new Gusto { Id = Guid.NewGuid(), Nombre = "Postres" };

            var gustosPaginados = new List<Gusto> { gustoC, gustoD, gustoE, gustoF };

            var usuarioSimulado = new Usuario
            {
                FirebaseUid = testUid,
                Gustos = gustosDelUsuario
            };

            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockGustoRepo = new Mock<IGustoRepository>();

            mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(
                    testUid,
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync(usuarioSimulado);

            mockGustoRepo.Setup(r => r.obtenerGustosPorPaginacion(inicio, final))
                .Returns(gustosPaginados);

            var casoDeUso = new ObtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase(
                mockUsuarioRepo.Object,
                mockGustoRepo.Object
            );

            var resultado = await casoDeUso.HandleAsync(testUid, inicio, final);

            Assert.Equal(6, resultado.Count);

            Assert.Equal(gustoB.Id, resultado[0].Id);
            Assert.Equal(gustoA.Id, resultado[1].Id);
            Assert.Equal(gustoC.Id, resultado[2].Id);

            mockUsuarioRepo.Verify(r => r.GetByFirebaseUidAsync(
                testUid,
                It.IsAny<CancellationToken>()
            ), Times.Once);

            mockGustoRepo.Verify(r => r.obtenerGustosPorPaginacion(inicio, final), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_DebeLanzarExcepcionSiUsuarioNoExiste()
        {
            const string testUid = "uid-inexistente";
            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockGustoRepo = new Mock<IGustoRepository>();

            mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(
                    testUid,
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync((Usuario)null!);

            var casoDeUso = new ObtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase(
                mockUsuarioRepo.Object,
                mockGustoRepo.Object
            );

            var excepcion = await Assert.ThrowsAsync<Exception>(() =>
                casoDeUso.HandleAsync(testUid, 0, 10)
            );

            Assert.Equal("Usuario no encontrado", excepcion.Message);

            mockGustoRepo.Verify(r => r.obtenerGustosPorPaginacion(
                It.IsAny<int>(),
                It.IsAny<int>()
            ), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_DebeRetornarSoloPaginacionSiUsuarioNoTieneGustos()
        {
            const string testUid = "uid-nuevo";
            const int inicio = 0;
            const int final = 10;

            var gustoSushi = new Gusto { Id = Guid.NewGuid(), Nombre = "Sushi" };
            var gustoPizza = new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza" };

            var gustosDevueltosPorMock = new List<Gusto> { gustoSushi, gustoPizza };

            var gustosPaginadosEsperados = new List<Gusto> { gustoPizza, gustoSushi };

            var usuarioSimulado = new Usuario
            {
                FirebaseUid = testUid,
                Gustos = new List<Gusto>()
            };

            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockGustoRepo = new Mock<IGustoRepository>();

            mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(
                testUid,
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(usuarioSimulado);

            // El Mock devuelve la lista de gustos pero no importa el orden ya que el caso de uso lo ordena

            mockGustoRepo.Setup(r => r.obtenerGustosPorPaginacion(inicio, final))
                .Returns(gustosDevueltosPorMock);

            var casoDeUso = new ObtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase(
                mockUsuarioRepo.Object, mockGustoRepo.Object
            );

            var resultado = await casoDeUso.HandleAsync(testUid, inicio, final);

            Assert.Equal(gustosPaginadosEsperados.Count, resultado.Count);

            // Usamos el SequenceEqual y comparar contra la lista que tiene el orden alfabetico (Pizza, Sushi)
            Assert.True(resultado.SequenceEqual(gustosPaginadosEsperados),
                "El resultado debe ser idéntico a la lista ordenada alfabéticamente (Pizza, Sushi)."
            );

            mockUsuarioRepo.Verify(r => r.GetByFirebaseUidAsync(
                testUid,
                It.IsAny<CancellationToken>()
            ), Times.Once);

            mockGustoRepo.Verify(r => r.obtenerGustosPorPaginacion(inicio, final), Times.Once);
        }
    }
}
