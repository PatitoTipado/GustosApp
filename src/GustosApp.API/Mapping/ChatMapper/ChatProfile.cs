using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.Mapping.ChatMapper
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<ChatMensaje, ChatMensajeResponse>();
        }
    }
}
