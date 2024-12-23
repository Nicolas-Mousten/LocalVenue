using LocalVenue.Core.Entities;
using LocalVenue.Core.Models;

namespace LocalVenue.Services.Interfaces
{
    public interface IShowService
    {
        Task<PagedList<Show>> GetShows(int page, int pageSize, string? searchParameter, string? searchProperty = "Title");
        Task<Show> GetShow(long id);
        Task<Show> GetShow(string searchParameter, string searchProperty = "Title");
        Task<List<Ticket>> GetAvailableTicketsForShow(long showId);
        Task<Show> AddShow(Show show);
        Task<Show> UpdateShow(Show show);
        Task<Show> DeleteShow(long id);
        public Task<List<Show>> GetAllShows();
        public Task<bool> CreateShowAsync(Web.Models.Show show);
        public Task<bool> UpdateShowAsync(Web.Models.Show show);
    }
}