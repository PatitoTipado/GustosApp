
namespace GustosApp.Domain.Tests.Common
{
    using GustosApp.Domain.Common;
    using System;
    using Xunit;

    public class CosenoTests
    {
        [Fact]
        public void Coseno_VectoresIdenticos_DaUno()
        {
            float[] a = { 1, 2, 3 };
            float[] b = { 1, 2, 3 };

            double result = CosineSimilarity.Coseno(a, b);

            Assert.True(Math.Abs(result - 1.0) < 1e-6);
        }

        [Fact]
        public void Coseno_VectoresOrtogonales_DaCero()
        {
            float[] a = { 1, 0 };
            float[] b = { 0, 1 };

            double result = CosineSimilarity.Coseno(a, b);

            Assert.True(Math.Abs(result) < 1e-6);
        }

        [Fact]
        public void Coseno_VectoresOpuestos_DaMenosUno()
        {
            float[] a = { 1, 2, 3 };
            float[] b = { -1, -2, -3 };

            double result = CosineSimilarity.Coseno(a, b);

            Assert.True(Math.Abs(result + 1.0) < 1e-6);
        }

        [Fact]
        public void Coseno_VectoresConCeros_NoRevienta()
        {
            float[] a = { 0, 0, 0 };
            float[] b = { 1, 2, 3 };

            double result = CosineSimilarity.Coseno(a, b);

            // dot = 0, magnitudes = 0 => retornará 0 / (1e-8) = 0
            Assert.Equal(0, result, 6);
        }

        [Fact]
        public void Coseno_VectoresCeroYCero_RetornaCero()
        {
            float[] a = { 0, 0 };
            float[] b = { 0, 0 };

            double result = CosineSimilarity.Coseno(a, b);

            Assert.Equal(0, result, 6);
        }

        [Fact]
        public void Coseno_CalculoGeneral_Correcto()
        {
            float[] a = { 1, 3 };
            float[] b = { 2, 1 };

            // Calculado a mano
            double dot = 1 * 2 + 3 * 1; // 5
            double na = Math.Sqrt(1 + 9); // sqrt(10)
            double nb = Math.Sqrt(4 + 1); // sqrt(5)
            double esperado = dot / (na * nb);

            double result = CosineSimilarity.Coseno(a, b);

            Assert.True(Math.Abs(result - esperado) < 1e-6);
        }
    }
}
