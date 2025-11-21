using Google.GenAI;
using Google.GenAI.Types;
using GustosApp.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Environment = System.Environment;

namespace GustosApp.Application.Services
{
    public class RecomendacionAIService : IRecomendacionAIService
    {
        private readonly Client _client;
        private readonly string _apiKey;

        
        public RecomendacionAIService(IOptions<GeminiSettings> opts, HttpClient http, IConfiguration config)
        {
            var cfg = opts?.Value ?? throw new ArgumentNullException(nameof(opts));
            var apiKey = cfg.ApiKey ?? throw new ArgumentNullException("GeminiSettings:ApiKey");

            if (string.IsNullOrWhiteSpace(apiKey))
                apiKey = config["GeminiSettings:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                apiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("GeminiSettings:ApiKey no configurada. Use user-secrets, appsettings o la variable de entorno GOOGLE_API_KEY.");

            _apiKey = apiKey;
            _client = new Client(apiKey: _apiKey);
        }

        public async Task<string> GenerarRecomendacion(string prompt)
        {
            try
            {
                var response = await _client.Models.GenerateContentAsync(
                    model: "gemini-2.0-flash",
                    contents: prompt
                );

                return response.Candidates[0].Content.Parts[0].Text;
            }
            catch (Exception ex)
            {
                return $"Error generando recomendación: {ex.Message}";
            }
        }
    }
}

