using System.ComponentModel.DataAnnotations;

namespace Vehicles_API.ViewModels.Authorization
{
    public class LoginViewModel
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}