using LocalVenue.Core.Entities;
using LocalVenue.Services;
using LocalVenue.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocalVenue.Core.Services;

public class SeatService(IDbContextFactory<VenueContext> contextFactory) : GenericCRUDService<Seat>(contextFactory), ISeatService
{
    private readonly IDbContextFactory<VenueContext> _contextFactory = contextFactory;


    public async Task<List<Seat>> GetSeatsInRow(int row)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        
        var seats = await context.Seats.Where(seat => seat.Row == row).ToListAsync();
        if (seats.Count == 0)
        {
            throw new ArgumentNullException($"No seats found in row {row}");
        }
        return seats;
    }

    public async Task<Seat> GetSeat(long id)
    {
        return await base.GetItem(id);
    }

    public async Task<Seat> AddSeat(Seat seat)
    {
        return await base.AddItem(seat);
    }

    public async Task<Seat> UpdateSeat(Seat seat)
    {
        return await base.UpdateItem(seat);
    }

    public async Task<Seat> DeleteSeat(long id)
    {
        return await base.DeleteItem(id);
    }
}