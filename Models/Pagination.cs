namespace CarPark.Models;

public class Pagination<TDataItem> where TDataItem : class
{
    public IEnumerable<TDataItem> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public Pagination(IEnumerable<TDataItem> items, int pageSize, int pageNumber)
    {
        var totalItems = items.Count();

        Items = items.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    }
}
