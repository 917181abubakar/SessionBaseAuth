using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestApi.Data;
using TestApi.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using NuGet.Packaging.Signing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints().AddDefaultTokenProviders();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Mysession";
    //options.IdleTimeout = TimeSpan.FromMinutes(1);
    //options.Cookie.MaxAge=TimeSpan.FromMinutes(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use Secure cookies if HTTPS
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
})
.AddCookie(IdentityConstants.ApplicationScheme, options =>
{
    options.LoginPath = "/User/login";
    options.LogoutPath = "/User/logout";
    options.AccessDeniedPath = "/account/access-denied";
    options.SlidingExpiration = true;
    //options.ExpireTimeSpan = TimeSpan.FromMinutes(6);
    options.Cookie.HttpOnly = true;
    options.Cookie.Name = "Mysessioncookie";

});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"))); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSession();
app.UseMiddleware<SessionValidationMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapIdentityApi<IdentityUser>();

app.MapControllers();


app.Run();
