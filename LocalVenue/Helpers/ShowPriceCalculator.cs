using AutoMapper;
using LocalVenue.Web.Models;

namespace LocalVenue.Helpers;

public static class ShowPriceCalculator
{
    private const decimal BasePrice = 120;

    public static List<Show> AddPrice(this List<Show> shows)
    {
        foreach (var show in shows)
        {
            if (show.Tickets is not null)
            {
                foreach (var ticket in show.Tickets)
                {
                    ticket.Price = CalculatePrice(show, ticket, show.OpeningNight);
                }
            }
        }

        return shows;
    }

    public static decimal CalculatePrice(Show show, Ticket ticket, bool openingNight)
    {
        decimal additionalPrice = 0;

        if (openingNight)
        {
            additionalPrice += BasePrice * 0.2m;
        }

        if (ShowLastForMoreThanTwoHours(show))
        {
            additionalPrice += BasePrice * 0.15m;
        }

        if (ticket.Seat == null)
        {
            throw new ArgumentException("Ticket must have a seat");
        }

        if (ticket.Seat.Row <= 2)
        {
            additionalPrice += BasePrice * 0.25m;
        }

        if (show.StartTime.Hour >= 18)
        {
            additionalPrice += 20;
        }

        return BasePrice + additionalPrice;
    }

    public static decimal CalculatePrice(Core.Entities.Show show, Core.Entities.Ticket ticket, bool openingNight, IMapper mapper)
    {
        var webShow = mapper.Map<Show>(show);
        var webTicket = mapper.Map<Ticket>(ticket);

        return CalculatePrice(webShow, webTicket, openingNight);
    }

    public static bool ShowLastForMoreThanTwoHours(Show show)
    {
        return new TimeSpan(2, 0, 0) < show.EndTime - show.StartTime;
    }
}