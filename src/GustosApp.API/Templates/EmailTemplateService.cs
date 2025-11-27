using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace GustosApp.API.Templates.Email
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<EmailTemplateService> _logger;

        public EmailTemplateService(IWebHostEnvironment env, ILogger<EmailTemplateService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public string Render(string templateName, Dictionary<string, string> data)
        {
            if (!templateName.EndsWith(".html"))
                templateName += ".html";

            var path = Path.Combine(_env.ContentRootPath, "Templates", "Email", templateName);

            _logger.LogInformation("📄 Buscando template en: {Path}", path);

            if (!File.Exists(path))
            {
                _logger.LogError("❌ No existe la plantilla en: {Path}", path);
                throw new FileNotFoundException($"No existe la plantilla: {path}");
            }

            var html = File.ReadAllText(path);

            foreach (var pair in data)
                html = html.Replace($"{{{{{pair.Key}}}}}", pair.Value);

            return html;
        }
    }


}
