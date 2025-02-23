using System.Linq.Expressions;
using Common.Data;
using Microsoft.EntityFrameworkCore;
using PaymentService.Models;

namespace PaymentService.Data;

/// <summary>
///  A base repository class that implements the <see cref="IPaymentRepo{T}">IRepository</see> interface
/// </summary>
/// <param name="context">
///  The database context to be used
/// </param>
/// <inheritdoc cref="IPaymentRepo{T}"/>
public class PaymentRepo(AppDbContext context) : IPaymentRepo
{
    /// <inheritdoc />
    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await context.Set<Payment>().FindAsync(id);
    }
    
    /// <inheritdoc />
    public async Task<List<Payment>> ListAsync()
    {
        return await context.Set<Payment>().ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Payment> AddAsync(Payment entity)
    {
        await context.Set<Payment>().AddAsync(entity);
        return entity;
    }

    /// <inheritdoc />
    public void DeleteAsync(Payment entity)
    {
        context.Set<Payment>().Remove(entity);
    }

    /// <inheritdoc />
    public virtual async Task<PagedResult<Payment>> GetAllPagedAsync(int pageIndex, int pageSize)
    {
        var items = await context.Set<Payment>().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        var count = await context.Set<Payment>().CountAsync();
        return new PagedResult<Payment>(items, count, pageIndex, pageSize);
    }

    /// <inheritdoc />
    public async Task<PagedResult<Payment>> GetSortedByPagedAsync(int pageIndex, int pageSize,
        Expression<Func<Payment, object>> orderBy, bool ascending = true)
    {
        var items = ascending
            ? await context.Set<Payment>().OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync()
            : await context.Set<Payment>().OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .ToListAsync();
        var count = items.Count;
        return new PagedResult<Payment>(items, count, pageIndex, pageSize);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Payment>> GetSortByAsync(Expression<Func<Payment, object>> orderBy, bool ascending = true)
    {
        return ascending
            ? await context.Set<Payment>().OrderBy(orderBy).ToListAsync()
            : await context.Set<Payment>().OrderByDescending(orderBy).ToListAsync();
    }


    /// <inheritdoc />
    public async Task<PagedResult<Payment>> GetAllWhereAsync(Expression<Func<Payment, bool>> condition, int pageIndex, int pageSize)
    {
        var items = await context.Set<Payment>().Where(condition).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        var count = await context.Set<Payment>().Where(condition).CountAsync();
        return new PagedResult<Payment>(items, count, pageIndex, pageSize);
    }

    /// <inheritdoc />
    public async Task<Payment?> GetFirstWhereAsync(Expression<Func<Payment, bool>> condition)
    {
        var item = await context.Set<Payment>().FirstOrDefaultAsync(condition);
        return item;
    }

    /// <inheritdoc />
    public async Task<Payment?> GetLastWhereAsync(Expression<Func<Payment, bool>> condition)
    {
        var item = await context.Set<Payment>().LastOrDefaultAsync(condition);
        return item;
    }

    /// <inheritdoc />
    public async Task<int> CountWhereAsync(Expression<Func<Payment, bool>> condition)
    {
        var count = await context.Set<Payment>().CountAsync(condition);
        return count;
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(Expression<Func<Payment, bool>> condition)
    {
        var any = await context.Set<Payment>().AnyAsync(condition);
        return any;
    }
    
    /// <inheritdoc />
    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}