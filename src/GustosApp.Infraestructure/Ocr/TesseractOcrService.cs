using System.Text;
using Tesseract;
using GustosApp.Application.Interfaces;

namespace GustosApp.Infraestructure.Ocr
{
    /// <summary>
    /// OCR con Tesseract usando modelos en la carpeta tessdata indicada por constructor.
    /// Requiere paquetes:
    /// - Tesseract
    /// - Tesseract.runtime.win64 (y compilar/rutime x64)
    /// </summary>
    public sealed class TesseractOcrService : IOcrService
    {
        private readonly string _tessdataPath;

        /// <param name="tessdataPath">
        /// Ruta absoluta a la carpeta que contiene *.traineddata (por ej.: {ContentRoot}/tessdata).
        /// </param>
        public TesseractOcrService(string tessdataPath)
        {
            if (string.IsNullOrWhiteSpace(tessdataPath))
                throw new ArgumentException("tessdataPath no puede ser nulo o vacío.", nameof(tessdataPath));

            _tessdataPath = tessdataPath;
        }

        public async Task<string> ReconocerTextoAsync(IEnumerable<Stream> imagenes, string languages = "spa+eng", CancellationToken ct = default)
        {
            if (!Directory.Exists(_tessdataPath))
                throw new DirectoryNotFoundException($"No se encontró tessdata: {_tessdataPath}");

            // 🔥 OJO: NO usar AutoOsd —ROMPE TODO—
            using var engine = new TesseractEngine(_tessdataPath, languages, EngineMode.Default);

            var sb = new StringBuilder();

            foreach (var img in imagenes)
            {
                ct.ThrowIfCancellationRequested();

                using var mem = new MemoryStream();
                await img.CopyToAsync(mem, ct);
                var bytes = mem.ToArray();

                using var pix = Pix.LoadFromMemory(bytes);

                // 👉 ESTE MODO FUNCIONA PARA MENÚS
                using var page = engine.Process(pix, PageSegMode.SingleColumn);

                var raw = page.GetText();
                Console.WriteLine(">>> RAW OCR TEXT:");
                Console.WriteLine(raw);

                if (!string.IsNullOrWhiteSpace(raw))
                    sb.AppendLine(Normalizar(raw));
            }

            return sb.ToString().Trim();
        }


        private static string Normalizar(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var lines = input
                .Replace("\r", "")
                .Split('\n')
                .Select(l => l.Trim())
                .Where(l => l.Length > 0);

            return string.Join(Environment.NewLine, lines);
        }
    }
}


