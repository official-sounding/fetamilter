using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{

    public DbSet<Post> Posts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Site>()
            .ToTable("Site")
            .HasIndex((s) => s.Slug, "idx_site_slug");

        modelBuilder.Entity<User>()
            .ToTable("User")
            .HasIndex((u) => u.UserName, "idx_user_username");

        modelBuilder.Entity<Comment>()
            .ToTable("Comment");



        modelBuilder.Entity<Post>()
        .ToTable("Post")
        .HasIndex("SiteID", nameof(Post.Number));

        if (Database.IsSqlite())
        {
            modelBuilder.Entity<User>().Property((u) => u.UserName).UseCollation("NOCASE");
        }
    }

}
