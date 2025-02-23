namespace Common.Data;

/// <summary>
///   A generic class that represents a paged result
/// </summary>
/// <typeparam name="T">
///    The type of the items in the paged result
/// </typeparam>
public class PagedResult<T>
{

    /// <summary>
    ///    Items in the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; }

    private const int MaxPageSize = 25;

    /// <summary>
    ///     Total number of items in the data source
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    ///     Number of items in each page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    ///     Current page index
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    ///     Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    ///     Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>
    ///     Whether there is a next page
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages;

    /// <summary>
    ///     Constructor for the <see cref="PagedResult{T}">PagedResult</see> class
    /// </summary>
    /// <param name="items">
    ///     Items in the current page
    /// </param>
    /// <param name="totalCount">
    ///     Total number of items in the data source
    /// </param>
    /// <param name="pageIndex">
    ///     Current page index
    /// </param>
    /// <param name="pageSize">
    ///    Number of items in each page
    /// </param>
    public PagedResult(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize > MaxPageSize ? MaxPageSize : pageSize;
    }
}