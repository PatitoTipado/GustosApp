using System.Threading;
using System.Threading.Tasks;

namespace GustosApp.Application.Interfaces
{
    public interface IEmbeddingRepository
    {
        Task<float[]> GetTextEmbeddingAsync(string text, CancellationToken ct = default);
    }
}