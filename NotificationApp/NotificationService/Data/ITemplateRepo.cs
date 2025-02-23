using System.Linq.Expressions;
using Common.Data;
using NotificationService.Models;

namespace NotificationService.Data;

/// <summary>
///   A generic repository interface that defines the basic operations that can be performed on an entity
/// </summary>
/// <typeparam name="T">
///  The type of the entity to be used
/// </typeparam>
/// <typeparam name="TKey">
///     The type of the entity's primary key
/// </typeparam>
public interface ITemplateRepo
{
    Task<Template?> GetByIdAsync(Guid id);
    Task<List<Template>> ListAsync();
    Task<Template> AddAsync(Template entity);
    void DeleteAsync(Template entity);
    Task<List<Template>> GetAll();

    /// <summary>
    ///     Get all entities with pagination support
    /// </summary>
    /// <param name="pageIndex">
    ///     The index of the page to be retrieved
    /// </param>
    /// <param name="pageSize">
    ///     The number of items in each page
    /// </param>
    /// <returns>
    ///    A paged result of entities wrapped <see cref="PagedResult{T}">PagedResult</see>
    /// </returns>
    Task<PagedResult<Template>> GetAllPagedAsync(int pageIndex, int pageSize);

    /// <summary>
    ///   Get all entities with pagination support and sorting
    /// </summary>
    /// <param name="pageIndex">
    ///   The index of the page to be retrieved
    /// </param>
    /// <param name="pageSize">
    ///  The number of items in each page
    /// </param>
    /// <param name="orderBy">
    ///  The property to be sorted by
    /// </param>
    /// <param name="ascending">
    ///   Whether the sorting is ascending or descending
    /// </param>
    /// <returns>
    ///     A paged result of entities wrapped <see cref="PagedResult{T}">PagedResult</see>
    /// </returns>
    Task<PagedResult<Template>> GetSortedByPagedAsync(int pageIndex, int pageSize, Expression<Func<Template, object>> orderBy,
        bool ascending = true);

    /// <summary>
    ///   Get all entities sorted by a property
    /// </summary>
    /// <param name="orderBy">
    ///     The property to be sorted by
    /// </param>
    /// <param name="ascending">
    ///    Whether the sorting is ascending or descending
    /// </param>
    /// <returns>
    ///   A list of entities sorted by the property
    /// </returns>
    Task<IEnumerable<Template>> GetSortByAsync(Expression<Func<Template, object>> orderBy, bool ascending = true);


    /// <summary>
    ///    Get all entities that satisfy the condition
    /// </summary>
    /// <param name="condition">
    ///    The condition to be satisfied
    /// </param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns>
    ///   A list of entities that satisfy the condition
    /// </returns>
    Task<PagedResult<Template>> GetAllWhereAsync(Expression<Func<Template, bool>> condition, int pageIndex, int pageSize);

    
    /// <summary>
    ///   Get the first entity that satisfies the condition
    /// </summary>
    /// <param name="condition">
    ///     The condition to be satisfied
    /// </param>
    /// <returns >
    ///   The first entity that satisfies the condition
    /// </returns>
    Task<Template?> GetFirstWhereAsync(Expression<Func<Template, bool>> condition);

    /// <summary>
    ///  Get the last entity that satisfies the condition
    /// </summary>
    /// <param name="condition">
    ///  The condition to be satisfied
    /// </param>
    /// <returns>
    ///     The last entity that satisfies the condition
    /// </returns>
    Task<Template?> GetLastWhereAsync(Expression<Func<Template, bool>> condition);

    /// <summary>
    ///  Count the number of entities that satisfy the condition
    /// </summary>
    /// <param name="condition">
    /// The condition to be satisfied
    /// </param>
    /// <returns>
    ///  The number of entities that satisfy the condition
    /// </returns>
    Task<int> CountWhereAsync(Expression<Func<Template, bool>> condition);

    /// <summary>
    /// Check if there is any entity that satisfies the condition
    /// </summary>
    /// <param name="condition">
    /// The condition to be satisfied
    /// </param>
    /// <returns>
    /// Whether there is any entity that satisfies the condition
    /// </returns>
    Task<bool> AnyAsync(Expression<Func<Template, bool>> condition);


    /// <summary>
    /// Save changes to the database
    /// </summary>
    /// <returns>
    ///     The number of entities that were saved
    /// </returns>
    Task<int> SaveChangesAsync();
}