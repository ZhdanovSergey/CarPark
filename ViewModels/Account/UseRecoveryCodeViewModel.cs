using System.ComponentModel.DataAnnotations;

namespace CarPark.ViewModels.Account
{
    public class UseRecoveryCodeViewModel
    {
        [Required]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }
    }
}
