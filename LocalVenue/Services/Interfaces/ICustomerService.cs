using LocalVenue.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace LocalVenue.Services.Interfaces;

public interface ICustomerService
{
    public Task<IdentityResult> CreateCustomerAsync(Customer customer, string password);
}