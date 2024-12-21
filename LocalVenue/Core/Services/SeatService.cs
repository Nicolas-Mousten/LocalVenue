using LocalVenue.Core.Entities;
using LocalVenue.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocalVenue.Core.Services;

public class SeatService : GenericCRUDService<Seat>, ISeatService
{
    private readonly VenueContext _context;

    public SeatService(VenueContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Seat>> GetSeatsInRow(int row)
    {
        var seats = await _context.Seats.Where(seat => seat.Row == row).ToListAsync();
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