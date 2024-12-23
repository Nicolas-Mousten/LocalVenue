using LocalVenue.Services.Interfaces;
using LocalVenue.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace LocalVenue.Services;

public class CustomerService(UserManager<Core.Entities.Customer> userManager) : ICustomerService
{
    public async Task<IdentityResult> CreateCustomerAsync(Customer customer, string password)
    {
        var businessCustomer = new Core.Entities.Customer
        {
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            UserName = customer.FirstName,
        };
        
        
        return await userManager.CreateAsync(businessCustomer, password);
    }
}