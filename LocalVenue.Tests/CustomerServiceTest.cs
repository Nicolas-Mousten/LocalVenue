using LocalVenue.Core;
using LocalVenue.Core.Entities;
using LocalVenue.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace LocalVenue.Tests;

public class CustomerServiceTest
{
    private readonly ServiceProvider serviceProvider;

    public CustomerServiceTest()
    {
        var services = new ServiceCollection();

        services
            .AddIdentityCore<Customer>(options => { })
            .AddEntityFrameworkStores<VenueContext>()
            .AddDefaultTokenProviders();

        services.AddDataProtection();

        services.AddDbContext<VenueContext>(options =>
            options.UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
        );

        var userManagerMock = new Mock<UserManager<Customer>>(
            Mock.Of<IUserStore<Customer>>(),
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!
        );

        userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<Customer>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        services.AddSingleton(userManagerMock.Object);

        services.AddScoped<CustomerService>();

        serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task CreateCustomerAsyncSucceed()
    {
        var customerService = serviceProvider.GetRequiredService<CustomerService>();

        var customer = new Web.Models.Customer
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "John@hotmail.com",
        };

        var response = await customerService.CreateCustomerAsync(customer, "Password123");

        Assert.True(response.Succeeded);
    }

    [Fact]
    public async Task CreateCustomerAsyncFail()
    {
        var customerService = serviceProvider.GetRequiredService<CustomerService>();

        var customer = new Web.Models.Customer
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "John@hotmail.com",
        };

        var response = await customerService.CreateCustomerAsync(customer, "");

        Assert.False(response.Succeeded);
    }
}
