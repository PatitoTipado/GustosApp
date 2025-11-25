using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.NotificacionesMapper
{
    public class NotificacionProfile : Profile
    {
        public NotificacionProfile()
        {
            CreateMap<Notificacion, NotificacionDTO>()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()));
        }
    }
}
