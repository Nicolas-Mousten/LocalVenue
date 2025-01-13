using Docker.DotNet.Models;
using LocalVenue.Core;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Services;
using LocalVenue.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace LocalVenue.Tests;

public class GenericCRUDServiceExceptionsTest
{
    // We only test for exceptions in this class as the functionality is tested when testing Services that inherit from GenericCRUDService
    private readonly ServiceProvider serviceProvider;

    public GenericCRUDServiceExceptionsTest()
    {
        var services = new ServiceCollection();
        services.AddDbContextFactory<VenueContext>(options =>
        {
            options
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        });

        serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task GetItemsSearchArgumentNullException()
    {
        // Arrange
        var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();

        var genericCRUDService = new GenericCRUDService<Show>(contextFactory);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await genericCRUDService.GetItems(1, 1, "searchParameter", "searchProperty")
        );

        Assert.Contains("does not exist on type", exception.Message);
    }

    [Fact]
    public async Task GetItemKeyNotFoundException()
    {
        // Arrange
        var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();

        var genericCRUDService = new GenericCRUDService<Show>(contextFactory);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await genericCRUDService.GetItem(1)
        );

        Assert.Contains("Item with key", exception.Message);
    }

    [Fact]
    public async Task GetItemArgumentNullException()
    {
        // Arrange
        var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();

        var genericCRUDService = new GenericCRUDService<Web.Models.Actor>(contextFactory);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await genericCRUDService.GetItem(1)
        );

        Assert.Contains("No key property found on type", exception.Message);
    }

    [Fact]
    public async Task GetItemSearchArgumentNullException()
    {
        // Arrange
        var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();

        var genericCRUDService = new GenericCRUDService<Show>(contextFactory);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await genericCRUDService.GetItem("searchParameter", "searchProperty")
        );

        Assert.Contains("does not exist on type", exception.Message);
    }

    [Fact]
    public async Task AddItemArgumentNullException()
    {
        // Arrange
        var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();

        var genericCRUDService = new GenericCRUDService<Web.Models.Actor>(contextFactory);

        var actor = new Web.Models.Actor { Name = "Test Actor", Gender = Core.Enums.Gender.Male };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await genericCRUDService.AddItem(actor)
        );

        Assert.Contains("No key property found on type", exception.Message);
    }

    [Fact]
    public async Task UpdateItemArgumentNullException()
    {
        // Arrange
        var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();

        var genericCRUDService = new GenericCRUDService<Web.Models.Actor>(contextFactory);

        var actor = new Web.Models.Actor { Name = "Test Actor", Gender = Core.Enums.Gender.Male };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await genericCRUDService.UpdateItem(actor)
        );

        Assert.Contains("No key property found on type", exception.Message);
    }

    [Fact]
    public async Task UpdateItemKeyNotFoundException()
    {
        // Arrange
        var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();
        var context = await contextFactory.CreateDbContextAsync();

        var genericCRUDService = new GenericCRUDService<Show>(contextFactory);

        var show = new Core.Entities.Show
        {
            ShowId = 1,
            Title = "Test Show",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Genre = Core.Enums.Genre.Comedy,
            OpeningNight = true,
        };
        context.Shows.Add(show);
        await context.SaveChangesAsync();

        var showToUpdate = new Core.Entities.Show
        {
            ShowId = 10,
            Title = "Test Show",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Genre = Core.Enums.Genre.Comedy,
            OpeningNight = true,
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await genericCRUDService.UpdateItem(showToUpdate)
        );

        Assert.Contains("Item with key ", exception.Message);
    }
}
