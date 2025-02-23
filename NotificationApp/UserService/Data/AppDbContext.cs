using Common.Models;
using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

/// <summary>
///     Db Context for the application
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        AddTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is BaseModel && (x.State == EntityState.Added || x.State == EntityState.Modified));
    
        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow; // current datetime
    
            if (entity.State == EntityState.Added)
            {
                ((BaseModel) entity.Entity).CreatedAt = now;
            }
    
            ((BaseModel) entity.Entity).UpdatedAt = now;
        }
    }
}