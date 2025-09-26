using GustosApp.API.Controllers;

namespace ApiTest
{
    public class GustoControllerTest
    {
        private readonly GustoController gustoController = new GustoController();
        //como probar con esto? vi que se puede hacer asi y tener unica instancia 
        //o crear un http client cual va a ser el aprobado por la catedra
        //tmb podria ser perfectamente un mock o un metodo que se inicie antes de cada prueba tipo before para ahcer setup

        [Fact]
        public void Test1()
        {
            string resultado = gustoController.Hola();

            Assert.Equal("Hola soy GustoApp", resultado);
        }
    }
}