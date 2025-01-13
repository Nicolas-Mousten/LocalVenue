using LocalVenue.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LocalVenue.Core;

public class VenueContext : IdentityDbContext<Customer>
{
    public VenueContext(DbContextOptions<VenueContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Ticket>()
            .HasOne(t => t.Customer)
            .WithMany(c => c.Tickets)
            .HasForeignKey(t => t.CustomerId)
            .IsRequired(false);

        modelBuilder
            .Entity<Customer>()
            .HasMany(c => c.Tickets)
            .WithOne(t => t.Customer)
            .HasForeignKey(t => t.CustomerId)
            .IsRequired(false);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Show> Shows { get; init; }
    public DbSet<Seat> Seats { get; init; }
    public DbSet<Ticket> Tickets { get; init; }
}
