using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.DTO
{
    public record GustoDto(Guid Id, string Nombre, string? ImagenUrl);

}
