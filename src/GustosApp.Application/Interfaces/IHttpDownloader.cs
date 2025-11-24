using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Interfaces
{
    public interface IHttpDownloader
    {
        Task<byte[]> DownloadAsync(string url, CancellationToken ct);
    }

}
