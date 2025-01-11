using AutoMapper;
using LocalVenue.Core.Entities;
using LocalVenue.RequestModels;
using LocalVenue.Web.Models;

namespace LocalVenue.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TicketRequest, Core.Entities.Ticket>();

        CreateMap<ShowRequest, Core.Entities.Show>();

        CreateMap<Core.Entities.Show, Web.Models.Show>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ShowId))
            .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets != null ? src.Tickets.ToList() : null));

        CreateMap<Web.Models.Show, Core.Entities.Show>()
            .ForMember(dest => dest.ShowId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets != null ? src.Tickets.ToList() : null));

        CreateMap<Core.Entities.Ticket, Web.Models.Ticket>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TicketId))
            .ForMember(dest => dest.Seat, opt => opt.MapFrom(src => src.Seat))
            .ForMember(dest => dest.SoldToCustomerId, opt => opt.MapFrom(src => src.CustomerId));

        CreateMap<Web.Models.Ticket, Core.Entities.Ticket>()
            .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Seat, opt => opt.MapFrom(src => src.Seat))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.SoldToCustomerId));
        
        CreateMap<Core.Entities.Seat, Web.Models.Seat>();
        CreateMap<Web.Models.Seat, Core.Entities.Seat>();
            
    }
}