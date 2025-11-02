

namespace GustosApp.API.DTO
{
    public class GuardarIdsRequest()
    {
     
        public List<Guid> Ids { get; set; }
        public bool Skip { get; set; }
    }
   
}
