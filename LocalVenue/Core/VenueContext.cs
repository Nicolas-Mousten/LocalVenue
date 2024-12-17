using LocalVenue.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalVenue.Core;

public class VenueContext : DbContext
{
    public VenueContext(DbContextOptions<VenueContext> options) : base(options)
    {
    }
    
    public DbSet<Show> Shows { get; init; }
    public DbSet<Seat> Seats { get; init; }
    public DbSet<Ticket> Tickets { get; init; }
    
    
    
    
}