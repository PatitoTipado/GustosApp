using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;

namespace GustosApp.Infraestructure.Services
{
    public class HttpDownloader : IHttpDownloader
    {
        private readonly HttpClient _http;

        public HttpDownloader(HttpClient http)
        {
            _http = http;
        }

        public async Task<byte[]> DownloadAsync(string url, CancellationToken ct)
        {
            return await _http.GetByteArrayAsync(url, ct);
        }
    }

}
