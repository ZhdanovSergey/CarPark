using CarPark.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarPark.APIModels
{
    public class ManagerAPIModel
    {
        public int Id { get; set; }
        public List<int> EnterprisesIds { get; set; } = new();
        public ManagerAPIModel(Manager manager)
        {
            Id = manager.Id;
            EnterprisesIds = manager.EnterprisesManagers
                .Select(em => em.EnterpriseId).ToList();
        }
    }
}
