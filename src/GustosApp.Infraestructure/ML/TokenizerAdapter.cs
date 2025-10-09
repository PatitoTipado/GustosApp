using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;

namespace GustosApp.Infraestructure.ML
{
    /// <summary>
    /// Adapter responsable de convertir texto en tensores que espera el modelo ONNX.
    /// IMPORTANTE: este es un stub inicial. Si dispones del Tokenizer exacto desde el proyecto
    /// "prueba de modelo", copia aquí ese código para asegurar compatibilidad exacta.
    /// </summary>
    public class TokenizedInput
    {
        public long[] InputIds { get; set; } = Array.Empty<long>();
        public long[] TokenTypeIds { get; set; } = Array.Empty<long>();
        public long[] AttentionMask { get; set; } = Array.Empty<long>();
        public int SequenceLength { get; set; }
    }

    public class TokenizerAdapter
    {
        private readonly string _tokenizerJsonPath;
        private readonly int _maxLen = 128;

        public TokenizerAdapter(string tokenizerJsonPath, int maxLen = 128)
        {
            _tokenizerJsonPath = tokenizerJsonPath;
            _maxLen = maxLen;
            // TODO: si tienes un tokenizador real, inicialízalo aquí leyendo tokenizer.json
        }

        /// <summary>
        /// Convierte el texto a tensores listos para pasar al modelo.
        /// Este método devuelve tensores ya ajustados a la longitud _maxLen (padding/truncation).
        /// Reemplaza esta implementación por la que tengas en 'prueba de modelo' para resultados idénticos.
        /// </summary>
        public TokenizedInput Encode(string text)
        {
            // Implementación muy simple: tokeniza por espacios y mapea a ids hash (NO recomendable en prod)
            var tokens = (text ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var ids = new long[_maxLen];
            var types = new long[_maxLen];
            var mask = new long[_maxLen];

            for (int i = 0; i < _maxLen; i++)
            {
                if (i < tokens.Length)
                {
                    // Hash como aproximación rápida — reemplazar por vocab real
                    ids[i] = Math.Abs(tokens[i].GetHashCode()) % 30000; // vocab size approx
                    mask[i] = 1;
                }
                else
                {
                    ids[i] = 0;
                    mask[i] = 0;
                }
                types[i] = 0;
            }

            return new TokenizedInput
            {
                InputIds = ids,
                TokenTypeIds = types,
                AttentionMask = mask,
                SequenceLength = _maxLen
            };
        }

        /// <summary>
        /// Helper: crea DenseTensor<long> a partir del array con shape [1, seqLen]
        /// </summary>
        public static DenseTensor<long> ToDense1D(long[] array, int seqLen)
        {
            var tensor = new DenseTensor<long>(new[] { 1, seqLen });
            for (int i = 0; i < seqLen; i++) tensor[0, i] = i < array.Length ? array[i] : 0;
            return tensor;
        }
    }
}
