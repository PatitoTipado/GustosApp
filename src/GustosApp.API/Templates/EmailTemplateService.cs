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

        public EmailTemplateService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string Render(string templateName, Dictionary<string, string> data)
        {
            var path = Path.Combine(_env.ContentRootPath, "Templates", "Email", templateName);

            if (!File.Exists(path))
                throw new FileNotFoundException($"No existe la plantilla: {path}");

            var html = File.ReadAllText(path);

            foreach (var pair in data)
                html = html.Replace("{{" + pair.Key + "}}", pair.Value);

            return html;
        }
    }

}
