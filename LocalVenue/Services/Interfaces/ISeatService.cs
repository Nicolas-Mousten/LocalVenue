using LocalVenue.Core.Entities;

namespace LocalVenue.Core.Interfaces
{
    public interface ISeatService
    {
        Task<List<Seat>> GetSeatsInRow(int row);
        Task<Seat> GetSeat(long id);
        Task<Seat> AddSeat(Seat seat);
        Task<Seat> UpdateSeat(Seat seat);
        Task<Seat> DeleteSeat(long id);
    }
}