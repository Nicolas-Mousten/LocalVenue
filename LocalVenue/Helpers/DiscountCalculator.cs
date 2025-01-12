using LocalVenue.Web.Models;

namespace LocalVenue.Helpers
{
    public static class DiscountCalculator
    {
        public static List<Ticket> ApplyDiscount(this List<Ticket> tickets, int ticketsAlreadyPurchased)
        {
            {
                foreach (var ticket in tickets.OrderByDescending(x => x.Price).Take(ticketsAlreadyPurchased))
                {
                    ticket.Price *= 0.8m;
                }

                return tickets;
            }
        }
    }
}