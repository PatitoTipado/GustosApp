using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using GustosApp.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GustosApp.Infraestructure.Services
{
    public class FirebaseStorageService : IFileStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly IConfiguration _config;
        private readonly ILogger<FirebaseStorageService> _logger;
        private static readonly string[] _allowedMimeTypes =
           { "image/jpeg", "image/png", "image/webp" };

        public FirebaseStorageService(
            string? firebaseJson,
            string localFilePath,
            IConfiguration config,
            ILogger<FirebaseStorageService> logger)
        {
            _config = config;
            _logger = logger;

            GoogleCredential credential;

            if (!string.IsNullOrEmpty(firebaseJson))
            {
                credential = GoogleCredential.FromJson(firebaseJson);
                _logger.LogInformation("🔥 Firebase cargado desde JSON (PROD)");
            }
            else if (File.Exists(localFilePath))
            {
                credential = GoogleCredential.FromFile(localFilePath);
                _logger.LogInformation("💻 Firebase cargado desde archivo local");
            }
            else
            {
                _logger.LogError("❌ No se encontró ni JSON ni archivo físico de Firebase");
                throw new InvalidOperationException("Credenciales de Firebase no disponibles.");
            }

            _storageClient = StorageClient.Create(credential);

            _bucketName = _config["Firebase:StorageBucket"]
                ?? throw new InvalidOperationException("Falta Firebase:StorageBucket");
        }


        


        public async Task<string> UploadFileAsync(Stream stream, string fileName, string? folder = null)
        {
            if (stream == null || stream.Length == 0)
                throw new ArgumentException("Archivo vacío o nulo.");

            // Detectar MIME y validar tipo
            var mimeType = GetMimeType(fileName);
            if (!_allowedMimeTypes.Contains(mimeType))
                throw new InvalidOperationException($"Tipo de archivo no permitido: {mimeType}");

            // Nombre único (GUID + timestamp)
            var uniqueName = $"{Guid.NewGuid():N}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(fileName)}";
            var objectName = string.IsNullOrWhiteSpace(folder)
                ? uniqueName
                : $"{folder.TrimEnd('/')}/{uniqueName}";

            await _storageClient.UploadObjectAsync(_bucketName, objectName, mimeType, stream);

            return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(objectName)}?alt=media";
        }

        public async Task DeleteFileAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            try
            {
                var objectName = ExtractObjectName(url);
                await _storageClient.DeleteObjectAsync(_bucketName, objectName);
            }
            catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
            {
                // Archivo ya eliminado, ignorar
            }
        }

        private static string ExtractObjectName(string url)
        {
            var marker = "/o/";
            var start = url.IndexOf(marker) + marker.Length;
            var end = url.IndexOf("?alt=media", start);
            var encodedName = url.Substring(start, end - start);
            return Uri.UnescapeDataString(encodedName);
        }

        private static string GetMimeType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}
