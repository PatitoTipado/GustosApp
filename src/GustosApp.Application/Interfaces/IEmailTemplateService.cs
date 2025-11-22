using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Interfaces
{
    public interface IEmailTemplateService
    {
        string Render(string templateName, Dictionary<string, string> data);
    }

}
