using LocalVenue.Core.Entities;
using LocalVenue.Core.Interfaces;
using LocalVenue.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalVenue.Core.Services;

public class ShowService : GenericCRUDService<Show>, IShowService
{
    private readonly VenueContext _context;

    public ShowService(VenueContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PagedList<Show>> GetShows(int page, int pageSize, string? searchParameter, string? searchProperty = "Title")
    {
        return await base.GetItems(page, pageSize, searchParameter, searchProperty ?? "Title", show => show.Tickets!); ;
    }

    public async Task<Show> GetShow(long id)
    {
        return await base.GetItem(id, show => show.Tickets!);
    }

    public async Task<Show> GetShow(string searchParameter, string searchProperty = "Title")
    {
        return await base.GetItem(searchParameter, searchProperty, show => show.Tickets!);
    }

    public async Task<List<Ticket>> GetAvailableTicketsForShow(long showId)
    {
        var ticket = await _context.Shows.Include(show => show.Tickets).FirstOrDefaultAsync(show => show.ShowId == showId);

        if (ticket == null)
        {
            throw new KeyNotFoundException($"Show with id {showId} not found");
        }

        return ticket!.Tickets!.Where(ticket => ticket.Status == Enums.Status.Available).ToList();
    }

    public async Task<Show> AddShow(Show show)
    {
        return await base.AddItem(show, show => show.Tickets!);
    }

    public async Task<Show> UpdateShow(Show show)
    {
        return await base.UpdateItem(show);
    }

    public async Task<Show> DeleteShow(long id)
    {
        return await base.DeleteItem(id);
    }
}