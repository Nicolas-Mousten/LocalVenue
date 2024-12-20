using LocalVenue.Core;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Enums;

public static class DbSeeder
{
    public static void UpsertSeed(VenueContext context)
    {
        // Seed Seats
        UpsertSeat(context, new Seat { SeatId = 1, Section = "Front", Row = 1, Number = 1 });
        UpsertSeat(context, new Seat { SeatId = 2, Section = "Front", Row = 1, Number = 2 });
        UpsertSeat(context, new Seat { SeatId = 3, Section = "Front", Row = 2, Number = 1 });
        UpsertSeat(context, new Seat { SeatId = 4, Section = "Front", Row = 2, Number = 2 });
        UpsertSeat(context, new Seat { SeatId = 5, Section = "Left", Row = 1, Number = 1 });
        UpsertSeat(context, new Seat { SeatId = 6, Section = "Left", Row = 1, Number = 2 });
        UpsertSeat(context, new Seat { SeatId = 7, Section = "Left", Row = 2, Number = 1 });
        UpsertSeat(context, new Seat { SeatId = 8, Section = "Left", Row = 2, Number = 2 });
        UpsertSeat(context, new Seat { SeatId = 9, Section = "Right", Row = 1, Number = 1 });
        UpsertSeat(context, new Seat { SeatId = 10, Section = "Right", Row = 1, Number = 2 });
        UpsertSeat(context, new Seat { SeatId = 11, Section = "Right", Row = 2, Number = 1 });
        UpsertSeat(context, new Seat { SeatId = 12, Section = "Right", Row = 2, Number = 2 });
        UpsertSeat(context, new Seat { SeatId = 13, Section = "Standing", Row = 1, Number = 1 });
        UpsertSeat(context, new Seat { SeatId = 14, Section = "Standing", Row = 1, Number = 2 });
        UpsertSeat(context, new Seat { SeatId = 15, Section = "Standing", Row = 2, Number = 1 });
        UpsertSeat(context, new Seat { SeatId = 16, Section = "Standing", Row = 2, Number = 2 });
        context.SaveChanges();

        // Seed Shows
        UpsertShow(context, new Show { ShowId = 1, Title = "Comedy Night", Description = "A night full of laughs", StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(2), Genre = Genre.Comedy });
        UpsertShow(context, new Show { ShowId = 2, Title = "Magic Show", Description = "A magical evening", StartTime = DateTime.Now.AddHours(2), EndTime = DateTime.Now.AddHours(4), Genre = Genre.Romance });
        UpsertShow(context, new Show { ShowId = 3, Title = "Rock Concert", Description = "Rock the night away", StartTime = DateTime.Now.AddHours(4), EndTime = DateTime.Now.AddHours(6), Genre = Genre.Horror });
        UpsertShow(context, new Show { ShowId = 4, Title = "Dance Performance", Description = "An evening of dance", StartTime = DateTime.Now.AddHours(8), EndTime = DateTime.Now.AddHours(10), Genre = Genre.Documentary });
        UpsertShow(context, new Show { ShowId = 5, Title = "Drama Play", Description = "A dramatic performance", StartTime = DateTime.Now.AddHours(10), EndTime = DateTime.Now.AddHours(12), Genre = Genre.Drama });
        context.SaveChanges();

        //Seed Tickets
        var seats = context.Seats.ToList();
        var shows = context.Shows.ToList();
        var ticketId = 1;
        var random = new Random();
        foreach (Show show in shows)
        {
            foreach (Seat seat in seats)
            {
                UpsertTicket(context, new Ticket { TicketId = ticketId++, Price = (decimal)(random.NextDouble() * 100), SeatId = seat.SeatId, ShowId = show.ShowId });
            }
        }
        context.SaveChanges();
    }

    private static void UpsertSeat(VenueContext context, Seat seat)
    {
        var existingSeat = context.Seats.Find(seat.SeatId);
        if (existingSeat == null)
        {
            context.Seats.Add(seat);
        }
        else
        {
            context.Entry(existingSeat).CurrentValues.SetValues(seat);
        }
    }

    private static void UpsertShow(VenueContext context, Show show)
    {
        var existingShow = context.Shows.Find(show.ShowId);
        if (existingShow == null)
        {
            context.Shows.Add(show);
        }
        else
        {
            context.Entry(existingShow).CurrentValues.SetValues(show);
        }
    }

    private static void UpsertTicket(VenueContext context, Ticket ticket)
    {
        var existingTicket = context.Tickets.Find(ticket.TicketId);
        if (existingTicket == null)
        {
            context.Tickets.Add(ticket);
        }
        else
        {
            context.Entry(existingTicket).CurrentValues.SetValues(ticket);
        }
    }
}