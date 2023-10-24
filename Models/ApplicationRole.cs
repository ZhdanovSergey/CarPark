using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CarPark.Models
{
    public class ApplicationRole : IdentityRole<int> {}

    public static class RoleNames
    {
        public const string
            Admin = "admin",
            Manager = "manager";

    }
}
