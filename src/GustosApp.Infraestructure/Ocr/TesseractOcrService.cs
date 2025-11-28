
using GustosApp.Application.Interfaces;
using Google.Cloud.Vision.V1;
using System.Text;
using Google.Apis.Auth.OAuth2;

namespace GustosApp.Infraestructure.Ocr
{
    /// <summary>
    /// OCR con Tesseract usando modelos en la carpeta tessdata indicada por constructor.
    /// Requiere paquetes:
    /// - Tesseract
    /// - Tesseract.runtime.win64 (y compilar/rutime x64)
    /// </summary>
    public sealed class GoogleVisionOcrService : IOcrService
    {
        private readonly ImageAnnotatorClient _client;

        public GoogleVisionOcrService(string jsonCredentials)
        {
            // Convertimos el JSON a Credential real
            var credential = GoogleCredential.FromJson(jsonCredentials)
                .CreateScoped(ImageAnnotatorClient.DefaultScopes);

            _client = new ImageAnnotatorClientBuilder
            {
                Credential = credential
            }.Build();
        }

        public async Task<string> ReconocerTextoAsync(IEnumerable<Stream> imagenes, string languages = "spa+eng", CancellationToken ct = default)
        {
            var sb = new StringBuilder();

            foreach (var img in imagenes)
            {
                ct.ThrowIfCancellationRequested();

                using var ms = new MemoryStream();
                await img.CopyToAsync(ms, ct);

                var response = await _client.DetectDocumentTextAsync(
                    Google.Cloud.Vision.V1.Image.FromBytes(ms.ToArray()));

                sb.AppendLine(response.Text);
            }

            return sb.ToString().Trim();
        }
    }

}


