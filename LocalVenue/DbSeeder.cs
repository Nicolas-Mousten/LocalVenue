using LocalVenue.Core;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static void UpsertSeed(VenueContext context)
    {
        if (context.Shows.Any(show => show.EndTime < DateTime.Now))
        {
            int offset = 1;
            var showsToUpdate = new List<Show>();
            foreach (var show in context.Shows)
            {
                var existingShow = context.Shows.Find(show.ShowId);
                if (existingShow != null)
                {
                    context.Entry(existingShow).State = EntityState.Detached;
                }

                var newShow = new Show
                {
                    ShowId = show.ShowId,
                    Title = show.Title,
                    Description = show.Description,
                    StartTime = DateTime.Now.AddHours(offset),
                    EndTime = DateTime.Now.AddHours(offset + 2),
                    Genre = show.Genre
                };
                showsToUpdate.Add(newShow);
                offset += 2;
            }
            context.Shows.UpdateRange(showsToUpdate);
            context.SaveChanges();
        }

        if (context.Shows.Any())
        {
            return;
        }

        // Seed Seats
        UpsertSeat(context, new Seat { SeatId = 1, Section = "Front", Row = 1, Number = 1 });
        UpsertSeat(context, new Seat { SeatId = 2, Section = "Front", Row = 1, Number = 2 });
        UpsertSeat(context, new Seat { SeatId = 3, Section = "Front", Row = 1, Number = 3 });
        UpsertSeat(context, new Seat { SeatId = 4, Section = "Front", Row = 1, Number = 4 });
        UpsertSeat(context, new Seat { SeatId = 5, Section = "Left", Row = 2, Number = 1 });
        UpsertSeat(context, new Seat { SeatId = 6, Section = "Left", Row = 2, Number = 2 });
        UpsertSeat(context, new Seat { SeatId = 7, Section = "Left", Row = 2, Number = 3 });
        UpsertSeat(context, new Seat { SeatId = 8, Section = "Left", Row = 2, Number = 4 });
        UpsertSeat(context, new Seat { SeatId = 9, Section = "Right", Row = 3, Number = 1 });
        UpsertSeat(context, new Seat { SeatId = 10, Section = "Right", Row = 3, Number = 2 });
        UpsertSeat(context, new Seat { SeatId = 11, Section = "Right", Row = 3, Number = 3 });
        UpsertSeat(context, new Seat { SeatId = 12, Section = "Right", Row = 3, Number = 4 });
        UpsertSeat(context, new Seat { SeatId = 13, Section = "Standing", Row = 4, Number = 1 });
        UpsertSeat(context, new Seat { SeatId = 14, Section = "Standing", Row = 4, Number = 2 });
        UpsertSeat(context, new Seat { SeatId = 15, Section = "Standing", Row = 4, Number = 3 });
        UpsertSeat(context, new Seat { SeatId = 16, Section = "Standing", Row = 4, Number = 4 });
        context.SaveChanges();

        // Seed Shows
        UpsertShow(context, new Show { ShowId = 1, Title = "Comedy Night", Description = "A night full of laughs", StartTime = DateTime.Now.AddHours(1), EndTime = DateTime.Now.AddHours(3), Genre = Genre.Comedy });
        UpsertShow(context, new Show { ShowId = 2, Title = "Magic Show", Description = "A magical evening", StartTime = DateTime.Now.AddHours(3), EndTime = DateTime.Now.AddHours(5), Genre = Genre.Romance });
        UpsertShow(context, new Show { ShowId = 3, Title = "Rock Concert", Description = "Rock the night away", StartTime = DateTime.Now.AddHours(5), EndTime = DateTime.Now.AddHours(7), Genre = Genre.Horror });
        UpsertShow(context, new Show { ShowId = 4, Title = "Dance Performance", Description = "An evening of dance", StartTime = DateTime.Now.AddHours(7), EndTime = DateTime.Now.AddHours(9), Genre = Genre.Documentary });
        UpsertShow(context, new Show { ShowId = 5, Title = "Drama Play", Description = "A dramatic performance", StartTime = DateTime.Now.AddHours(9), EndTime = DateTime.Now.AddHours(11), Genre = Genre.Drama });
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

        //Seed Admin user
        var admin = new Customer
        {
            Id = "1",
            FirstName = "Admin",
            LastName = "Admin",
            Email = "admin@hotmail.com",
            NormalizedUserName = "ADMIN",
            NormalizedEmail = "ADMIN@HOTMAIL.COM",
            UserName = "admin"
        };

        if (context.Users.Find("1") == null)
        {

            var passwordHasher = new PasswordHasher<Customer>();

            admin.PasswordHash = passwordHasher.HashPassword(admin, "123");

            context.Users.Add(admin);

            var role = new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN",
            };

            context.SaveChanges();

            context.Roles.Add(role);

            context.Roles.Add(role);

            var roleMapping = new IdentityUserRole<string>
            {
                RoleId = role.Id,
                UserId = admin.Id
            };

            context.UserRoles.Add(roleMapping);

            context.SaveChanges();

        }
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