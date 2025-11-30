using System;
using System.Linq;
using GustosApp.Domain.Model;
using Xunit;

namespace GustosApp.Application.Tests
{
    public class VotacionGrupoModelTests
    {
        [Fact]
        public void CerrarVotacion_VotacionYaCerrada_LanzaInvalidOperationException()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var votacion = new VotacionGrupo(grupoId);
            votacion.CerrarVotacion();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => votacion.CerrarVotacion());
            Assert.Equal("Solo se pueden cerrar votaciones activas", exception.Message);
        }

        [Fact]
        public void CerrarVotacion_VotacionCancelada_LanzaInvalidOperationException()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var votacion = new VotacionGrupo(grupoId);
            votacion.Cancelar();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => votacion.CerrarVotacion());
            Assert.Equal("Solo se pueden cerrar votaciones activas", exception.Message);
        }

        [Fact]
        public void CerrarVotacion_ConGanador_EstableceGanadorYCierraVotacion()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var restauranteGanadorId = Guid.NewGuid();
            var votacion = new VotacionGrupo(grupoId);

            // Act
            votacion.CerrarVotacion(restauranteGanadorId);

            // Assert
            Assert.Equal(EstadoVotacion.Cerrada, votacion.Estado);
            Assert.Equal(restauranteGanadorId, votacion.RestauranteGanadorId);
            Assert.NotNull(votacion.FechaCierre);
        }

        [Fact]
        public void CerrarVotacion_SinGanador_CierraVotacionSinGanador()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var votacion = new VotacionGrupo(grupoId);

            // Act
            votacion.CerrarVotacion();

            // Assert
            Assert.Equal(EstadoVotacion.Cerrada, votacion.Estado);
            Assert.Null(votacion.RestauranteGanadorId);
            Assert.NotNull(votacion.FechaCierre);
        }

        [Fact]
        public void EstablecerGanadorRuleta_VotacionCerrada_LanzaInvalidOperationException()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();
            var votacion = new VotacionGrupo(grupoId);
            votacion.CerrarVotacion();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(
                () => votacion.EstablecerGanadorRuleta(restauranteId));
            Assert.Equal("Solo se puede establecer ganador en votaciones activas", exception.Message);
        }

        [Fact]
        public void EstablecerGanadorRuleta_GanadorYaSeleccionado_LanzaInvalidOperationException()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var restauranteId1 = Guid.NewGuid();
            var restauranteId2 = Guid.NewGuid();
            var votacion = new VotacionGrupo(grupoId);
            votacion.EstablecerGanadorRuleta(restauranteId1);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(
                () => votacion.EstablecerGanadorRuleta(restauranteId2));
            Assert.Equal("Ya se seleccionó un ganador para esta votación", exception.Message);
        }

        [Fact]
        public void EstablecerGanadorRuleta_VotacionActiva_EstableceGanador()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();
            var votacion = new VotacionGrupo(grupoId);

            // Act
            votacion.EstablecerGanadorRuleta(restauranteId);

            // Assert
            Assert.Equal(restauranteId, votacion.RestauranteGanadorId);
            Assert.Equal(EstadoVotacion.Activa, votacion.Estado);
        }

        [Fact]
        public void Cancelar_VotacionActiva_CancelaVotacion()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var votacion = new VotacionGrupo(grupoId);

            // Act
            votacion.Cancelar();

            // Assert
            Assert.Equal(EstadoVotacion.Cancelada, votacion.Estado);
            Assert.NotNull(votacion.FechaCierre);
        }

        [Fact]
        public void Cancelar_VotacionCerrada_LanzaInvalidOperationException()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var votacion = new VotacionGrupo(grupoId);
            votacion.CerrarVotacion();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => votacion.Cancelar());
            Assert.Equal("No se puede cancelar una votación cerrada", exception.Message);
        }

        [Fact]
        public void ObtenerResultados_SinVotos_RetornaDiccionarioVacio()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var votacion = new VotacionGrupo(grupoId);

            // Act
            var resultados = votacion.ObtenerResultados();

            // Assert
            Assert.Empty(resultados);
        }

        [Fact]
        public void ObtenerResultados_ConVotos_RetornaConteoCorrect()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var votacionId = Guid.NewGuid();
            var usuario1Id = Guid.NewGuid();
            var usuario2Id = Guid.NewGuid();
            var usuario3Id = Guid.NewGuid();
            var restaurante1Id = Guid.NewGuid();
            var restaurante2Id = Guid.NewGuid();

            var votacion = new VotacionGrupo(grupoId);
            
            // Usar reflexión para establecer el Id ya que es private set
            typeof(VotacionGrupo).GetProperty("Id")!.SetValue(votacion, votacionId);

            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario1Id, restaurante1Id, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario2Id, restaurante1Id, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario3Id, restaurante2Id, null));

            // Act
            var resultados = votacion.ObtenerResultados();

            // Assert
            Assert.Equal(2, resultados.Count);
            Assert.Equal(2, resultados[restaurante1Id]);
            Assert.Equal(1, resultados[restaurante2Id]);
        }

        [Fact]
        public void TodosHanVotado_TodosVotaron_RetornaTrue()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var votacionId = Guid.NewGuid();
            var usuario1Id = Guid.NewGuid();
            var usuario2Id = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();

            var votacion = new VotacionGrupo(grupoId);
            typeof(VotacionGrupo).GetProperty("Id")!.SetValue(votacion, votacionId);

            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario1Id, restauranteId, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario2Id, restauranteId, null));

            // Act
            var todosVotaron = votacion.TodosHanVotado(2);

            // Assert
            Assert.True(todosVotaron);
        }

        [Fact]
        public void TodosHanVotado_NoTodosVotaron_RetornaFalse()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var votacionId = Guid.NewGuid();
            var usuario1Id = Guid.NewGuid();
            var restauranteId = Guid.NewGuid();

            var votacion = new VotacionGrupo(grupoId);
            typeof(VotacionGrupo).GetProperty("Id")!.SetValue(votacion, votacionId);

            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario1Id, restauranteId, null));

            // Act
            var todosVotaron = votacion.TodosHanVotado(3);

            // Assert
            Assert.False(todosVotaron);
        }

        [Fact]
        public void ObtenerRestaurantesEmpatados_SinVotos_RetornaListaVacia()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var votacion = new VotacionGrupo(grupoId);

            // Act
            var empatados = votacion.ObtenerRestaurantesEmpatados();

            // Assert
            Assert.Empty(empatados);
        }

        [Fact]
        public void ObtenerRestaurantesEmpatados_ConEmpate_RetornaRestaurantesEmpatados()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var votacionId = Guid.NewGuid();
            var usuario1Id = Guid.NewGuid();
            var usuario2Id = Guid.NewGuid();
            var restaurante1Id = Guid.NewGuid();
            var restaurante2Id = Guid.NewGuid();

            var votacion = new VotacionGrupo(grupoId);
            typeof(VotacionGrupo).GetProperty("Id")!.SetValue(votacion, votacionId);

            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario1Id, restaurante1Id, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario2Id, restaurante2Id, null));

            // Act
            var empatados = votacion.ObtenerRestaurantesEmpatados();

            // Assert
            Assert.Equal(2, empatados.Count);
            Assert.Contains(restaurante1Id, empatados);
            Assert.Contains(restaurante2Id, empatados);
        }

        [Fact]
        public void ObtenerRestaurantesEmpatados_SinEmpate_RetornaGanadorUnico()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var votacionId = Guid.NewGuid();
            var usuario1Id = Guid.NewGuid();
            var usuario2Id = Guid.NewGuid();
            var usuario3Id = Guid.NewGuid();
            var restaurante1Id = Guid.NewGuid();
            var restaurante2Id = Guid.NewGuid();

            var votacion = new VotacionGrupo(grupoId);
            typeof(VotacionGrupo).GetProperty("Id")!.SetValue(votacion, votacionId);

            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario1Id, restaurante1Id, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario2Id, restaurante1Id, null));
            votacion.Votos.Add(new VotoRestaurante(votacionId, usuario3Id, restaurante2Id, null));

            // Act
            var empatados = votacion.ObtenerRestaurantesEmpatados();

            // Assert
            Assert.Single(empatados);
            Assert.Contains(restaurante1Id, empatados);
        }

        [Fact]
        public void Constructor_ConDescripcion_EstableceDescripcion()
        {
            // Arrange
            var grupoId = Guid.NewGuid();
            var descripcion = "Votación para cena de viernes";

            // Act
            var votacion = new VotacionGrupo(grupoId, descripcion);

            // Assert
            Assert.Equal(grupoId, votacion.GrupoId);
            Assert.Equal(descripcion, votacion.Descripcion);
            Assert.Equal(EstadoVotacion.Activa, votacion.Estado);
            Assert.NotEqual(Guid.Empty, votacion.Id);
        }

        [Fact]
        public void Constructor_SinDescripcion_DescripcionEsNull()
        {
            // Arrange
            var grupoId = Guid.NewGuid();

            // Act
            var votacion = new VotacionGrupo(grupoId);

            // Assert
            Assert.Equal(grupoId, votacion.GrupoId);
            Assert.Null(votacion.Descripcion);
            Assert.Equal(EstadoVotacion.Activa, votacion.Estado);
        }
    }
}
