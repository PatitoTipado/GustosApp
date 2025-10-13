using FluentAssertions;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Validations.Restaurantes;
using Xunit;

namespace GustosApp.API.Tests.Validaciones;

public class ValidadoresRestaurantesPruebas
{
    [Fact]
    public void CrearRestaurante_Valido_DeberiaSerValido()
    {
        var validator = new CrearRestauranteValidator();
        var dto = new CrearRestauranteDto
        {
            Nombre = "Trattoria",
            Direccion = "Av. Italia 123",
            Latitud = -34.6,
            Longitud = -58.4,
            Tipo = "Italiana",
            Platos = new() { "Pasta" }
        };

        var res = validator.Validate(dto);
        res.IsValid.Should().BeTrue(res.ToString());
    }

    [Fact]
    public void CrearRestaurante_NombreCorto_DeberiaFallar()
    {
        var validator = new CrearRestauranteValidator();
        var dto = new CrearRestauranteDto
        {
            Nombre = "ab",
            Direccion = "X",
            Latitud = 0,
            Longitud = 0,
            Tipo = "Parrilla"
        };

        var res = validator.Validate(dto);
        res.IsValid.Should().BeFalse();
        res.Errors.Should().Contain(e => e.PropertyName == "Nombre");
    }
}
