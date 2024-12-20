using AutoMapper;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Ticket, TicketDTO>();
        CreateMap<TicketDTO, Ticket>();
        CreateMap<Ticket, TicketDTO_Nested>()
            .ForMember(dest => dest.Show, opt => opt.MapFrom(src => src.Show))
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer));
        CreateMap<TicketDTO_Nested, Ticket>();

        CreateMap<Show, ShowDTO>();
        CreateMap<ShowDTO, Show>();
        CreateMap<Show, ShowDTO_Nested>()
            .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets));
        CreateMap<ShowDTO_Nested, Show>();
    }
}