using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarPark.Models;

public class EnterpriseFilter
{
    public int SelectedId { get; set; }
    public IEnumerable<Enterprise> UserEnterprises { get; set;} = new List<Enterprise>();
    public static async Task<EnterpriseFilter> EnterpriseFilterAsync
    (
        ApplicationDbContext dbContext,
        ClaimsPrincipal claimsPrincipal,
        int selectedId
    )
    {
        return new EnterpriseFilter()
        {
            SelectedId = selectedId,
            UserEnterprises = await Enterprise.GetUserEnterprises(dbContext, claimsPrincipal).ToListAsync(),
        };
    }
}
