using GustosApp.API.Controllers;
using GustosApp.Application.DTO;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiTest
{
    public class GustoControllerTest
    {

        //como probar con esto? vi que se puede hacer asi y tener unica instancia 
        //o crear un http client cual va a ser el aprobado por la catedra
        //tmb podria ser perfectamente un mock o un metodo que se inicie antes de cada prueba tipo before para hacer setup


        [Fact]
        public async Task HandleAsync_DeberiaRetornarListaDeGustos()
        {
            // Arrange
            var mockRepo = new Mock<IGustoRepository>();
            mockRepo
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Gusto>
                {
                    new Gusto { Id = Guid.NewGuid(), Nombre = "Pizza", ImagenUrl = "pizza.png" },
                    new Gusto { Id = Guid.NewGuid(), Nombre = "Sushi", ImagenUrl = "sushi.png" }
                });

            var useCase = new ObtenerGustosUseCase(mockRepo.Object);

            // Act
            var result = await useCase.HandleAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, g => g.nombre == "Pizza");
            Assert.Contains(result, g => g.nombre == "Sushi");
        }
    }



}

    

    
