using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using GustosApp.Application.UseCases;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Tests
{
    public class SugerirGustosUseCase_EmbeddingTests
    {
        [Fact]
        public void Handle_Debe_Aplicar_Penalizacion_y_Umbral_Correctamente()
        {
            // 1. Configuración del Mock IEmbeddingService
            var mockEmbeddingService = new Mock<IEmbeddingService>();

            // 2. Instanciación del Use Case con el Mock
            var useCase = new SugerirGustosUseCase(mockEmbeddingService.Object);


        }
    }

}