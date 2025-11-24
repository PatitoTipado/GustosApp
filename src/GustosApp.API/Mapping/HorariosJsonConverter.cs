using System.Text.Json;
using AutoMapper;
using GustosApp.API.DTO;

namespace GustosApp.API.Mapping
{
    public class HorariosJsonConverter : IValueConverter<string?, List<HorarioSimpleDto>>
    {
        public List<HorarioSimpleDto> Convert(string? source, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source))
                return new List<HorarioSimpleDto>();

            try
            {
                return JsonSerializer.Deserialize<List<HorarioSimpleDto>>(source)
                       ?? new List<HorarioSimpleDto>();
            }
            catch
            {
                return new List<HorarioSimpleDto>();
            }
        }
    }

}
