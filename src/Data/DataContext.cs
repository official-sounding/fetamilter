using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{

    public DbSet<Post> Posts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public DbSet<PostFavorite> PostFavorites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Site>()
            .ToTable("Site")
            .HasIndex((s) => s.Slug, "idx_site_slug");

        modelBuilder.Entity<User>()
            .ToTable("User")
            .HasIndex((u) => u.UserName, "idx_user_username").IsUnique();

        modelBuilder.Entity<Comment>()
            .ToTable("Comment");


        modelBuilder.Entity<Post>()
        .ToTable("Post")
        .HasIndex("SiteID", nameof(Post.Number));

        modelBuilder.Entity<PostFavorite>()
        .ToTable("PostFavorite")
        .HasIndex((pf) => pf.PostID, "idx_postfavorite_postID");

        modelBuilder.Entity<PostFavorite>().HasIndex((pf) => pf.UserID, "idx_postfavorite_userID");

        modelBuilder.Entity<Role>()
            .ToTable("Role");

        if (Database.IsSqlite())
        {
            modelBuilder.Entity<User>().Property((u) => u.UserName).UseCollation("NOCASE");
        }
    }

}
