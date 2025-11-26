using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.AmistadMapper
{
    public class AmistadProfile : Profile
    {
        public AmistadProfile()
        {
            CreateMap<SolicitudAmistad, SolicitudAmistadResponse>()
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()))
                .ForMember(dest => dest.Remitente, opt => opt.MapFrom(src => src.Remitente))
                .ForMember(dest => dest.Destinatario, opt => opt.MapFrom(src => src.Destinatario));
        }
    }
}
