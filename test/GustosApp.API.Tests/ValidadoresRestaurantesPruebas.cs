using GustosApp.Application.Validations.Restaurantes;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
namespace GustosApp.API.Tests.Validaciones
{
    public class ValidadoresRestaurantesPruebas
    {
        /*  [Fact]
          public void CrearRestaurante_Valido_DeberiaSerValido()
          {
              var validator = new CrearRestauranteValidator();

              var dto = new CrearRestauranteDto
              {
                  Nombre    = "Trattoria",
                  Direccion = "Av. Italia 123",
                  Lat = -34.6,
                  Lng = -58.4,

                  PrimaryType = "italian_restaurant",
                  Types       = new List<string> { "restaurant", "italian_restaurant" },

                  Platos     = new() { "Pasta" }
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
                  Nombre    = "ab",   // muy corto
                  Direccion = "X",
                  Lat = -34.6,
                  Lng = -58.4,
                  PrimaryType = "restaurant"
              };

              var res = validator.Validate(dto);
              res.IsValid.Should().BeFalse();
              res.Errors.Should().Contain(e => e.PropertyName == "Nombre");
          }

          [Fact]
          public void CrearRestaurante_SinCoordenadas_DeberiaFallar()
          {
              var validator = new CrearRestauranteValidator();

              var dto = new CrearRestauranteDto
              {
                  Nombre    = "Demo",
                  Direccion = "Calle 1",
                  PrimaryType = "restaurant"
              };

              var res = validator.Validate(dto);
              res.IsValid.Should().BeFalse();
              (res.ToString().Contains("Lat/Lng") || res.ToString().Contains("coordenadas")).Should().BeTrue();
          }
      }
  */
    }
}
