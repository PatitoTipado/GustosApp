using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.RestriccionesUseCases;
using GustosApp.Application.UseCases.VotacionUseCases;
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
    public class ObtenerRestriccionesUseCaseTest
    {
        [Fact]
        public async Task HandleAsync_DebeRetornarTodasLasRestriccionesSiUsuarioExiste()
        {
            const string testUid = "firebase-user-123";
            var ct = CancellationToken.None;

            var usuario = new Usuario { FirebaseUid = testUid };
            var restriccionesEsperadas = new List<Restriccion>
            {
                new Restriccion { Id = Guid.NewGuid(), Nombre = "Sin Lactosa" },
                new Restriccion { Id = Guid.NewGuid(), Nombre = "Vegetariano" }
            };

            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockRestriccionRepo = new Mock<IRestriccionRepository>();

            mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(testUid,ct))
                .ReturnsAsync(usuario);

            mockRestriccionRepo.Setup(r => r.GetAllAsync(ct))
                .ReturnsAsync(restriccionesEsperadas);

            var casoDeUso = new ObtenerRestriccionesUseCase(mockRestriccionRepo.Object, mockUsuarioRepo.Object);
            var resultado = await casoDeUso.HandleAsync(testUid, ct);

            Assert.Equal(restriccionesEsperadas.Count, resultado.Count);
            Assert.True(resultado.SequenceEqual(restriccionesEsperadas));
            
            // Verificamos que ambos respositorios fueron llamados 1 vez
            mockUsuarioRepo.Verify(r => r.GetByFirebaseUidAsync(testUid,ct),Times.Once());
            mockRestriccionRepo.Verify(r => r.GetAllAsync(ct), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_DebeLanzarExcepcionSiUsuarioNoEncontrado()
        {
            const string testUid = "uid-inexistente";
            var ct = CancellationToken.None;

            var mockUsuarioRepo = new Mock<IUsuarioRepository>();
            var mockRestriccionRepo = new Mock<IRestriccionRepository>();

            mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(testUid, ct))
                           .ReturnsAsync((Usuario)null!); 

            var casoDeUso = new ObtenerRestriccionesUseCase(mockRestriccionRepo.Object, mockUsuarioRepo.Object);

            var excepcion = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                casoDeUso.HandleAsync(testUid, ct)
            );

            Assert.Equal("Usuario no encontrado.", excepcion.Message);
            mockRestriccionRepo.Verify(r => r.GetAllAsync(ct), Times.Never);
        }
    }
}
