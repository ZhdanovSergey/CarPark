namespace CarPark.Models;

public class EnterpriseManager
{
    public int EnterpriseId { get; set; }
    public int ManagerId { get; set; }
    public Enterprise? Enterprise { get; set; }
    public Manager? Manager { get; set; }
    public static void Update(ApplicationDbContext dbContext, IEnumerable<EnterpriseManager> oldData, IEnumerable<EnterpriseManager> newData, EnterpriseManagerIdProp comparedIdProp)
    {
        foreach (var oldDataItem in oldData)
        {
            if (!newData.Any(newDataItem => newDataItem[comparedIdProp] == oldDataItem[comparedIdProp]))
                dbContext.EnterprisesManagers.Remove(oldDataItem);
        }

        foreach (var newDataItem in newData)
        {
            if (!oldData.Any(oldDataItem => oldDataItem[comparedIdProp] == newDataItem[comparedIdProp]))
                dbContext.EnterprisesManagers.Add(newDataItem);
        }
    }
    int this[EnterpriseManagerIdProp idProp]
    {
        get => idProp switch
        {
            EnterpriseManagerIdProp.EnterpriseId => this.EnterpriseId,
            EnterpriseManagerIdProp.ManagerId => this.ManagerId,
        };
    }
}
public enum EnterpriseManagerIdProp
{
    EnterpriseId,
    ManagerId
}
