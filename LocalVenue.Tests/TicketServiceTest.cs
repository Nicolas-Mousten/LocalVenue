




using LocalVenue.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace LocalVenue.Tests;

    
    
    
public class TicketServiceTest
{
    private readonly ServiceProvider serviceProvider;
    
    
    public TicketServiceTest()
    {
        var services = new ServiceCollection();
        services.AddDbContextFactory<VenueContext>(options =>
        {
            options.UseInMemoryDatabase("InMemoryDb")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        });

        serviceProvider = services.BuildServiceProvider();
    }
    
    
    
    
}
