using LocalVenue.Core;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Models;
using LocalVenue.Core.Services;
using LocalVenue.Services.Interfaces;
using LocalVenue.Translators;
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
        await using var context = await _contextFactory.CreateDbContextAsync();

        var returnedShow = await base.AddItem(show, show => show.Tickets!);
        var seats = await context.Seats.ToListAsync();

        foreach (var seat in seats)
        {
            var newTicket = new Ticket
            {
                ShowId = returnedShow.ShowId,
                SeatId = seat.SeatId,
                Status = Core.Enums.Status.Available
            };

            context.Tickets.Add(newTicket);
        }
        context.SaveChanges();

        returnedShow = await base.GetItem(returnedShow.ShowId, show => show.Tickets!);
        return returnedShow;
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

    public async Task<bool> CreateShowAsync(Web.Models.Show show)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var newShow = new Show
            {
                Title = show.Title,
                Description = show.Description,
                StartTime = show.StartTime,
                EndTime = show.EndTime,
                Genre = show.Genre,
                OpeningNight = show.OpeningNight,
            };

            await AddShow(newShow);
        }
        catch
        {
            return false;
        }
        return true;
    }

    public async Task<bool> UpdateShowAsync(Web.Models.Show show)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var showToUpdate = await context.Shows.FirstOrDefaultAsync(x => x.ShowId == show.Id);

            if (showToUpdate == null)
            {
                return false;
            }

            showToUpdate.Title = show.Title;
            showToUpdate.Description = show.Description;
            showToUpdate.StartTime = show.StartTime;
            showToUpdate.EndTime = show.EndTime;
            showToUpdate.Genre = show.Genre;
            showToUpdate.OpeningNight = show.OpeningNight;

            context.Shows.Update(showToUpdate);
            await context.SaveChangesAsync();

        }
        catch
        {
            return false;
        }
        return true;
    }

    public async Task<Web.Models.Show?> GetShowWithTicketsAsync(long id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var showWithTickets = await context.Shows
            .Include(show => show.Tickets!)
            .ThenInclude(ticket => ticket.Seat)
            .FirstOrDefaultAsync(show => show.ShowId == id);

        if (showWithTickets == null)
        {
            return null;
        }

        return ShowTranslator.Translate(showWithTickets);
    }
}