using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Domain.Model;
using GustosApp.Infraestructure.Services;
using GustosApp.API.Tests.Infraestructura;
using Xunit;

namespace GustosApp.API.Tests.Servicios
{
    public class ServicioRestaurantesPruebas
    {
        private static CrearRestauranteDto DtoBasico() => new CrearRestauranteDto
        {
            Nombre   = "Parrilla Demo",
            Direccion= "Calle 123",
            Lat      = -34.6,
            Lng      = -58.4,
            Horarios = new { tz = "America/Argentina/Buenos_Aires" },

            PrimaryType = "barbecue_restaurant",
            Types       = new List<string> { "restaurant", "barbecue_restaurant", "food" },

            Platos      = new() { "Hamburguesas" },

            ImagenUrl   = "https://ejemplo.com/img.jpg",
            Valoracion  = 4.2m
        };

        [Fact]
        public async Task Crear_DeberiaCrearYDevolverDto()
        {
            var ctx      = DbContextEnMemoria.Crear(nameof(Crear_DeberiaCrearYDevolverDto));
            var servicio = new ServicioRestaurantes(ctx);

            var dto = DtoBasico();
            var res = await servicio.CrearAsync("uid-1", dto);

            res.Id.Should().NotBeEmpty();
            res.Nombre.Should().Be(dto.Nombre);
            res.PrimaryType.Should().Be(dto.PrimaryType);
            res.Types.Should().Contain(new[] { "restaurant", "barbecue_restaurant" });
            res.Platos.Should().Contain("Hamburguesas");
        }

        [Fact]
        public async Task Crear_CuandoNombreDuplicado_DeberiaFallar()
        {
            var ctx      = DbContextEnMemoria.Crear(nameof(Crear_CuandoNombreDuplicado_DeberiaFallar));
            var servicio = new ServicioRestaurantes(ctx);

            await servicio.CrearAsync("uid-1", DtoBasico());

            var act = async () => await servicio.CrearAsync("uid-2", DtoBasico());
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*ya está en uso*");
        }

        [Fact]
        public async Task Crear_CuandoFaltanCoordenadas_DeberiaFallar()
        {
            var ctx      = DbContextEnMemoria.Crear(nameof(Crear_CuandoFaltanCoordenadas_DeberiaFallar));
            var servicio = new ServicioRestaurantes(ctx);

            var dto = DtoBasico();
            dto.Lat = null; dto.Lng = null;
            dto.Latitud = null; dto.Longitud = null;

            var act = async () => await servicio.CrearAsync("uid-1", dto);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Lat/Lng*"); // mensaje del servicio
        }

        [Fact]
        public async Task ListarCercanos_FiltraPorPlatos()
        {
            var ctx      = DbContextEnMemoria.Crear(nameof(ListarCercanos_FiltraPorPlatos));
            var servicio = new ServicioRestaurantes(ctx);

            // A: Pasta (cerca)
            var a = DtoBasico();
            a.Nombre = "Trattoria A";
            a.PrimaryType = "italian_restaurant";
            a.Types = new() { "restaurant", "italian_restaurant" };
            a.Platos = new() { "Pasta" };
            a.Lat = -34.600; a.Lng = -58.400;
            await servicio.CrearAsync("u1", a);

            // B: Sushi (cerca)
            var b = DtoBasico();
            b.Nombre = "Sushi B";
            b.PrimaryType = "japanese_restaurant";
            b.Types = new() { "restaurant", "japanese_restaurant" };
            b.Platos = new() { "Sushi" };
            b.Lat = -34.601; b.Lng = -58.401;
            await servicio.CrearAsync("u2", b);

            // C: Postres (lejos)
            var c = DtoBasico();
            c.Nombre = "Trattoria C";
            c.PrimaryType = "italian_restaurant";
            c.Types = new() { "restaurant", "italian_restaurant" };
            c.Platos = new() { "Postres" };
            c.Lat = -35.0; c.Lng = -58.0; // lejos
            await servicio.CrearAsync("u3", c);

            var res = await servicio.ListarCercanosAsync(-34.60, -58.40, 1000, tipo: null, platos: new[] { "Pasta" });

            res.Should().HaveCount(1);
            res.First().Nombre.Should().Be("Trattoria A");
        }

        [Fact]
        public async Task Actualizar_DeberiaReemplazarPlatosYDatos()
        {
            var ctx      = DbContextEnMemoria.Crear(nameof(Actualizar_DeberiaReemplazarPlatosYDatos));
            var servicio = new ServicioRestaurantes(ctx);

            var creado = await servicio.CrearAsync("u1", DtoBasico());

            var dto = new ActualizarRestauranteDto
            {
                Nombre    = "Nuevo Nombre",
                Direccion = "Nueva 456",
                Lat = -34.61, Lng = -58.41,
                Horarios = new { tz = "America/Argentina/Buenos_Aires" },

                PrimaryType = "mexican_restaurant",
                Types       = new() { "restaurant", "mexican_restaurant" },

                Platos     = new() { "Ensaladas" },
                ImagenUrl  = "https://nuevo.jpg",
                Valoracion = 3.9m
            };

            var actualizado = await servicio.ActualizarAsync(creado.Id, "u1", false, dto);

            actualizado.Should().NotBeNull();
            actualizado!.Nombre.Should().Be("Nuevo Nombre");
            actualizado.PrimaryType.Should().Be("mexican_restaurant");
            actualizado.Types.Should().Contain("mexican_restaurant");
            actualizado.Platos.Should().BeEquivalentTo(new[] { "Ensaladas" });
        }
    }
}

