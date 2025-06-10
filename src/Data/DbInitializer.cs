using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public static class DbInitializer
{
    public static async Task Initialize(DataContext ctx, bool generateTestData)
    {
        await ctx.Database.EnsureCreatedAsync();

        Site[] sites;
        Role[] roles;

        if (!ctx.Sites.Any())
        {
            sites = [
                new() { Slug = "www", Title = "FetaMilter" },
                new() { Slug = "ask", Title = "AskFeta" },
                new() { Slug = "meta", Title = "FetaTalk" }
            ];

            roles = [
                new() { Name = "User" },
                new() { Name = "Moderator", NameTag = "Staff" }
            ];


            await ctx.Sites.AddRangeAsync(sites);
            await ctx.Roles.AddRangeAsync(roles);
            await ctx.SaveChangesAsync();
        }
        else
        {
            sites = await ctx.Sites.ToArrayAsync();
            roles = await ctx.Roles.ToArrayAsync();
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
                    CreatedOn = DateTime.UtcNow.AddYears(-5).AddMonths(-1 * Random.Shared.Next(0, 10)).AddHours(Random.Shared.Next(0, 12)),
                    Role = roles[0]
                })
                .Append(new()
                {
                    UserName = "testing-mod",
                    EmailAddress = "testing@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                    CreatedOn = DateTime.UtcNow,
                    Role = roles[1]

                })
                .DistinctBy(u => u.UserName)
                .ToArray();

            await ctx.Users.AddRangeAsync(users);
            await ctx.SaveChangesAsync();


            var date = DateTime.UtcNow;
            int[] siteCounts = [1, 1, 1];
            for (var j = 0; j < 1000; j++)
            {
                var user = users[Random.Shared.Next(0, users.Length - 2)];
                var site = sites[j % 3];
                var num = siteCounts[j % 3];


                var post = await ctx.Posts.AddAsync(new Post()
                {
                    Site = site,
                    Number = num,
                    PostedBy = user,
                    PostedOn = date,
                    Title = Faker.Lorem.Sentence(),
                    Body = string.Join("\n", Faker.Lorem.Paragraphs(Random.Shared.Next(2, 5))),
                    MoreInside = Random.Shared.Next(0, 3) == 1 ? string.Empty : string.Join("\n\n", Faker.Lorem.Paragraphs(3)),
                });


                var comments = Enumerable.Range(0, Random.Shared.Next(0, 5)).Select((_) => new Comment()
                {
                    PostedBy = users[Random.Shared.Next(1, 100)],
                    PostedOn = date.AddMinutes(Random.Shared.Next(1, 30)),
                    Post = post.Entity,
                    Body = string.Join("\n\n", Faker.Lorem.Paragraphs(2))
                });

                await ctx.Comments.AddRangeAsync(comments);

                siteCounts[j % 3]++;
                date = date.AddHours(-1 * Random.Shared.Next(1, 4));
            }

            await ctx.SaveChangesAsync();
        }
    }
}
