using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.UsuariosMapper
{




    public class PerfilUsuarioProfile : Profile
    {
        public PerfilUsuarioProfile()
        {
            CreateMap<Usuario, UsuarioPerfilResponse>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.IdUsuario));

            CreateMap<Gusto, GustoLiteDto>();

            CreateMap<UsuarioRestauranteVisitado, VisitadoDto>()
                .ForMember(dest => dest.IdRestaurante, opt =>
                    opt.MapFrom(src =>
                        src.RestauranteId.HasValue
                            ? src.RestauranteId.Value.ToString()
                            : src.Id.ToString()))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src =>
                    src.Restaurante != null ? src.Restaurante.Nombre : string.Empty))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src =>
                    src.Restaurante != null ? src.Restaurante.Direccion : string.Empty))
                .ForMember(dest => dest.FechaVisita, opt => opt.MapFrom(src => src.FechaVisitaUtc));
        }
    }


}
