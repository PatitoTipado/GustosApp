using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Services
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(Stream stream, string nombreArchivo, string? carpeta = null);

    }
}
