using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Application.UseCases.RestauranteUseCases.OpinionesRestaurantes;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Moq;


namespace GustosApp.Application.Tests
{
    public class ActualizarValoracionRestauranteUseCaseTests
    {
        private readonly Mock<IRestauranteRepository> _restauranteRepo;
        private readonly Mock<IOpinionRestauranteRepository> _opinionesRepo;
        private readonly ActualizarValoracionRestauranteUseCase _useCase;

        public ActualizarValoracionRestauranteUseCaseTests()
        {
            _restauranteRepo = new Mock<IRestauranteRepository>();
            _opinionesRepo = new Mock<IOpinionRestauranteRepository>();

            _useCase = new ActualizarValoracionRestauranteUseCase(
                _restauranteRepo.Object,
                _opinionesRepo.Object
            );
        }

        private OpinionRestaurante FakeOpinion(double valor)
            => new OpinionRestaurante { Valoracion = valor };

        [Fact]
        public async Task HandleAsync_SinOpiniones_DeberiaActualizarConCero()
        {
            var id = Guid.NewGuid();

            _opinionesRepo.Setup(r => r.ObtenerPorRestauranteAsync(id, default))
                .ReturnsAsync(new List<OpinionRestaurante>());

            _restauranteRepo.Setup(r =>
                r.ActualizarValoracionAsync(id, 0, default))
                .Returns(Task.CompletedTask);

            await _useCase.HandleAsync(id, default);

            _restauranteRepo.Verify(r =>
                r.ActualizarValoracionAsync(id, 0, default),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ConOpiniones_DeberiaCalcularPromedio()
        {
            var id = Guid.NewGuid();

            var opiniones = new List<OpinionRestaurante>
        {
            FakeOpinion(3),
            FakeOpinion(5),
            FakeOpinion(4)
        };

            _opinionesRepo.Setup(r => r.ObtenerPorRestauranteAsync(id, default))
                .ReturnsAsync(opiniones);

            double promedioEsperado = (3 + 5 + 4) / 3.0;

            await _useCase.HandleAsync(id, default);

            _restauranteRepo.Verify(r =>
                r.ActualizarValoracionAsync(id, promedioEsperado, default),
                Times.Once);
        }


        [Fact]
        public async Task HandleAsync_DeberiaLlamarActualizarUnaSolaVez()
        {
            var id = Guid.NewGuid();

            var opiniones = new List<OpinionRestaurante>
        {
            FakeOpinion(5),
            FakeOpinion(1)
        };

            _opinionesRepo.Setup(r => r.ObtenerPorRestauranteAsync(id, default))
                .ReturnsAsync(opiniones);

            await _useCase.HandleAsync(id, default);

            _restauranteRepo.Verify(r =>
                r.ActualizarValoracionAsync(id, It.IsAny<double>(), default),
                Times.Once);
        }

      

        [Fact]
        public async Task HandleAsync_PromedioDecimal_DeberiaCalcularDouble()
        {
            var id = Guid.NewGuid();

            var opiniones = new List<OpinionRestaurante>
        {
            FakeOpinion(5),
            FakeOpinion(4),
            FakeOpinion(4)
        };
            // Promedio = 13 / 3 = 4.3333333

            _opinionesRepo.Setup(r => r.ObtenerPorRestauranteAsync(id, default))
                .ReturnsAsync(opiniones);

            await _useCase.HandleAsync(id, default);

            _restauranteRepo.Verify(r =>
                r.ActualizarValoracionAsync(id,
                    It.Is<double>(v => Math.Abs(v - 4.3333) < 0.0001),
                    default),
                Times.Once);
        }



        [Fact]
        public async Task HandleAsync_DeberiaConsultarValoracionesConIdCorrecto()
        {
            var id = Guid.NewGuid();

            _opinionesRepo.Setup(r => r.ObtenerPorRestauranteAsync(id, default))
                .ReturnsAsync(new List<OpinionRestaurante>());

            await _useCase.HandleAsync(id, default);

            _opinionesRepo.Verify(r =>
                r.ObtenerPorRestauranteAsync(id, default),
                Times.Once);
        }
    }
}
