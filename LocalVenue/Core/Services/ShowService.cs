using AutoMapper;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Interfaces;
using LocalVenue.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalVenue.Core.Services;

public class ShowService : GenericCRUDService<Show>, IShowService
{
    private readonly VenueContext _context;
    private readonly IMapper _mapper;

    public ShowService(VenueContext context, IMapper mapper) : base(context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedList<ShowDTO_Nested>> GetShows(int page, int pageSize, string? searchParameter, string? searchProperty = "Title")
    {
        var shows = await base.GetItems(page, pageSize, searchParameter, searchProperty ?? "Title", show => show.Tickets!); ;

        var mappedShows = shows.Results!.Select(_mapper.Map<ShowDTO_Nested>).ToList();

        return new PagedList<ShowDTO_Nested>
        {
            Count = shows.Count,
            Results = mappedShows,
            Next = shows.Next
        };
    }

    public async Task<ShowDTO_Nested> GetShow(long id)
    {
        var returnShow = await base.GetItem(id, show => show.Tickets!);
        return _mapper.Map<ShowDTO_Nested>(returnShow);
    }

    public async Task<ShowDTO_Nested> GetShow(string searchParameter, string searchProperty = "Title")
    {
        var returnshow = await base.GetItem(searchParameter, searchProperty, show => show.Tickets!);
        return _mapper.Map<ShowDTO_Nested>(returnshow);
    }

    public async Task<List<TicketDTO_Nested>> GetAvailableTicketsForShow(long showId)
    {
        var ticket = await _context.Shows.Include(show => show.Tickets).FirstOrDefaultAsync(show => show.ShowId == showId);

        if (ticket == null)
        {
            throw new KeyNotFoundException($"Show with id {showId} not found");
        }

        var tickets = ticket!.Tickets!.Where(ticket => ticket.Status == Enums.Status.Available).ToList();

        var mappedTickets = tickets.Select(_mapper.Map<TicketDTO_Nested>).ToList();

        if (mappedTickets.Count == 0)
        {
            throw new ArgumentNullException($"No available tickets found for show with id {showId}");
        }

        return mappedTickets;
    }

    public async Task<ShowDTO_Nested> AddShow(Show show)
    {
        var returnShow = await base.AddItem(show, show => show.Tickets!);
        return _mapper.Map<ShowDTO_Nested>(returnShow);
    }

    public async Task<ShowDTO_Nested> UpdateShow(Show show)
    {
        var returnShow = await base.UpdateItem(show);
        return _mapper.Map<ShowDTO_Nested>(returnShow);
    }

    public async Task<ShowDTO_Nested> DeleteShow(long id)
    {
        var returnShow = await base.DeleteItem(id);
        return _mapper.Map<ShowDTO_Nested>(returnShow);
    }
}