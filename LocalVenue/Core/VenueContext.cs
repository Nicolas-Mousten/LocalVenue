using LocalVenue.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LocalVenue.Core;

public class VenueContext : IdentityDbContext<Customer>
{
    public VenueContext(DbContextOptions<VenueContext> options) : base(options)
    {
    }
    
    public DbSet<Show> Shows { get; init; }
    public DbSet<Seat> Seats { get; init; }
    public DbSet<Ticket> Tickets { get; init; }
    
    
    
    
}