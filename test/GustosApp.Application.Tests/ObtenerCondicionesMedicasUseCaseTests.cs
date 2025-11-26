using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.UseCases.UsuarioUseCases.CondicionesMedicasUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;

namespace GustosApp.Application.Tests
{
    public class ObtenerCondicionesMedicasUseCaseTests
    {
        private readonly Mock<ICondicionMedicaRepository> _condiciones;
        private readonly Mock<IUsuarioRepository> _usuarios;
        private readonly ObtenerCondicionesMedicasUseCase _useCase;

        public ObtenerCondicionesMedicasUseCaseTests()
        {
            _condiciones = new Mock<ICondicionMedicaRepository>();
            _usuarios = new Mock<IUsuarioRepository>();
            _useCase = new ObtenerCondicionesMedicasUseCase(_condiciones.Object, _usuarios.Object);
        }

      
        [Fact]
        public async Task HandleAsync_UsuarioNoEncontrado_DeberiaLanzarExcepcion()
        {
            _usuarios.Setup(r => r.GetByFirebaseUidAsync("uid123", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);

            var act = async () => await _useCase.HandleAsync("uid123");

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Usuario no encontrado*");
        }

        
        [Fact]
        public async Task HandleAsync_DeberiaDevolverTodasYSeleccionadas()
        {
            var uid = "uid123";

            var cond1 = new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Celiaquía" };
            var cond2 = new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Diabetes" };
            var cond3 = new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Hipertensión" };

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = uid,
                CondicionesMedicas = new List<CondicionMedica> { cond1, cond3 }
            };

            _usuarios.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _condiciones.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CondicionMedica> { cond1, cond2, cond3 });

            // Act
            var (todas, seleccionadas) = await _useCase.HandleAsync(uid);

            // Assert
            todas.Should().HaveCount(3);
            seleccionadas.Should().BeEquivalentTo(new[] { cond1.Id, cond3.Id });
        }

       
        [Fact]
        public async Task HandleAsync_UsuarioSinCondiciones_DeberiaDevolverSeleccionVacia()
        {
            var uid = "uid123";

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                FirebaseUid = uid,
                CondicionesMedicas = null! // o new List<CondicionMedica>()
            };

            _usuarios.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            var conds = new List<CondicionMedica>
        {
            new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Asma" },
            new CondicionMedica { Id = Guid.NewGuid(), Nombre = "Hipertensión" }
        };

            _condiciones.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(conds);

            var (todas, seleccionadas) = await _useCase.HandleAsync(uid);

            todas.Should().HaveCount(2);
            seleccionadas.Should().BeEmpty();
        }

      
        [Fact]
        public async Task HandleAsync_DeberiaLlamarRepositoriosCorrectamente()
        {
            var uid = "uid123";

            _usuarios.Setup(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Usuario { Id = Guid.NewGuid(), FirebaseUid = uid });

            _condiciones.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CondicionMedica>());

            await _useCase.HandleAsync(uid);

            _usuarios.Verify(r => r.GetByFirebaseUidAsync(uid, It.IsAny<CancellationToken>()), Times.Once);
            _condiciones.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
