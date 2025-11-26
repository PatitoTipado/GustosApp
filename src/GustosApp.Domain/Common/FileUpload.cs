using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Common
{
    public class FileUpload
    {
        public string FileName { get; set; } = string.Empty;
        public Stream Content { get; set; } = null!;
        public string ContentType { get; set; } = string.Empty;
   
    }
}
