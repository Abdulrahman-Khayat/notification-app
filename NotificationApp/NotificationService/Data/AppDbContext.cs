using Microsoft.EntityFrameworkCore;
using NotificationService.Models;


namespace NotificationService.Data;
/// <summary>
///     Db Context for the application
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Template> Templates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Template>()
        .HasIndex(p => p.Name)
            .IsUnique();
    }

}