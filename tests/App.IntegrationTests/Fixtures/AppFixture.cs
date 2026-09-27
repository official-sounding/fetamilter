using App.Config;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace App.IntegrationTests.Fixtures;

public class AppFixture() : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder("postgres:18.0")
            .WithDatabase("fetamilter_test")
            .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await RunMigrationsAsync();
        await SeedDataAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ApplyTestData", "false");
        builder.UseSetting($"Site:{nameof(SiteConfig.IncludePort)}", "true");
        builder.UseSetting("ConnectionStrings:pgsql", _dbContainer.GetConnectionString());
        builder.UseEnvironment("Testing");

        builder.UseStaticWebAssets();
    }

    private async Task RunMigrationsAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        await context.Database.MigrateAsync();
    }

    private async Task SeedDataAsync()
    {
        using var scope = Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();

        Site[] sites;
        Role[] roles;

        if (!ctx.Sites.Any())
        {
            (sites, roles) = await DbInitializer.CreateRequredData(ctx);
        }
        else
        {
            sites = await ctx.Sites.ToArrayAsync();
            roles = await ctx.Roles.ToArrayAsync();
        }

        List<User> users = [
            new() { EmailAddress = "user1@example.com", UserName = "user1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Role = roles[0] },
            new() { EmailAddress = "user2@example.com", UserName = "user2", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Role = roles[0] },
            new() { EmailAddress = "mod1@example.com", UserName = "mod1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Role = roles[1] }
        ];

        await ctx.Users.AddRangeAsync(users);
        await ctx.SaveChangesAsync();

        List<Post> posts = [
            new Post() { Body = "This is a post for testing", Title = "Test Post", PostedBy = users[0], Site = sites[0], PostedOn = new(2026, 07, 02, 12, 00, 00, DateTimeKind.Utc) },
            new Post() { Body = "This is a post for testing with a diff body", Title = "Test Post 2", PostedBy = users[1], Site = sites[0], PostedOn = new(2026, 07, 02, 11, 00, 00, DateTimeKind.Utc) },
            new Post() { Body = "This is a post for testing on another site", Title = "Test Post", PostedBy = users[0], Site = sites[1], PostedOn = new(2026, 07, 02, 12, 00, 00, DateTimeKind.Utc) },
        ];


        await ctx.Posts.AddRangeAsync(posts);
        await ctx.SaveChangesAsync();
    }
}
