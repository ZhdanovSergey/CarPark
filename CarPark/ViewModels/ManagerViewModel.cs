using CarPark.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarPark.ViewModels
{
    public class ManagerViewModel
    {
        public int Id { get; set; }
        public List<int> EnterprisesIds { get; set; } = new();
        public MultiSelectList? EnterprisesSelectList { get; set; }
        public ManagerViewModel() { }
        public ManagerViewModel(Manager manager)
        {
            Id = manager.Id;
            EnterprisesIds = manager.EnterprisesManagers.Select(em => em.EnterpriseId).ToList();
        }
        public async Task AddEditSelectLists(ApplicationDbContext dbContext)
        {
            this.EnterprisesSelectList = new MultiSelectList(await dbContext.Enterprises.ToListAsync(), "Id", "Name");
        }
    }
}
