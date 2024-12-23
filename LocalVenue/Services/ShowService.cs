using LocalVenue.Core;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Models;
using LocalVenue.Core.Services;
using LocalVenue.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocalVenue.Services;

public class ShowService(IDbContextFactory<VenueContext> contextFactory) : GenericCRUDService<Show>(contextFactory), IShowService
{
    private readonly IDbContextFactory<VenueContext> _contextFactory = contextFactory;

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
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var ticket = await context.Shows.Include(show => show.Tickets).FirstOrDefaultAsync(show => show.ShowId == showId);

        if (ticket == null)
        {
            throw new KeyNotFoundException($"Show with id {showId} not found");
        }

        return ticket!.Tickets!.Where(ticket => ticket.Status == Core.Enums.Status.Available).ToList();
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

    public async Task<List<Show>> GetAllShows()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        return await context.Shows.ToListAsync();
    }
}