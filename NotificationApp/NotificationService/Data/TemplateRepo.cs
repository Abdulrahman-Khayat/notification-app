using System.Linq.Expressions;
using Common.Data;
using Microsoft.EntityFrameworkCore;
using NotificationService.Models;

namespace NotificationService.Data;


/// <summary>
///  A base repository class that implements the <see cref="ITemplateRepo{T}">IRepository</see> interface
/// </summary>
/// <param name="context">
///  The database context to be used
/// </param>
/// <inheritdoc cref="ITemplateRepo{T}"/>
public class TemplateRepo(AppDbContext context) : ITemplateRepo
{
    /// <inheritdoc />
    public async Task<Template?> GetByIdAsync(Guid id)
    {
        return await context.Set<Template>().FindAsync(id);
    }
    
    /// <inheritdoc />
    public async Task<List<Template>> ListAsync()
    {
        return await context.Set<Template>().ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Template> AddAsync(Template entity)
    {
        await context.Set<Template>().AddAsync(entity);
        return entity;
    }

    /// <inheritdoc />
    public void DeleteAsync(Template entity)
    {
        context.Set<Template>().Remove(entity);
    }
    
    /// <inheritdoc />
    public virtual async Task<List<Template>> GetAll()
    {
        return await context.Set<Template>().ToListAsync();
    }
    
    /// <inheritdoc />
    public virtual async Task<PagedResult<Template>> GetAllPagedAsync(int pageIndex, int pageSize)
    {
        var items = await context.Set<Template>().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        var count = await context.Set<Template>().CountAsync();
        return new PagedResult<Template>(items, count, pageIndex, pageSize);
    }

    /// <inheritdoc />
    public async Task<PagedResult<Template>> GetSortedByPagedAsync(int pageIndex, int pageSize,
        Expression<Func<Template, object>> orderBy, bool ascending = true)
    {
        var items = ascending
            ? await context.Set<Template>().OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync()
            : await context.Set<Template>().OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .ToListAsync();
        var count = items.Count;
        return new PagedResult<Template>(items, count, pageIndex, pageSize);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Template>> GetSortByAsync(Expression<Func<Template, object>> orderBy, bool ascending = true)
    {
        return ascending
            ? await context.Set<Template>().OrderBy(orderBy).ToListAsync()
            : await context.Set<Template>().OrderByDescending(orderBy).ToListAsync();
    }


    /// <inheritdoc />
    public async Task<PagedResult<Template>> GetAllWhereAsync(Expression<Func<Template, bool>> condition, int pageIndex, int pageSize)
    {
        var items = await context.Set<Template>().Where(condition).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        var count = await context.Set<Template>().Where(condition).CountAsync();
        return new PagedResult<Template>(items, count, pageIndex, pageSize);
    }

    /// <inheritdoc />
    public async Task<Template?> GetFirstWhereAsync(Expression<Func<Template, bool>> condition)
    {
        var item = await context.Set<Template>().FirstOrDefaultAsync(condition);
        return item;
    }

    /// <inheritdoc />
    public async Task<Template?> GetLastWhereAsync(Expression<Func<Template, bool>> condition)
    {
        var item = await context.Set<Template>().LastOrDefaultAsync(condition);
        return item;
    }

    /// <inheritdoc />
    public async Task<int> CountWhereAsync(Expression<Func<Template, bool>> condition)
    {
        var count = await context.Set<Template>().CountAsync(condition);
        return count;
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(Expression<Func<Template, bool>> condition)
    {
        var any = await context.Set<Template>().AnyAsync(condition);
        return any;
    }
    
    /// <inheritdoc />
    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}