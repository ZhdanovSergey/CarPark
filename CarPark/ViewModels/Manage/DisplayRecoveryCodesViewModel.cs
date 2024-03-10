using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarPark.ViewModels.Manage
{
    public class DisplayRecoveryCodesViewModel
    {
        [Required]
        public IEnumerable<string> Codes { get; set; }

    }
}
