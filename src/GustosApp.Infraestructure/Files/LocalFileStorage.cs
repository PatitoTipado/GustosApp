using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;

namespace GustosApp.Infraestructure.Files
{
    public class LocalFileStorage : IAlmacenamientoArchivos
    {
        private readonly string _uploadsRoot;
  
        /// uploadsRoot = ruta absoluta a "wwwroot/uploads"

        public LocalFileStorage(string uploadsRoot)
        {
            _uploadsRoot = uploadsRoot;
            Directory.CreateDirectory(_uploadsRoot);
        }

        public async Task<string> SubirAsync(Stream contenido, string rutaRelativa, CancellationToken ct = default)
        {
            var fullPath = Path.Combine(_uploadsRoot, rutaRelativa);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await contenido.CopyToAsync(fs, ct);
            }

            // URL pública relativa: /uploads/...
            var url = "/uploads/" + rutaRelativa.Replace("\\", "/");
            return url;
        }
    }
}
