using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.API.Controllers;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Collections.Generic;

namespace GustosApp.API.Tests.Api;

public class RestaurantesControllerPruebas
{
    private static RestaurantesController ConstruirController(Mock<IServicioRestaurantes> mock)
    {
        var controller = new RestaurantesController(mock.Object);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("user_id", "uid-test"),
            new Claim(ClaimTypes.Role, "user")
        }, "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        return controller;
    }

    [Fact]
    public async Task Post_DeberiaDevolver201ConUbicacion()
    {
        var mock = new Mock<IServicioRestaurantes>();
        var creado = new RestauranteDto
        {
            Id = Guid.NewGuid(),
            PropietarioUid = "uid-test",
            Nombre = "Parrilla Demo",
            Direccion = "Calle",
            Latitud = -34.6,
            Longitud = -58.4,
            Tipo = "Parrilla",
            Platos = new() { "Hamburguesas" }
        };

        mock.Setup(s => s.CrearAsync("uid-test", It.IsAny<CrearRestauranteDto>()))
            .ReturnsAsync(creado);

        var controller = ConstruirController(mock);
        var resp = await controller.Post(new CrearRestauranteDto { Nombre = "Parrilla Demo", Direccion = "Calle", Latitud = -1, Longitud = -1, Tipo = "Parrilla" });

        resp.Should().BeOfType<CreatedAtActionResult>();
        var result = (CreatedAtActionResult)resp;
        result.RouteValues!["id"].Should().Be(creado.Id);
        result.Value.Should().BeOfType<RestauranteDto>();
    }



}
