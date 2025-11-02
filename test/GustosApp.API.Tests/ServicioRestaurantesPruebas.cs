using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Domain.Model;
using GustosApp.Infraestructure.Services;
using GustosApp.API.Tests.Infraestructura;
using Xunit;

namespace GustosApp.API.Tests.Servicios;

public class ServicioRestaurantesPruebas
{
   /* private static CrearRestauranteDto DtoBasico() => new CrearRestauranteDto
    {
        Nombre = "Parrilla Demo",
        Direccion = "Calle 123",
        Latitud = -34.6,
        Longitud = -58.4,
        Horarios = new { tz = "America/Argentina/Buenos_Aires" },
        Tipo = "Parrilla",
        Platos = new() { "Hamburguesas" },
        ImagenUrl = "https://ejemplo.com/img.jpg",
        Valoracion = 4.2m
    };

    [Fact]
    public async Task Crear_DeberiaCrearYDevolverDto()
    {
        var ctx = DbContextEnMemoria.Crear(nameof(Crear_DeberiaCrearYDevolverDto));
        var servicio = new ServicioRestaurantes(ctx);

        var dto = DtoBasico();
        var res = await servicio.CrearAsync("uid-1", dto);

        res.Id.Should().NotBeEmpty();
        res.Nombre.Should().Be(dto.Nombre);
        res.Tipo.Should().Be(TipoRestaurante.Parrilla);
        res.Platos.Should().Contain("Hamburguesas");
    }

    [Fact]
    public async Task Crear_CuandoNombreDuplicado_DeberiaFallar()
    {
        var ctx = DbContextEnMemoria.Crear(nameof(Crear_CuandoNombreDuplicado_DeberiaFallar));
        var servicio = new ServicioRestaurantes(ctx);

        await servicio.CrearAsync("uid-1", DtoBasico());

        var act = async () => await servicio.CrearAsync("uid-2", DtoBasico());
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*ya está en uso*");
    }

    [Fact]
    public async Task Crear_CuandoTipoInvalido_DeberiaFallar()
    {
        var ctx = DbContextEnMemoria.Crear(nameof(Crear_CuandoTipoInvalido_DeberiaFallar));
        var servicio = new ServicioRestaurantes(ctx);

        var dto = DtoBasico();
        dto.Tipo = "TIPO_QUE_NO_EXISTE";

        var act = async () => await servicio.CrearAsync("uid-1", dto);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Tipo inválido*");
    }

    [Fact]
    public async Task ListarCercanos_FiltraPorTipoYPlatos()
    {
        var ctx = DbContextEnMemoria.Crear(nameof(ListarCercanos_FiltraPorTipoYPlatos));
        var servicio = new ServicioRestaurantes(ctx);

        // A: Italiana / Pasta
        var a = DtoBasico();
        a.Nombre = "Trattoria A";
        a.Tipo = "Italiana";
        a.Platos = new() { "Pasta" };
        a.Latitud = -34.60; a.Longitud = -58.40;
        await servicio.CrearAsync("u1", a);

        // B: Japonesa / Sushi
        var b = DtoBasico();
        b.Nombre = "Sushi B";
        b.Tipo = "Japonesa";
        b.Platos = new() { "Sushi" };
        b.Latitud = -34.601; b.Longitud = -58.401;
        await servicio.CrearAsync("u2", b);

        // C: Italiana / Postres (fuera de radio)
        var c = DtoBasico();
        c.Nombre = "Trattoria C";
        c.Tipo = "Italiana";
        c.Platos = new() { "Postres" };
        c.Latitud = -35.0; c.Longitud = -58.0; // lejos
        await servicio.CrearAsync("u3", c);

        var res = await servicio.ListarCercanosAsync(-34.60, -58.40, 1000, "Italiana", new[] { "Pasta" });

        res.Should().HaveCount(1);
        res.First().Nombre.Should().Be("Trattoria A");
    }

    [Fact]
    public async Task Actualizar_DeberiaReemplazarPlatosYDatos()
    {
        var ctx = DbContextEnMemoria.Crear(nameof(Actualizar_DeberiaReemplazarPlatosYDatos));
        var servicio = new ServicioRestaurantes(ctx);

        var creado = await servicio.CrearAsync("u1", DtoBasico());

        var dto = new ActualizarRestauranteDto
        {
            Nombre = "Nuevo Nombre",
            Direccion = "Nueva 456",
            Latitud = -34.61,
            Longitud = -58.41,
            Horarios = new { tz = "America/Argentina/Buenos_Aires" },
            Tipo = "Mexicana",
            Platos = new() { "Ensaladas" },
            ImagenUrl = "https://nuevo.jpg",
            Valoracion = 3.9m
        };

        var actualizado = await servicio.ActualizarAsync(creado.Id, "u1", false, dto);

        actualizado.Should().NotBeNull();
        actualizado!.Nombre.Should().Be("Nuevo Nombre");
        actualizado.Tipo.Should().Be("Mexicana");
        actualizado.Platos.Should().BeEquivalentTo(new[] { "Ensaladas" });
    }
   */
}
