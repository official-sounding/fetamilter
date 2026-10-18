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
    public DbSet<Tag> Tags { get; set; }

    public DbSet<PostFavorite> PostFavorites { get; set; }
    public DbSet<CommentFavorite> CommentFavorites { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseNpgsql(x => x.MigrationsAssembly("PgsqlMigrations"))
        .UseSnakeCaseNamingConvention();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Site>()
            .ToTable("site")
            .HasIndex((s) => s.Slug, "idx_site_slug");

        modelBuilder.Entity<User>()
            .ToTable("user")
            .HasIndex((u) => u.UserName, "idx_user_username").IsUnique();

        modelBuilder.Entity<Comment>()
            .ToTable("comment");


        modelBuilder.Entity<Post>()
        .ToTable("post")
        .HasIndex(nameof(Post.SiteID), nameof(Post.Number));

        modelBuilder.Entity<Post>().HasOne(p => p.StateUpdatedBy).WithMany();
        modelBuilder.Entity<Comment>().HasOne(p => p.RemovedBy).WithMany();

        modelBuilder.Entity<PostFavorite>()
        .ToTable("post_favorite");

        modelBuilder.Entity<CommentFavorite>()
        .ToTable("comment_favorite");

        modelBuilder.Entity<Tag>()
        .ToTable("tag")
        .HasIndex((t) => t.Name, "idx_tag_name").IsUnique();


        modelBuilder.Entity<Role>()
            .ToTable("role");


        modelBuilder.Entity<User>().Property((u) => u.UserName).HasColumnType("citext");
        modelBuilder.Entity<Tag>().Property(t => t.Name).HasColumnType("citext");
    }

}
