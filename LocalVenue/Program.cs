using LocalVenue.Web;
using LocalVenue.Core;
using LocalVenue.Core.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.WebComponents;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<VenueContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("VenueContext")!,
    ServerVersion.Parse("8.0-mysql"));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
        options.Cookie.Name = "CookieAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.MaxAge = TimeSpan.FromDays(1);
        options.Cookie.IsEssential = true;
    });

builder.Services.AddIdentity<Customer, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
        options.SignIn.RequireConfirmedAccount = false;
        
        options.Password.RequiredLength = 3;
        options.Password.RequireUppercase = false;
        options.Password.RequireDigit = false;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<VenueContext>();

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

//test push in GitHub with organization
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<BlazorCookieLoginMiddleware>();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();