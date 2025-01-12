using LocalVenue.Web.Models;

namespace LocalVenue.Helpers
{
    public static class DiscountCalculator
    {
        public static List<Ticket> ApplyDiscount(
            this List<Ticket> tickets,
            int ticketsAlreadyPurchased
        )
        {
            {
                var ticketsToBuy = Math.Clamp(4 - ticketsAlreadyPurchased, 0, 4);
                foreach (var ticket in tickets.OrderBy(x => x.Price).Take(ticketsToBuy))
                {
                    ticket.Price *= 0.8m;
                }

                return tickets;
            }
        }
    }
}
