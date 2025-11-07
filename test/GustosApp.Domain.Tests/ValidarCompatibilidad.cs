using GustosApp.Application.Tests.mocks;
using GustosApp.Domain.Model;

namespace GustosApp.Domain.Tests
{
    public class ValidarCompatibilidad
    {
            [Fact]
            public void ValidarCompatibilidad_DeberiaDetectarYRemoverGustosIncompatibles()
            {
                // Arrange
                var usuario = new UsuarioRealFake("firebase_1", "test@mail.com", "Juan", "Pérez", "USR1");

                // TAGS
                var tagGluten = new Tag { Nombre = "Gluten" };
                var tagAzucar = new Tag { Nombre = "Azúcar" };
                var tagSal = new Tag { Nombre = "Sal" };

                // GUSTOS
                var pizza = new Gusto { Nombre = "Pizza", Tags = new List<Tag> { tagGluten, tagSal } };
                var helado = new Gusto { Nombre = "Helado", Tags = new List<Tag> { tagAzucar } };
                var sushi = new Gusto { Nombre = "Sushi", Tags = new List<Tag> { tagSal } };
                usuario.Gustos = new List<Gusto> { pizza, helado, sushi };

                // RESTRICCIONES
                var sinGluten = new Restriccion { Nombre = "Sin gluten", TagsProhibidos = new List<Tag> { tagGluten } };
                usuario.Restricciones.Add(sinGluten);

                // CONDICIONES MÉDICAS
                var diabetes = new CondicionMedica { Nombre = "Diabetes", TagsCriticos = new List<Tag> { tagAzucar } };
                usuario.CondicionesMedicas.Add(diabetes);

                // Act
                var conflictos = usuario.ValidarCompatibilidad();

                // Assert
                Assert.Contains("Pizza", conflictos);
                Assert.Contains("Helado", conflictos);
                Assert.DoesNotContain("Sushi", conflictos);

                // Los gustos incompatibles deben eliminarse del usuario
                Assert.DoesNotContain(usuario.Gustos, g => g.Nombre == "Pizza");
                Assert.DoesNotContain(usuario.Gustos, g => g.Nombre == "Helado");
                Assert.Contains(usuario.Gustos, g => g.Nombre == "Sushi");
            }
        
        [Fact]
        public void ValidarCompatibilidad_DeberiaDevolverVacioSiTodoEsCompatible()
        {
            // Arrange
            var usuario = new UsuarioRealFake("firebase_2", "ok@mail.com", "Ana", "García", "USR2");

            var tagFruta = new Tag { Nombre = "Fruta" };
            var gusto = new Gusto { Nombre = "Ensalada de frutas", Tags = new List<Tag> { tagFruta } };

            usuario.Gustos.Add(gusto);
            usuario.Restricciones.Add(new Restriccion { Nombre = "Sin gluten" });
            usuario.CondicionesMedicas.Add(new CondicionMedica { Nombre = "Hipertensión" });

            // Act
            var conflictos = usuario.ValidarCompatibilidad();

            // Assert
            Assert.Empty(conflictos);
            Assert.Single(usuario.Gustos);
        }
        [Fact]
        public void ValidarCompatibilidad_DeberiaDetectarConflictosPorCondiciones()
        {
            // Arrange
            var usuario = new UsuarioRealFake("firebase_3", "med@mail.com", "Luis", "Santos", "USR3");

            var tagAzucar = new Tag { Nombre = "Azúcar" };
            var tagVegetal = new Tag { Nombre = "Vegetal" };

            var postre = new Gusto { Nombre = "Postre", Tags = new List<Tag> { tagAzucar } };
            var ensalada = new Gusto { Nombre = "Ensalada", Tags = new List<Tag> { tagVegetal } };



            usuario.Gustos = usuario.Gustos.Concat(new[] { postre, ensalada }).ToList();

            usuario.CondicionesMedicas.Add(new CondicionMedica
            {
                Nombre = "Diabetes",
                TagsCriticos = new List<Tag> { tagAzucar }
            });

            // Act
            var conflictos = usuario.ValidarCompatibilidad();

            // Assert
            Assert.Contains("Postre", conflictos);
            Assert.DoesNotContain("Ensalada", conflictos);
            Assert.Single(usuario.Gustos); // sólo la ensalada debería quedar
        }

        [Fact]
        public void ValidarCompatibilidad_DeberiaCombinarTagsDeMultiplesRestricciones()
        {
            // Arrange
            var usuario = new UsuarioRealFake("firebase_4", "restr@mail.com", "Laura", "Gómez", "USR4");

            var tagGluten = new Tag { Nombre = "Gluten" };
            var tagLacteo = new Tag { Nombre = "Lácteo" };

            var pizza = new Gusto { Nombre = "Pizza", Tags = new List<Tag> { tagGluten, tagLacteo } };
            var sushi = new Gusto { Nombre = "Sushi", Tags = new List<Tag>() };

            usuario.Gustos = usuario.Gustos.Concat(new[] { pizza, sushi }).ToList();

            usuario.Restricciones.Add(new Restriccion { Nombre = "Sin gluten", TagsProhibidos = new List<Tag> { tagGluten } });
            usuario.Restricciones.Add(new Restriccion { Nombre = "Sin lactosa", TagsProhibidos = new List<Tag> { tagLacteo } });

            // Act
            var conflictos = usuario.ValidarCompatibilidad();

            // Assert
            Assert.Contains("Pizza", conflictos);
            Assert.DoesNotContain("Sushi", conflictos);
            Assert.Single(usuario.Gustos); // sólo sushi queda
        }



    }
}
