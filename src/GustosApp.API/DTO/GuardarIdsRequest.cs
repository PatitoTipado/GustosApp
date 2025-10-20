using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.API.DTO
{
    public class GuardarIdsRequest()
    {
     
        public List<Guid> Ids { get; set; }
        public bool Skip { get; set; }
    }
   
}
