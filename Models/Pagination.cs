using Microsoft.EntityFrameworkCore;

namespace CarPark.Models;

public class Pagination<TDataItem> where TDataItem : class
{
    public IEnumerable<TDataItem> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public Pagination(IEnumerable<TDataItem> itemsForPage, int totalItems, int pageSize, int pageNumber)
    {
        Items = itemsForPage;
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    }
    public static async Task<Pagination<TDataItem>> PaginationAsync(IQueryable<TDataItem> allItems, int pageSize, int pageNumber)
    {
        var totalItems = await allItems.CountAsync();

        var itemsForPage = await allItems
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new Pagination<TDataItem>(itemsForPage, totalItems, pageSize, pageNumber);
    }
}
