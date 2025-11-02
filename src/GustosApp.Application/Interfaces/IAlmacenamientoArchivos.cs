using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GustosApp.Application.Interfaces
{
    public interface IAlmacenamientoArchivos
    {
        Task<string> SubirAsync(Stream contenido, string rutaRelativa, CancellationToken ct = default);
    }
}