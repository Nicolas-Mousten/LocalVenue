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
        var price = BasePrice;

        if (openingNight)
        {
            price *= 1.2m;
        }

        if (show.StartTime.Hour >= 18)
        {
            price += 20;
        }

        if (ShowLastForMoreThanTwoHours(show))
        {
            price *= 1.15m;
        }

        if (ticket.Seat.Row <= 2)
        {
            price *= 1.25m;
        }

        return price;
    }
    
    public static bool ShowLastForMoreThanTwoHours(Show show)
    {
        return new TimeSpan(2, 0, 0) < show.EndTime - show.StartTime;
    }
}