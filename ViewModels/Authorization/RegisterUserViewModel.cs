namespace Vehicles_API.ViewModels.Authorization
{
    public class RegisterUserViewModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}