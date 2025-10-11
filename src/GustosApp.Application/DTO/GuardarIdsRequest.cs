using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.DTO
{
    public record GuardarIdsRequest(List<Guid> Ids, bool Skip);
   
}
