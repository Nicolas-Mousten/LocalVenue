using LocalVenue.Core.Enums;
using LocalVenue.Web.Models;

namespace LocalVenue.Translators;

public static class ShowTranslator
{
    public static Show Translate(Core.Entities.Show show)
    {
        return new Show
        {
            Id = show.ShowId,
            Title = show.Title,
            Description = show.Description,
            StartTime = show.StartTime,
            EndTime = show.EndTime,
            Genre = show.Genre,
            Tickets = show.Tickets?
                .Where(x => x is not null)
                .Select(TicketTranslator.Translate)
                .Where(x => x != null)
                .Cast<Ticket>()
                .ToList(),
        };
    }
}