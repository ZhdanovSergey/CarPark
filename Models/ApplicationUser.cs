using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CarPark.Models
{
    public class ApplicationUser : IdentityUser<int> {
        public static int GetUserId(ClaimsPrincipal? claimsPrincipal)
        {
            var userId = claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            return Int32.Parse(userId);
        }
    }
}
