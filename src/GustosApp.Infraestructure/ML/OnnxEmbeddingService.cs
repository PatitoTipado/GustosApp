using GustosApp.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace GustosApp.Infraestructure.ML
{
    public class OnnxEmbeddingService : IEmbeddingService
    {
        private readonly OnnxInferenceService _inference;
        private readonly TokenizerAdapter _tokenizer;

        public OnnxEmbeddingService(OnnxInferenceService inference, TokenizerAdapter tokenizer)
        {
            _inference = inference;
            _tokenizer = tokenizer;
        }

        public Task<float[]> GetTextEmbeddingAsync(string text, CancellationToken ct = default)
        {
            var tokenized = _tokenizer.Encode(text);
            var vec = _inference.Run(tokenized);
            return Task.FromResult(vec);
        }
    }
}
