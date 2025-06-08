using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public static class DbInitializer
{
    public static async Task Initialize(DataContext ctx, bool generateTestData)
    {
        await ctx.Database.EnsureCreatedAsync();

        Site[] sites;
        if (!ctx.Sites.Any())
        {
            sites = [
                new() { Slug = "www", Title = "FetaMilter" },
                new() { Slug = "ask", Title = "AskFeta" },
                new() { Slug = "meta", Title = "FetaTalk" }
            ];

            foreach (var s in sites)
            {
                await ctx.Sites.AddAsync(s);
            }

            await ctx.SaveChangesAsync();
        }
        else
        {
            sites = await ctx.Sites.ToArrayAsync();
        }

        var sitesBySlug = sites.ToDictionary((s) => s.Slug);

        if (generateTestData && !ctx.Users.Any())
        {
            var users = Enumerable
                .Range(0, 100)
                .Select((_) => new User()
                {
                    UserName = Faker.Internet.UserName(),
                    EmailAddress = Faker.Internet.Email(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Faker.Internet.UserName()),
                    CreatedOn = DateTime.UtcNow.AddYears(-5).AddMonths(-1 * Random.Shared.Next(0, 10)).AddHours(Random.Shared.Next(0, 12))
                })
                .Append(new()
                {
                    UserName = "testing",
                    EmailAddress = "testing@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                    CreatedOn = DateTime.UtcNow
                })
                .ToArray();

            await ctx.Users.AddRangeAsync(users);
            await ctx.SaveChangesAsync();


            var date = DateTime.UtcNow;
            for (var j = 0; j < 1000; j++)
            {
                var user = users[Random.Shared.Next(1, 100)];
                var site = sites[j % 3];


                var post = await ctx.Posts.AddAsync(new Post()
                {
                    Site = site,
                    PostedBy = user,
                    PostedOn = date,
                    Title = Faker.Lorem.Sentence(),
                    Body = string.Join("\n", Faker.Lorem.Paragraphs(Random.Shared.Next(2, 5))),
                    MoreInside = Random.Shared.Next(0, 3) == 1 ? string.Empty : string.Join("\n", Faker.Lorem.Paragraphs(3)),
                });


                var comments = Enumerable.Range(0, Random.Shared.Next(0, 5)).Select((_) => new Comment()
                {
                    PostedBy = users[Random.Shared.Next(1, 100)],
                    PostedOn = date.AddMinutes(Random.Shared.Next(1, 30)),
                    Post = post.Entity,
                    Body = string.Join("\n", Faker.Lorem.Paragraphs(2))
                });

                await ctx.Comments.AddRangeAsync(comments);

                date = date.AddHours(-1 * Random.Shared.Next(1, 4));
            }

            await ctx.SaveChangesAsync();
        }
    }
}
