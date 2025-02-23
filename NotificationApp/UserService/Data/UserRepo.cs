using System.Linq.Expressions;
using Common.Data;
using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;


/// <summary>
///  A base repository class that implements the <see cref="IUserRepo{T}">IRepository</see> interface
/// </summary>
/// <param name="context">
///  The database context to be used
/// </param>
/// <inheritdoc cref="IUserRepo{T}"/>
public class UserRepo(AppDbContext context) : IUserRepo
{
    /// <inheritdoc />
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.Set<User>().FindAsync(id);
    }
    
    /// <inheritdoc />
    public async Task<List<User>> ListAsync()
    {
        return await context.Set<User>().ToListAsync();
    }

    /// <inheritdoc />
    public async Task<User> AddAsync(User entity)
    {
        await context.Set<User>().AddAsync(entity);
        return entity;
    }

    /// <inheritdoc />
    public void DeleteAsync(User entity)
    {
        context.Set<User>().Remove(entity);
    }

    /// <inheritdoc />
    public virtual async Task<PagedResult<User>> GetAllPagedAsync(int pageIndex, int pageSize)
    {
        var items = await context.Set<User>().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        var count = await context.Set<User>().CountAsync();
        return new PagedResult<User>(items, count, pageIndex, pageSize);
    }

    /// <inheritdoc />
    public async Task<PagedResult<User>> GetSortedByPagedAsync(int pageIndex, int pageSize,
        Expression<Func<User, object>> orderBy, bool ascending = true)
    {
        var items = ascending
            ? await context.Set<User>().OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync()
            : await context.Set<User>().OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .ToListAsync();
        var count = items.Count;
        return new PagedResult<User>(items, count, pageIndex, pageSize);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetSortByAsync(Expression<Func<User, object>> orderBy, bool ascending = true)
    {
        return ascending
            ? await context.Set<User>().OrderBy(orderBy).ToListAsync()
            : await context.Set<User>().OrderByDescending(orderBy).ToListAsync();
    }


    /// <inheritdoc />
    public async Task<PagedResult<User>> GetAllWhereAsync(Expression<Func<User, bool>> condition, int pageIndex, int pageSize)
    {
        var items = await context.Set<User>().Where(condition).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        var count = await context.Set<User>().Where(condition).CountAsync();
        return new PagedResult<User>(items, count, pageIndex, pageSize);
    }

    /// <inheritdoc />
    public async Task<User?> GetFirstWhereAsync(Expression<Func<User, bool>> condition)
    {
        var item = await context.Set<User>().FirstOrDefaultAsync(condition);
        return item;
    }

    /// <inheritdoc />
    public async Task<User?> GetLastWhereAsync(Expression<Func<User, bool>> condition)
    {
        var item = await context.Set<User>().LastOrDefaultAsync(condition);
        return item;
    }

    /// <inheritdoc />
    public async Task<int> CountWhereAsync(Expression<Func<User, bool>> condition)
    {
        var count = await context.Set<User>().CountAsync(condition);
        return count;
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(Expression<Func<User, bool>> condition)
    {
        var any = await context.Set<User>().AnyAsync(condition);
        return any;
    }
    
    /// <inheritdoc />
    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}