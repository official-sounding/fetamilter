using System.Security.Claims;
using App;
using App.Authorization;
using App.Config;
using App.Services;
using Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.Configure<SiteConfig>(builder.Configuration.GetSection(SiteConfig.SECTION));


var siteConfig = builder.Configuration.GetSection(SiteConfig.SECTION).Get<SiteConfig>();

ArgumentNullException.ThrowIfNull(siteConfig);

// Add services to the container.
var mvcbuilder = builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddSingleton<ISiteService, SiteService>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IPostService, PostService>();
builder.Services.AddTransient<IFavoriteFacade, PostFavoriteFacade>();
builder.Services.AddTransient<IFavoriteFacade, CommentFavoriteFacade>();
builder.Services.AddTransient<IFavoriteService, FavroiteService>();


builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.SlidingExpiration = true;
        o.Cookie.HttpOnly = true;
        o.LoginPath = "/login";
        o.ExpireTimeSpan = TimeSpan.FromDays(365);
        o.Cookie.Domain = $".{siteConfig.RootDomain}";
        o.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAuthorization(o =>
{
    foreach (var p in Policy.AllPolicies)
    {
        o.AddPolicy(p, (pol) => pol.RequireClaim(ClaimTypes.Role, p));
    }
});

builder.Services.AddDatabase(builder.Configuration);

if (builder.Environment.IsDevelopment())
{
    mvcbuilder.AddRazorRuntimeCompilation();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}



app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();
        await DbInitializer.Initialize(context, true);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}



app.Run();
