using System.Threading;
using System.Threading.Tasks;

namespace GustosApp.Application.Interfaces
{

    public interface IEmbeddingService
    {
        float[] GetEmbedding(string text);
    }
}
