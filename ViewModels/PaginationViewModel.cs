namespace CarPark.ViewModels;

public class PaginationViewModel<TDataItem> where TDataItem : class
{
    public IEnumerable<TDataItem> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginationViewModel(IEnumerable<TDataItem> items, int totalItems, int pageSize, int pageNumber)
    {
        Items = items;
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    }
}
