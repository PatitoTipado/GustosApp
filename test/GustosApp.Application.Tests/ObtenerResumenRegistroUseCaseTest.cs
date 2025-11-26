using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases;
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
    public class ObtenerResumenRegistroUseCaseTest
    {
        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_LanzaException()
        {
            var usuarioRepo = new Mock<IUsuarioRepository>();
            var cache = new Mock<ICacheService>();

            usuarioRepo.Setup(r => r.GetByFirebaseUidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            var casoDeUso = new ObtenerResumenRegistroUseCase(usuarioRepo.Object, cache.Object);

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await casoDeUso.HandleAsync("uid_inexistente", "modo", CancellationToken.None);
            });
        }

        [Fact]
        public async Task HandleAsync_ModoRegistro_AplicaFiltrosDesdeRedis()
        {
            var r1 = Guid.NewGuid();
            var r2 = Guid.NewGuid();
            var c1 = Guid.NewGuid();
            var g1 = Guid.NewGuid();
            var g2 = Guid.NewGuid();

            var usuario = new Usuario
            {
                FirebaseUid = "uid",
                Restricciones = new List<Restriccion>
            {
                new Restriccion { Id = r1, Nombre = "Sin gluten" },
                new Restriccion { Id = r2, Nombre = "Vegano" }
            },
                CondicionesMedicas = new List<CondicionMedica>
            {
                new CondicionMedica { Id = c1, Nombre = "Diabetes" }
            },
                Gustos = new List<Gusto>
            {
                new Gusto { Id = g1, Nombre = "Italiana" },
                new Gusto { Id = g2, Nombre = "Mexicana" }
            }
            };

            var usuarioRepo = new Mock<IUsuarioRepository>();
            var cache = new Mock<ICacheService>();

            usuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            cache.Setup(c => c.GetAsync<List<Guid>>("registro:uid:restricciones"))
                 .ReturnsAsync(new List<Guid> { r1 });

            cache.Setup(c => c.GetAsync<List<Guid>>("registro:uid:condiciones"))
                 .ReturnsAsync(new List<Guid> { c1 });

            cache.Setup(c => c.GetAsync<List<Guid>>("registro:uid:gustos"))
                 .ReturnsAsync(new List<Guid> { g1 });

            var casoDeUso = new ObtenerResumenRegistroUseCase(usuarioRepo.Object, cache.Object);

            var result = await casoDeUso.HandleAsync("uid", "registro", CancellationToken.None);

            Assert.Single(result.Restricciones);
            Assert.Equal(r1, result.Restricciones.First().Id);

            Assert.Single(result.CondicionesMedicas);
            Assert.Equal(c1, result.CondicionesMedicas.First().Id);

            Assert.Single(result.Gustos);
            Assert.Equal(g1, result.Gustos.First().Id);
        }


        [Fact]
        public async Task HandleAsync_ModoRegistro_RedisNull_NoModificaListas()
        {
            var usuario = new Usuario
            {
                FirebaseUid = "uid",
                Restricciones = new List<Restriccion> { new Restriccion { Id = Guid.NewGuid() } },
                CondicionesMedicas = new List<CondicionMedica> { new CondicionMedica { Id = Guid.NewGuid() } },
                Gustos = new List<Gusto> { new Gusto { Id = Guid.NewGuid() } }
            };

            var usuarioRepo = new Mock<IUsuarioRepository>();
            var cache = new Mock<ICacheService>();

            usuarioRepo.Setup(r => r.GetByFirebaseUidAsync("uid", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(usuario);

            cache.Setup(c => c.GetAsync<List<Guid>>(It.IsAny<string>()))
                 .ReturnsAsync((List<Guid>)null);

            var casoDeUso = new ObtenerResumenRegistroUseCase(usuarioRepo.Object,cache.Object);
            var result = await casoDeUso.HandleAsync("uid", "registro", CancellationToken.None);

            Assert.Equal(1, result.Restricciones.Count);
            Assert.Equal(1, result.CondicionesMedicas.Count);
            Assert.Equal(1, result.Gustos.Count);

        }

        [Fact]
        public async Task HandleAsync_ModoEdicion_NoUsaRedis()
        {
            var usuario = new Usuario { FirebaseUid = "uid" };
            var usuarioRepo = new Mock<IUsuarioRepository>();
            var cache = new Mock<ICacheService>();

            usuarioRepo
                .Setup(r => r.GetByFirebaseUidAsync("uid",It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            var casoDeUso = new ObtenerResumenRegistroUseCase(usuarioRepo.Object, cache.Object);

            var result = await casoDeUso.HandleAsync("uid", "edicion", CancellationToken.None);

            cache.Verify(c => c.GetAsync <List<Guid>>(It.IsAny<string>()), Times.Never);
        }
    }
    }


