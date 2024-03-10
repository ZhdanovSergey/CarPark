using CarPark.Models;

namespace CarPark.ViewModels
{
    public class PaginationWithEnterpriseFilter<TDataItem> where TDataItem : class
    {
         public Pagination<TDataItem> Pagination { get; set; }
         public EnterpriseFilter EnterpriseFilter { get; set; }
    }
}
