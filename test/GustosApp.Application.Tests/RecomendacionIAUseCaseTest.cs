using GustosApp.API.Controllers;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Tests
{
    public class RecomendacionIAUseCaseTest
    {

        [Fact]
        public async Task queLaIADeUnaExplicacion()
        {
            // Arrange
            // Crear la IA,crear un usuario con sus preferencias,restaurantes con sus especialidades y el servicio
            var cancellationToken = CancellationToken.None;
            var usuario = new Usuario
            {
                Nombre = "Juan",
                Apellido = "Perez",
                Gustos = new List<Gusto> { new Gusto { Nombre = "Picante" } },
                Restricciones = new List<Restriccion> { new Restriccion { Nombre = "Sin Lactosa" } },
                CondicionesMedicas = new List<CondicionMedica> { new CondicionMedica { Nombre = "Diabetes" } }
            };

            var restaurante = new Restaurante
            {
                Nombre = "Fuego Mexicano",
                GustosQueSirve = new List<Gusto> { new Gusto { Nombre = "Picante" } },
                RestriccionesQueRespeta = new List<Restriccion> { new Restriccion { Nombre = "Opciones Veganas" } }
            };

      
            var explicacion = "Si, Fuego Mexicano es una execelente opcion porque sirve comida picante que es tu gusto. Aunque no ofrece opcion sin lactosa, debes preguntar si pueden adaptarlo.";

            var mockAIService = new Mock<IRecomendacionAIService>();

            mockAIService
              .Setup(ai => ai.GenerarRecomendacion(It.IsAny<string>()))
              .ReturnsAsync(explicacion);

            // Act
            // Obtener la explicacion

            var recomendacionIAUseCase = new RecomendacionIAUseCase(mockAIService.Object);

            var explicacionReal = await recomendacionIAUseCase.Handle(usuario, restaurante,cancellationToken);

            // Assert
            // Verificar que la explicacion no sea nula o vacia y que sea igual a la esperada

            Assert.False(string.IsNullOrEmpty(explicacionReal), "La explicación no debe ser nula o vacía.");
            Assert.Equal(explicacion, explicacionReal);
            mockAIService.Verify(
                ai => ai.GenerarRecomendacion(It.IsAny<string>()),
                Times.Once,
                "El caso de uso debe llamar al servicio de IA exactamente una vez.");
        }

     
    }
}


          

       
