using LocalVenue.Core.Models;
using LocalVenue.Core.Entities;

namespace LocalVenue.Core.Interfaces
{
    public interface IShowService
    {
        Task<PagedList<ShowDTO_Nested>> GetShows(int page, int pageSize, string? searchParameter, string? searchProperty = "Title");
        Task<ShowDTO_Nested> GetShow(long id);
        Task<ShowDTO_Nested> GetShow(string searchParameter, string searchProperty = "Title");
        Task<List<TicketDTO_Nested>> GetAvailableTicketsForShow(long showId);
        Task<ShowDTO_Nested> AddShow(Show show);
        Task<ShowDTO_Nested> UpdateShow(Show show);
        Task<ShowDTO_Nested> DeleteShow(long id);
    }
}