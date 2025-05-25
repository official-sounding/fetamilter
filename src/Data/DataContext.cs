using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{

    public DbSet<Post> Posts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Site> Sites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Site>().ToTable("Site");
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Post>().ToTable("Post");
    }

}
