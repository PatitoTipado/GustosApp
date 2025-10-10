using GustosApp.Application.Interfaces;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Numerics.Tensors;
using Tokenizers.HuggingFace.Tokenizer;

// Nota: Las clases Restaurant, UserPreference, etc. se asumen como Entidades de Dominio

namespace GustosApp.Infraestructure.ML
{

    // Implementa la interfaz de Dominio y IDisposable para liberar recursos
    public class OnnxEmbeddingService : IEmbeddingService, IDisposable
    {
        // El Tokenizer se carga una sola vez y se hace est�tico (alta performance)
        private Tokenizer _tokenizer;
        private InferenceSession _session;
        private const int EmbeddingDimension = 128;

        // Constructor: Inicializa la sesi�n ONNX (debe ser inyectado como Singleton o Scoped)
        public OnnxEmbeddingService(string modelPath,string tokenizerPath)
        {
            // La ruta al modelo ONNX debe ser pasada desde la capa de Presentaci�n/Startup
            _session = new InferenceSession(modelPath);
            _tokenizer = Tokenizer.FromFile(tokenizerPath);
        }

        // Implementaci�n de la interfaz de Dominio: Convierte texto a vector
        public float[] GetEmbedding(string text)
        {
            // El c�digo de GetEmbedding de tu clase original, limpio y encapsulado

            // 1. Tokenizaci�n
            // Usamos el Tokenizer est�tico. maxLen ya no se pasa como par�metro sino como constante.
            var encodings = _tokenizer.Encode(text, true, includeTypeIds: true, includeAttentionMask: true).First();
            var sequenceLength = encodings.Ids.Count;

            // 2. Creaci�n de Tensores
            var input_ids = new DenseTensor<long>(encodings.Ids.Select(t => (long)t).ToArray(), new[] { 1, sequenceLength });
            var type_ids = new DenseTensor<long>(encodings.TypeIds.Select(t => (long)t).ToArray(), new[] { 1, sequenceLength });
            var attention_mask = new DenseTensor<long>(encodings.AttentionMask.Select(t => (long)t).ToArray(), new[] { 1, sequenceLength });

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input_ids", input_ids),
                NamedOnnxValue.CreateFromTensor("token_type_ids", type_ids),
                NamedOnnxValue.CreateFromTensor("attention_mask", attention_mask)
            };

            // 3. Ejecuci�n de la Inferencia
            using var results = _session.Run(inputs);
            var outputTensor = results.First().AsEnumerable<float>().ToArray();

            // 4. Mean Pooling (Promedio de los vectores de salida)
            float[] result = new float[EmbeddingDimension];

            for (int i = 0; i < sequenceLength; i++)
            {
                ReadOnlySpan<float> floats = new ReadOnlySpan<float>(outputTensor, i * EmbeddingDimension, EmbeddingDimension);
                TensorPrimitives.Add(floats, result, result);
            }

            TensorPrimitives.Divide(result, sequenceLength, result);

            // 5. Normalizaci�n (Crucial para la Similitud de Coseno)
            double norm = Math.Sqrt(result.Select(v => v * v).Sum());
            if (norm > 1e-6)
                for (int h = 0; h < result.Length; h++)
                    result[h] /= (float)norm;

            return result;
        }

        // Implementaci�n de IDisposable para limpiar la sesi�n ONNX
        public void Dispose()
        {
            _session.Dispose();
        }
    }
}

